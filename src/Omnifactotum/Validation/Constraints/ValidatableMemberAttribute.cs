using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     The base class for attributes annotating validatable members.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValidatableMemberAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValidatableMemberAttribute"/> class.
        /// </summary>
        internal ValidatableMemberAttribute()
        {
            // Nothing to do
        } 

        #endregion
    }
}