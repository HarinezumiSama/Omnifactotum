namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>, but otherwise should not be empty.
/// </summary>
public sealed class OptionalNotEmptyStringConstraint : TypedMemberConstraintBase<string?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (value is { Length: 0 })
        {
            AddError(memberContext, ValidationMessages.StringCannotBeEmpty);
        }
    }
}