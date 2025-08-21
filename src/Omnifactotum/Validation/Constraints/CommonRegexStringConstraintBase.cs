using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     The common base class for the <see cref="RegexObject"/> based constraints.
/// </summary>
/// <seealso cref="RegexObject"/>
public abstract class CommonRegexStringConstraintBase : TypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     The default <see cref="RegexOptions"/> used in the <see cref="CommonRegexStringConstraintBase"/> constructor.
    /// </summary>
    private protected const RegexOptions CommonDefaultRegexOptions = RegexOptions.Singleline | RegexOptions.Compiled;

    /// <summary>
    ///     The default regular expression match evaluation timeout, in milliseconds.
    /// </summary>
    private protected const int CommonDefaultRegexTimeoutMilliseconds = 100;

    /// <summary>
    ///     The default regular expression match evaluation timeout.
    /// </summary>
    private protected static readonly TimeSpan CommonDefaultRegexTimeout = TimeSpan.FromMilliseconds(CommonDefaultRegexTimeoutMilliseconds);

    private readonly bool _isOptional;

    private protected CommonRegexStringConstraintBase([NotNull] Regex regex, bool isOptional)
    {
        RegexObject = regex ?? throw new ArgumentNullException(nameof(regex));
        _isOptional = isOptional;
    }

    private protected CommonRegexStringConstraintBase(
        [NotNull] string pattern,
        RegexOptions options,
        TimeSpan? timeout,
        bool isOptional)
        : this(new Regex(pattern, options, ResolveTimeout(timeout)), isOptional)
    {
        // Nothing to do
    }

    /// <summary>
    ///     Gets the regular expression pattern that the annotated member should satisfy.
    /// </summary>
    protected string Pattern => RegexObject.ToString();

    /// <summary>
    ///     Gets the <see cref="RegexOptions"/> used in the validation <see cref="Regex"/>.
    /// </summary>
    protected RegexOptions Options => RegexObject.Options;

    /// <summary>
    ///     Gets the regular expression match evaluation timeout.
    /// </summary>
    protected TimeSpan Timeout => RegexObject.MatchTimeout;

    internal Regex RegexObject { get; }

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (value is null && _isOptional)
        {
            return;
        }

        if (value is null || !RegexObject.IsMatch(value))
        {
            AddError(
                memberContext,
                new ValidationErrorDetails(
                    GetErrorDetailsText(value).EnsureNotBlank(),
                    GetErrorDetailsDescription(value).EnsureNotBlank()));
        }
    }

    /// <summary>
    ///     Gets the user-friendly text describing the validation pattern used as <see cref="ValidationErrorDetails.Text"/>.
    /// </summary>
    /// <param name="value">
    ///     The value that did not pass validation.
    /// </param>
    /// <returns>
    ///     The user-friendly text describing the validation pattern.
    /// </returns>
    [NotNull]
    protected virtual string GetErrorDetailsText(string? value) => $"The value {FormatValue(value)} does not meet the validation pattern.";

    /// <summary>
    ///     Gets the description of the validation pattern used as <see cref="ValidationErrorDetails.Description"/>.
    /// </summary>
    /// <param name="value">
    ///     The value that did not pass validation.
    /// </param>
    /// <returns>
    ///     The description of the validation pattern.
    /// </returns>
    [NotNull]
    protected virtual string GetErrorDetailsDescription(string? value)
        => $"The value {FormatValue(value)} does not match the regular expression pattern {
            RegexObject.ToString().ToUIString()} (options: {RegexObject.Options}).";

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static TimeSpan ResolveTimeout(TimeSpan? timeout)
    {
        if (timeout is not { } timeoutValue)
        {
            return CommonDefaultRegexTimeout;
        }

        if (timeoutValue == Regex.InfiniteMatchTimeout)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeout),
                timeout,
                $"The value, when not null, cannot be equal to <{nameof(Regex)}.{nameof(Regex.InfiniteMatchTimeout)}>.");
        }

        if (timeoutValue <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "The value, when not null, must be greater than zero.");
        }

        return timeoutValue;
    }
}