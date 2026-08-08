using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type should not be <see langword="null"/>
///     and it should have the given fixed <see cref="string.Length"/>.
/// </summary>
/// <seealso cref="OptionalStringFixedLengthConstraintBase"/>
public abstract class NotNullStringFixedLengthConstraintBase : SimpleTypedMemberConstraintBase<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullStringFixedLengthConstraintBase"/> class.
    /// </summary>
    /// <param name="length">
    ///     The allowed fixed length of a <see cref="string"/> value.
    /// </param>
    protected NotNullStringFixedLengthConstraintBase(int length)
        => Length = ValidationFactotum.StringLengthConstraint.ValidateLength(length, nameof(length));

    /// <summary>
    ///     Gets the allowed fixed length of a <see cref="string"/> value.
    /// </summary>
    protected int Length { get; }

    /// <inheritdoc />
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is not null && value.Length == Length;

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value must not be null, and its length must be exactly {Length}.{actualValueDetails}");
    }
}