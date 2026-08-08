using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type should not be <see langword="null"/>
///     and it should have the given maximum <see cref="string.Length"/>.
/// </summary>
/// <seealso cref="OptionalStringMaxLengthConstraintBase"/>
public abstract class NotNullStringMaxLengthConstraintBase : SimpleTypedMemberConstraintBase<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullStringMaxLengthConstraintBase"/> class.
    /// </summary>
    /// <param name="maxLength">
    ///     The maximum allowed length of a <see cref="string"/> value.
    /// </param>
    protected NotNullStringMaxLengthConstraintBase(int maxLength)
        => MaxLength = ValidationFactotum.StringLengthConstraint.ValidateLength(maxLength, nameof(maxLength));

    /// <summary>
    ///     Gets the maximum allowed length of a <see cref="string"/> value.
    /// </summary>
    protected int MaxLength { get; }

    /// <inheritdoc />
    protected sealed override bool IsValid(MemberConstraintValidationContext memberContext, string? value)
        => value is not null && value.Length <= MaxLength;

    /// <inheritdoc />
    protected override ValidationErrorDetails CreateValidationErrorDetails(MemberConstraintValidationContext memberContext, string? value)
    {
        var actualValueDetails = ValidationFactotum.StringLengthConstraint.GetActualValueDetails(value);
        return AsInvariant($"The string value must not be null, and its length must be at most {MaxLength}.{actualValueDetails}");
    }
}