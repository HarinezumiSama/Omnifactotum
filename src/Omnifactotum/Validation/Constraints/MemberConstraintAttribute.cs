using System;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies how the annotated member is validated.
    /// </summary>
    //// ReSharper disable once RedundantAttributeUsageProperty - Just making sure
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class MemberConstraintAttribute : BaseMemberConstraintAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="MemberConstraintAttribute"/> attribute. The type must
        ///     have parameterless constructor.
        /// </param>
        public MemberConstraintAttribute(Type constraintType)
            : base(constraintType)
        {
            // Nothing to do
        }
    }
}