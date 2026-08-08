using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>,
///     but otherwise it should have the given maximum <see cref="string.Length"/>.
/// </summary>
/// <seealso cref="NotNullStringMaxLengthConstraintBase"/>
public abstract class OptionalStringMaxLengthConstraintBase : SimpleTypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalStringMaxLengthConstraintBase"/> class.
    /// </summary>
    /// <param name="maxLength">
    ///     The maximum allowed length of a <see cref="string"/> value.
    /// </param>
    protected OptionalStringMaxLengthConstraintBase(int maxLength)
        => MaxLength = ValidationFactotum.StringLengthConstraint.ValidateLength(maxLength, nameof(maxLength));

    /// <summary>
    ///     Gets the maximum allowed length of a <see cref="string"/> value.
    /// </summary>
    protected int MaxLength { get; }

    /// <inheritdoc />
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is null || value.Length <= MaxLength;

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value may be null, but otherwise its length must be at most {MaxLength}.{actualValueDetails}");
    }
}