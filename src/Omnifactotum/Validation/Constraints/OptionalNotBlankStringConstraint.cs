namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>,
///     but otherwise should not be <see cref="string.Empty"/> and should not consist only of white-space characters.
/// </summary>
/// <seealso cref="char.IsWhiteSpace(char)"/>
/// <seealso cref="NotNullAndNotBlankStringConstraint"/>
public sealed class OptionalNotBlankStringConstraint : TypedMemberConstraintBase<string?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            AddError(memberContext, ValidationMessages.StringCannotBeBlank);
        }
    }
}