using System;

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullAndNotBlankStringConstraint"/>
[Obsolete($"Use '{nameof(NotNullAndNotBlankStringConstraint)}' instead.")]
public sealed class NotBlankStringConstraint : LegacyTypedMemberConstraintBase<string?, NotNullAndNotBlankStringConstraint>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotBlankStringConstraint"/> class.
    /// </summary>
    public NotBlankStringConstraint()
        : base(new NotNullAndNotBlankStringConstraint())
    {
        // Nothing to do
    }
}