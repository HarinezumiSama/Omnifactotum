namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member should not be <see langword="null"/>.
/// </summary>
public sealed class NotNullConstraint : MemberConstraintBase
{
    /// <inheritdoc />
    protected override void ValidateValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        object? value)
    {
        if (value is null || ValidationFactotum.IsDefaultImmutableArray(value))
        {
            AddError(validatorContext, memberContext, ValidationMessages.CannotBeNull);
        }
    }
}