using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies how the annotated member item is validated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class MemberItemConstraintAttribute : BaseMemberConstraintAttribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberItemConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="MemberItemConstraintAttribute"/> attribute. The type must
        ///     have parameterless constructor.
        /// </param>
        public MemberItemConstraintAttribute(Type constraintType)
            : base(constraintType)
        {
            // Nothing to do
        }

        #endregion
    }
}