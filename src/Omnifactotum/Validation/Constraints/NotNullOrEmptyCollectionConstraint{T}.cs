using System;
using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullAndNotEmptyCollectionConstraint{T}"/>
[Obsolete($"Use '{nameof(NotNullAndNotEmptyCollectionConstraint<T>)}<T>' instead.")]
public sealed class NotNullOrEmptyCollectionConstraint<T> : LegacyTypedMemberConstraintBase<IEnumerable<T>?, NotNullAndNotEmptyCollectionConstraint<T>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullOrEmptyCollectionConstraint"/> class.
    /// </summary>
    public NotNullOrEmptyCollectionConstraint()
        : base(new NotNullAndNotEmptyCollectionConstraint<T>())
    {
        // Nothing to do
    }
}