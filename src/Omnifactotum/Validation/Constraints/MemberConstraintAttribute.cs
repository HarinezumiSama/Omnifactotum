#nullable enable

using System;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy
//// ReSharper disable RedundantAttributeUsageProperty

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies how the annotated member is validated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    [CLSCompliant(false)]
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
        public MemberConstraintAttribute([NotNull] Type constraintType)
            : base(constraintType)
        {
            // Nothing to do
        }
    }
}