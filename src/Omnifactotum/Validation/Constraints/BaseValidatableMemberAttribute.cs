using System;

namespace Omnifactotum.Validation.Constraints
{
    // As per my understanding, this warning does not make sense for an abstract attribute class ->
    // -> and therefore it can be turned off here
#pragma warning disable 3015

    /// <summary>
    ///     The base class for attributes annotating validatable members.
    /// </summary>
    public abstract class BaseValidatableMemberAttribute : Attribute
#pragma warning restore 3015
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseValidatableMemberAttribute"/> class.
        /// </summary>
        internal BaseValidatableMemberAttribute()
        {
            // Nothing to do
        }
    }
}