using System;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     The base class for attributes annotating validatable members.
/// </summary>
[CLSCompliant(false)]
public abstract class BaseValidatableMemberAttribute : Attribute, IBaseValidatableMemberAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseValidatableMemberAttribute"/> class.
    /// </summary>
    internal BaseValidatableMemberAttribute()
    {
        // Nothing to do
    }
}