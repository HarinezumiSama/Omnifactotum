namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type should not be <see langword="null"/>, should not be <see cref="string.Empty"/>,
///     and should not consist only of white-space characters.
/// </summary>
/// <seealso cref="char.IsWhiteSpace(char)"/>
/// <seealso cref="OptionalNotBlankStringConstraint"/>
public sealed class NotNullAndNotBlankStringConstraint : TypedMemberConstraintBase<string?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddError(memberContext, ValidationMessages.StringCannotBeNullOrBlank);
        }
    }
}