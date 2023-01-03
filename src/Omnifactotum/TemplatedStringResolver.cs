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
    ///     Initializes a new instance of the <see cref="TemplatedStringResolver"/> class using the specified template variables.
    /// </summary>
    /// <param name="templateVariables">
    ///     The template variables. See <see cref="TemplateVariables"/> for more details.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="templateVariables"/> is <see langword="null"/>.
    /// </exception>
    public TemplatedStringResolver([NotNull] IReadOnlyDictionary<string, string> templateVariables)
    {
        if (templateVariables is null)
        {
            throw new ArgumentNullException(nameof(templateVariables));
        }

        TemplateVariables = templateVariables.ToImmutableDictionary(StringComparer.Ordinal);

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
    ///     Gets the dictionary having variable names as its keys and corresponding variable values as its values. The variable names are case-sensitive.
    /// </summary>
    [NotNull]
    public ImmutableDictionary<string, string> TemplateVariables { get; }

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
    ///     An error in the templated string has occurred.
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

        var index = 0;
        while (index < templatedString.Length)
        {
            var match = Constants.TemplateRegex.Match(templatedString, index);
            if (!match.Success)
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                resultBuilder.Append(templatedString.AsSpan(index));
#else
                resultBuilder.Append(templatedString.Substring(index));
#endif
                break;
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            resultBuilder.Append(templatedString.AsSpan(index, match.Index - index));
#else
            resultBuilder.Append(templatedString.Substring(index, match.Index - index));
#endif
            index = match.Index + match.Length;

            var openingBraceGroup = match.Groups[Constants.GroupNames.OpeningBrace];
            if (openingBraceGroup.Success)
            {
                resultBuilder.Append(Constants.OpeningBraceChar);
                continue;
            }

            var closingBraceGroup = match.Groups[Constants.GroupNames.ClosingBrace];
            if (closingBraceGroup.Success)
            {
                resultBuilder.Append(Constants.ClosingBraceChar);
                continue;
            }

            var variableNameGroup = match.Groups[Constants.GroupNames.VariableName];
            if (variableNameGroup.Success)
            {
                var variableName = variableNameGroup.Value;
                if (!TemplateVariables.TryGetValue(variableName, out var variableValue))
                {
                    if (options.IsAnySet(TemplatedStringResolverOptions.TolerateUndefinedVariables))
                    {
                        continue;
                    }

                    throw new TemplatedStringResolverException(
                        $@"Error at index {match.Index}: the injected variable {variableName.ToUIString()} is not defined.");
                }

                resultBuilder.Append(variableValue);
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

                resultBuilder.Append(unexpectedTokenGroup.Value);
                continue;
            }

            var successfulGroups = match
                .Groups
#if !NETSTANDARD2_1_OR_GREATER
                .Cast<Group>()
#endif
                .Where(group => group.Success)
                .ToArray();

            var successfulGroupsDescription = successfulGroups
                .Select(group => $@"{GetGroupName(group)} @ {group.Index}: {group.Value.ToUIString()}")
                .Distinct(StringComparer.Ordinal)
                .Join(",\x0020");

            throw new InvalidOperationException(
                $@"[Internal error] Error at index {match.Index}: unexpected regular expression match has occurred: {successfulGroupsDescription}.");
        }

        return resultBuilder.ToString();

        //// ReSharper disable once UnusedParameter.Local :: Local contract
        [NotNull]
        static string GetGroupName([NotNull] Group group)
#if NETSTANDARD2_0
            => nameof(Group);
#else
            => $@"{nameof(Group)} {group.Name.ToUIString()}";
#endif
    }

    private static class Constants
    {
        public const char OpeningBraceChar = '{';
        public const char ClosingBraceChar = '}';

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