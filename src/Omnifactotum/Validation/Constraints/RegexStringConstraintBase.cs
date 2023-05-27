using System;
using System.Text.RegularExpressions;
using Omnifactotum.Annotations;

#if NET7_0_OR_GREATER
using StringSyntaxAttribute = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="String"/> type should match the specified regular expression pattern.
/// </summary>
/// <seealso cref="Regex"/>
/// <seealso cref="RegexOptions"/>
public abstract class RegexStringConstraintBase : TypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     The default <see cref="RegexOptions"/> used in the <see cref="RegexStringConstraintBase"/> constructor.
    /// </summary>
    [PublicAPI]
    public const RegexOptions DefaultRegexOptions = RegexOptions.Singleline | RegexOptions.Compiled;

    /// <summary>
    ///     The default regular expression match evaluation timeout.
    /// </summary>
    [PublicAPI]
    public static readonly TimeSpan DefaultRegexTimeout = TimeSpan.FromMilliseconds(100);

    private readonly string _pattern;
    private readonly Regex _regex;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RegexStringConstraintBase" /> class using the specified pattern, options, and timeout.
    /// </summary>
    /// <param name="pattern">
    ///     The regular expression pattern that the annotated member should satisfy.
    /// </param>
    /// <param name="options">
    ///     The <see cref="RegexOptions"/> to pass to <see cref="Regex"/> used for validation.
    /// </param>
    /// <param name="timeout">
    ///     The regular expression match evaluation timeout, or <see langword="null"/> to use <see cref="DefaultRegexTimeout"/>.
    /// </param>
    /// <seealso cref="DefaultRegexTimeout"/>
    protected RegexStringConstraintBase(
#if NET7_0_OR_GREATER
        [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
        [NotNull]
        [RegexPattern]
        string pattern,
        RegexOptions options = DefaultRegexOptions,
        TimeSpan? timeout = null)
    {
        if (timeout.HasValue && timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), timeout, @"The value, when not null, must be greater than zero.");
        }

        _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        _regex = new Regex(pattern, options, timeout ?? DefaultRegexTimeout);
    }

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        string? value)
    {
        if (value is null || !_regex.IsMatch(value))
        {
            AddError(
                validatorContext,
                memberContext,
                $"The value {value.ToUIString()} does not match the regular expression pattern {_pattern.ToUIString()} (options: {_regex.Options}).");
        }
    }
}