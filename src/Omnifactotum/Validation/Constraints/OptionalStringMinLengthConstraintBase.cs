using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>,
///     but otherwise it should have the given minimum <see cref="string.Length"/>.
/// </summary>
/// <seealso cref="NotNullStringMinLengthConstraintBase"/>
public abstract class OptionalStringMinLengthConstraintBase : SimpleTypedMemberConstraintBase<string?>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalStringMinLengthConstraintBase"/> class.
    /// </summary>
    /// <param name="minLength">
    ///     The minimum allowed length of a <see cref="string"/> value.
    /// </param>
    protected OptionalStringMinLengthConstraintBase(int minLength)
        => MinLength = ValidationFactotum.StringLengthConstraint.ValidateLength(minLength, nameof(minLength));

    /// <summary>
    ///     Gets the minimum allowed length of a <see cref="string"/> value.
    /// </summary>
    protected int MinLength { get; }

    /// <inheritdoc />
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is null || value.Length >= MinLength;

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value may be null, but otherwise its length must be at least {MinLength}.{actualValueDetails}");
    }
}