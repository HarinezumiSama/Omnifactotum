using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="IReadOnlyCollection{T}"/> type (or compatible)
///     or the <see cref="ICollection{T}"/> type (or compatible) may be <see langword="null"/>, but otherwise should not be empty.
/// </summary>
/// <remarks>
///     This constraint accepts a value of the <see cref="IEnumerable{T}"/> type, however at run-time the value must be compatible with
///     either <see cref="IReadOnlyCollection{T}"/> or <see cref="ICollection{T}"/>.
/// </remarks>
public sealed class OptionalNotEmptyCollectionConstraint<T> : TypedMemberConstraintBase<IEnumerable<T>?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, IEnumerable<T>? value)
    {
        switch (value)
        {
            case null or ImmutableArray<T> { IsDefault: true }:
                break;

            case IReadOnlyCollection<T> readOnlyCollection:
                if (readOnlyCollection.Count == 0)
                {
                    AddError(memberContext, ValidationMessages.CollectionCannotBeEmpty);
                }

                break;

            case ICollection<T> collection:
                if (collection.Count == 0)
                {
                    AddError(memberContext, ValidationMessages.CollectionCannotBeEmpty);
                }

                break;

            default:
                throw new InvalidOperationException($"Unexpected collection type '{value.GetType().GetFullName()}' in '{GetType().GetQualifiedName()}'.");
        }
    }
}
