﻿using System.Collections;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of type <see cref="ICollection"/> should not be <see langword="null"/> or empty.
/// </summary>
public sealed class NotNullOrEmptyCollectionConstraint : TypedMemberConstraintBase<ICollection?>
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