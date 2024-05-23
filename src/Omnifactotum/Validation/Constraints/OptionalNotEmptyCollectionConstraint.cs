using System.Collections;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="ICollection"/> type may be <see langword="null"/>, but otherwise should not be empty.
/// </summary>
public sealed class OptionalNotEmptyCollectionConstraint : TypedMemberConstraintBase<ICollection?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, ICollection? value)
    {
        if (value is null || ValidationFactotum.IsDefaultImmutableArray(value))
        {
            return;
        }

        if (value.Count == 0)
        {
            AddError(memberContext, ValidationMessages.CollectionCannotBeEmpty);
        }
    }
}