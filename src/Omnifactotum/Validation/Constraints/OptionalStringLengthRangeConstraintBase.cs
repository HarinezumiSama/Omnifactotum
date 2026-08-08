using System.Diagnostics.CodeAnalysis;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>,
///     but otherwise its length should be within the specified inclusive range.
/// </summary>
/// <seealso cref="NotNullStringLengthRangeConstraintBase"/>
public abstract class OptionalStringLengthRangeConstraintBase : SimpleTypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalStringLengthRangeConstraintBase"/> class.
    /// </summary>
    /// <param name="minLength">
    ///     The minimum allowed length of a <see cref="string"/> value.
    /// </param>
    /// <param name="maxLength">
    ///     The maximum allowed length of a <see cref="string"/> value.
    /// </param>
    protected OptionalStringLengthRangeConstraintBase(int minLength, int maxLength)
    {
        ValidationFactotum.StringLengthConstraint.ValidateMinMaxLengths(minLength, nameof(minLength), maxLength, nameof(maxLength));

        MinLength = minLength;
        MaxLength = maxLength;
    }

    /// <summary>
    ///     Gets the minimum allowed length of a <see cref="string"/> value.
    /// </summary>
    protected int MinLength { get; }

    /// <summary>
    ///     Gets the maximum allowed length of a <see cref="string"/> value.
    /// </summary>
    protected int MaxLength { get; }

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "ArrangeRedundantParentheses")]
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is null || (value.Length >= MinLength && value.Length <= MaxLength);

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value may be null, but otherwise its length must be at least {MinLength} and at most {MaxLength}.{actualValueDetails}");
    }
}