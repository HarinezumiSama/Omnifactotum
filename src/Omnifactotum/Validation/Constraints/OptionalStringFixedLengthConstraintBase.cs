using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>,
///     but otherwise it should have the given fixed <see cref="string.Length"/>.
/// </summary>
/// <seealso cref="NotNullStringFixedLengthConstraintBase"/>
public abstract class OptionalStringFixedLengthConstraintBase : SimpleTypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalStringFixedLengthConstraintBase"/> class.
    /// </summary>
    /// <param name="length">
    ///     The allowed fixed length of a <see cref="string"/> value.
    /// </param>
    protected OptionalStringFixedLengthConstraintBase(int length)
        => Length = ValidationFactotum.StringLengthConstraint.ValidateLength(length, nameof(length));

    /// <summary>
    ///     Gets the allowed fixed length of a <see cref="string"/> value.
    /// </summary>
    protected int Length { get; }

    /// <inheritdoc />
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is null || value.Length == Length;

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value may be null, but otherwise its length must be exactly {Length}.{actualValueDetails}");
    }
}