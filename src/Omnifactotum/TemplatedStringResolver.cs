using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable once UseNullableReferenceTypesAnnotationSyntax

namespace Omnifactotum;

/// <summary>
///     Represents a resolver of templated strings.
///     A templated string is defined in a way similar to C# interpolated string (see <see cref="TemplateVariables"/> and <see cref="Resolve"/>).
/// </summary>
public sealed class TemplatedStringResolver
{
    /// <summary>
    ///     The default <see cref="IEqualityComparer{T}"/> used to compare the names of the template variables.
    /// </summary>
    /// <seealso cref="TemplateVariableNameComparer"/>
    /// <seealso cref="TemplateVariables"/>
    public static readonly StringComparer DefaultTemplateVariableNameComparer = StringComparer.Ordinal;

    private delegate void AppendAction(ReadOnlySpan<char> value);

    /// <summary>
    ///     Initializes a new instance of the <see cref="TemplatedStringResolver"/> class using the specified template variables.
    /// </summary>
    /// <param name="templateVariables">
    ///     The template variables. See <see cref="TemplateVariables"/> for more details.
    /// </param>
    /// <param name="templateVariableNameComparer">
    ///     <see cref="IEqualityComparer{T}"/> used to compare the names of the template variables,
    ///     or <see langword="null"/> to use <see cref="DefaultTemplateVariableNameComparer"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="templateVariables"/> is <see langword="null"/>.
    /// </exception>
    public TemplatedStringResolver(
        [NotNull] IReadOnlyDictionary<string, string> templateVariables,
        [CanBeNull] IEqualityComparer<string>? templateVariableNameComparer = null)
    {
        if (templateVariables is null)
        {
            throw new ArgumentNullException(nameof(templateVariables));
        }

        var resolvedTemplateVariableNameComparer = ResolveVariableNameComparer(templateVariableNameComparer);

        TemplateVariableNameComparer = resolvedTemplateVariableNameComparer;
        TemplateVariables = templateVariables.ToImmutableDictionary(resolvedTemplateVariableNameComparer);

        var invalidVariableNames = TemplateVariables
            .Keys
            .Where(key => !Constants.ValidVariableNameRegex.IsMatch(key))
            .OrderBy(Factotum.For<string>.IdentityMethod)
            .ToArray();

        if (invalidVariableNames.Length != 0)
        {
            throw new ArgumentException($@"The following variable names are invalid: {invalidVariableNames.ToUIString()}.", nameof(templateVariables));
        }
    }

    /// <summary>
    ///     <see cref="IEqualityComparer{T}"/> used to compare the names of the template variables.
    /// </summary>
    /// <seealso cref="TemplatedStringResolver(IReadOnlyDictionary{string, string}, IEqualityComparer{string})"/>
    [NotNull]
    public IEqualityComparer<string> TemplateVariableNameComparer { get; }

    /// <summary>
    ///     Gets the dictionary having the template variable names as its keys and the corresponding template variable values as its values.
    ///     How the variable names are compared depends on <see cref="TemplateVariableNameComparer"/>.
    /// </summary>
    [NotNull]
    public ImmutableDictionary<string, string> TemplateVariables { get; }

    /// <summary>
    ///     Determines the names of the template variables used in the specified templated string.
    /// </summary>
    /// <param name="templatedString">
    ///     The templated string to determine the names of the template variables in.
    /// </param>
    /// <param name="templateVariableNameComparer">
    ///     <see cref="IEqualityComparer{T}"/> used to compare the names of the template variables.
    /// </param>
    /// <param name="tolerateUnexpectedTokens">
    ///     Specifies whether unexpected tokens should be tolerated.
    ///     Corresponds to the <see cref="TemplatedStringResolverOptions.TolerateUnexpectedTokens"/> flag.
    /// </param>
    /// <returns>
    ///     A <see cref="HashSet{T}"/> containing the unique names of the template variables used in the specified templated string.
    /// </returns>
    /// <exception cref="TemplatedStringResolverException">
    ///     There is an error in the templated string.
    /// </exception>
    [NotNull]
    [ItemNotNull]
    public static HashSet<string> GetVariableNames(
        [NotNull] string templatedString,
        [CanBeNull] IEqualityComparer<string>? templateVariableNameComparer = null,
        bool tolerateUnexpectedTokens = false)
    {
        var resolvedTemplateVariableNameComparer = ResolveVariableNameComparer(templateVariableNameComparer);

        var result = new HashSet<string>(resolvedTemplateVariableNameComparer);

        string? OnResolveVariable(string variableName)
        {
            result.Add(variableName);
            return null;
        }

        var options = TemplatedStringResolverOptions.TolerateUndefinedVariables;

        if (tolerateUnexpectedTokens)
        {
            options |= TemplatedStringResolverOptions.TolerateUnexpectedTokens;
        }

        ProcessTemplatedString(templatedString, OnResolveVariable, null, options);

        return result;
    }

    /// <summary>
    ///     Resolves the specified templated string.
    /// </summary>
    /// <param name="templatedString">
    ///     The templated string to resolve.
    /// </param>
    /// <param name="options">
    ///     The options specifying how the resolver should behave.
    /// </param>
    /// <returns>
    ///     The resolved templated string in which the variable placeholders are replaced with the corresponding variable names.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="templatedString"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="TemplatedStringResolverException">
    ///     There is an error in the templated string.
    /// </exception>
    [NotNull]
    public string Resolve(
        [NotNull] string templatedString,
        TemplatedStringResolverOptions options = TemplatedStringResolverOptions.None)
    {
        if (templatedString is null)
        {
            throw new ArgumentNullException(nameof(templatedString));
        }

        var resultBuilder = new StringBuilder(templatedString.Length);

        ProcessTemplatedString(
            templatedString,
            variableName => TemplateVariables.TryGetValue(variableName, out var variableValue) ? variableValue : null,
            value => resultBuilder.Append(value),
            options);

        return resultBuilder.ToString();
    }

