using System.Collections.Generic;
using System.Collections.Immutable;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of type <see cref="ICollection{T}"/> should not be <see langword="null"/> or empty.
/// </summary>
public sealed class NotNullOrEmptyCollectionConstraint<T> : TypedMemberConstraintBase<ICollection<T>?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, ICollection<T>? value)
    {
        if (value is null or ImmutableArray<T> { IsDefault: true })
        {
            AddError(memberContext, ValidationMessages.CannotBeNull);
        }
        else if (value.Count == 0)
        {
            AddError(memberContext, ValidationMessages.CollectionCannotBeEmpty);
        }
    }
}