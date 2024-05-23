using System;

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullWebUrlConstraint"/>
/// <seealso cref="NotNullWebUrlConstraint"/>
[Obsolete($"Use '{nameof(NotNullWebUrlConstraint)}' instead.")]
public sealed class WebUrlConstraint : LegacyTypedMemberConstraintBase<string?, NotNullWebUrlConstraint>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WebUrlConstraint" /> class.
    /// </summary>
    public WebUrlConstraint()
        : base(new NotNullWebUrlConstraint())
    {
        // Nothing to do
    }
}