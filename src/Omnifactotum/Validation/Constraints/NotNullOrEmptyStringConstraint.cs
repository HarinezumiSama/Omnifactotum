using System;

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullAndNotEmptyStringConstraint"/>
[Obsolete($"Use '{nameof(NotNullAndNotEmptyStringConstraint)}' instead.")]
public sealed class NotNullOrEmptyStringConstraint : LegacyTypedMemberConstraintBase<string?, NotNullAndNotEmptyStringConstraint>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullOrEmptyStringConstraint"/> class.
    /// </summary>
    public NotNullOrEmptyStringConstraint()
        : base(new NotNullAndNotEmptyStringConstraint())
    {
        // Nothing to do
    }
}