    private static IEqualityComparer<string> ResolveVariableNameComparer([CanBeNull] IEqualityComparer<string>? templateVariableNameComparer)
        => templateVariableNameComparer ?? DefaultTemplateVariableNameComparer;

    private static void ProcessTemplatedString(
        [NotNull] string templatedString,
        [NotNull] Func<string, string?> onResolveVariable,
        [CanBeNull] AppendAction? onAppend,
        TemplatedStringResolverOptions options)
    {
        if (templatedString is null)
        {
            throw new ArgumentNullException(nameof(templatedString));
        }

        var index = 0;
        while (index < templatedString.Length)
        {
            var match = Constants.TemplateRegex.Match(templatedString, index);
            if (!match.Success)
            {
                onAppend?.Invoke(templatedString.AsSpan(index));
                break;
            }

            onAppend?.Invoke(templatedString.AsSpan(index, match.Index - index));
            index = match.Index + match.Length;

            var openingBraceGroup = match.Groups[Constants.GroupNames.OpeningBrace];
            if (openingBraceGroup.Success)
            {
                onAppend?.Invoke(Constants.OpeningBraceChar);
                continue;
            }

            var closingBraceGroup = match.Groups[Constants.GroupNames.ClosingBrace];
            if (closingBraceGroup.Success)
            {
                onAppend?.Invoke(Constants.ClosingBraceChar);
                continue;
            }

            var variableNameGroup = match.Groups[Constants.GroupNames.VariableName];
            if (variableNameGroup.Success)
            {
                var variableName = variableNameGroup.Value;
                var variableValue = onResolveVariable(variableName);
                if (variableValue is null)
                {
                    if (options.IsAnySet(TemplatedStringResolverOptions.TolerateUndefinedVariables))
                    {
                        continue;
                    }

                    throw new TemplatedStringResolverException(
                        $@"Error at index {match.Index}: the injected variable {variableName.ToUIString()} is not defined.");
                }

                onAppend?.Invoke(variableValue);
                continue;
            }

            var unexpectedTokenGroup = match.Groups[Constants.GroupNames.UnexpectedToken];
            if (unexpectedTokenGroup.Success)
            {
                if (!options.IsAnySet(TemplatedStringResolverOptions.TolerateUnexpectedTokens))
                {
                    throw new TemplatedStringResolverException(
                        $@"Error at index {match.Index}: unexpected token {unexpectedTokenGroup.Value.ToUIString()}.");
                }

                onAppend?.Invoke(unexpectedTokenGroup.Value);
                continue;
            }

            var successfulGroups = ((IEnumerable<Group>)match.Groups).Where(group => group.Success).ToArray();

            var successfulGroupsDescription = successfulGroups
                .Select(group => $@"{GetGroupName(group)} @ {group.Index}: {group.Value.ToUIString()}")
                .Distinct(StringComparer.Ordinal)
                .Join(",\x0020");

            throw new InvalidOperationException(
                $@"[Internal error] Error at index {match.Index}: unexpected regular expression match has occurred: {successfulGroupsDescription}.");
        }

        [NotNull]
        static string GetGroupName([NotNull] Group group) => $@"{nameof(Group)} {group.Name.ToUIString()}";
    }

    private static class Constants
    {
        public const string OpeningBraceChar = "{";
        public const string ClosingBraceChar = "}";

        private const RegexOptions CommonRegexOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline
            | RegexOptions.IgnorePatternWhitespace;

        private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(50);

        private static readonly string EscapedOpeningBraceChar = Regex.Escape(OpeningBraceChar.ToString(CultureInfo.InvariantCulture));
        private static readonly string EscapedClosingBraceChar = Regex.Escape(ClosingBraceChar.ToString(CultureInfo.InvariantCulture));

        private static readonly string VariableNameRegexPattern = $@"[^{EscapedOpeningBraceChar}{EscapedClosingBraceChar}]*";
        private static readonly string ValidVariableNameRegexPattern = $@"^{VariableNameRegexPattern}$";

        private static readonly string TemplateRegexPattern =
            $@"(?<{GroupNames.OpeningBrace}>{EscapedOpeningBraceChar}{EscapedOpeningBraceChar}) | (?:{
                EscapedOpeningBraceChar}(?<{GroupNames.VariableName}>{VariableNameRegexPattern}){EscapedClosingBraceChar}) | (?<{
                    GroupNames.ClosingBrace}>{EscapedClosingBraceChar}{EscapedClosingBraceChar}) | (?<{
                        GroupNames.UnexpectedToken}>(?:{EscapedOpeningBraceChar}|{EscapedClosingBraceChar}))";

        public static Regex TemplateRegex { get; } = new(TemplateRegexPattern, CommonRegexOptions, RegexTimeout);

        public static Regex ValidVariableNameRegex { get; } = new(ValidVariableNameRegexPattern, CommonRegexOptions, RegexTimeout);

        public static class GroupNames
        {
            public const string OpeningBrace = nameof(OpeningBrace);
            public const string ClosingBrace = nameof(ClosingBrace);
            public const string VariableName = nameof(VariableName);
            public const string UnexpectedToken = nameof(UnexpectedToken);
        }
    }
}