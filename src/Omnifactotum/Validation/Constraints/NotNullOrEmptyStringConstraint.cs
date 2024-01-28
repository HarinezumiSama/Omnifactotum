using System;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="String"/> type should not be <see langword="null"/> or empty.
/// </summary>
public sealed class NotNullOrEmptyStringConstraint : TypedMemberConstraintBase<string?>
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