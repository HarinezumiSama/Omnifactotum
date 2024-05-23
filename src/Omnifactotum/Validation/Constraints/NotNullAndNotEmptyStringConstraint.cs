namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type should not be <see langword="null"/> and should not be empty.
/// </summary>
public sealed class NotNullAndNotEmptyStringConstraint : TypedMemberConstraintBase<string?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            AddError(memberContext, ValidationMessages.StringCannotBeNullOrEmpty);
        }
    }
}