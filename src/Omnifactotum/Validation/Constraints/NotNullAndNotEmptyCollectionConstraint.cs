using System.Collections;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="ICollection"/> type (or compatible) should not be <see langword="null"/> and should not be empty.
/// </summary>
public sealed class NotNullAndNotEmptyCollectionConstraint : TypedMemberConstraintBase<ICollection?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, ICollection? value)
    {
        if (value is null || ValidationFactotum.IsDefaultImmutableArray(value))
        {
            AddError(memberContext, ValidationMessages.CannotBeNull);
        }
        else if (value.Count == 0)
        {
            AddError(memberContext, ValidationMessages.CollectionCannotBeEmpty);
        }
    }
}