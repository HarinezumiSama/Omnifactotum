using System;
using System.Collections;

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullAndNotEmptyCollectionConstraint"/>
[Obsolete($"Use '{nameof(NotNullAndNotEmptyCollectionConstraint)}' instead.")]
public sealed class NotNullOrEmptyCollectionConstraint : LegacyTypedMemberConstraintBase<ICollection?, NotNullAndNotEmptyCollectionConstraint>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullOrEmptyCollectionConstraint"/> class.
    /// </summary>
    public NotNullOrEmptyCollectionConstraint()
        : base(new NotNullAndNotEmptyCollectionConstraint())
    {
        // Nothing to do
    }
}