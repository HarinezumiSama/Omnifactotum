#if NET7_0_OR_GREATER
using System;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy
//// ReSharper disable RedundantAttributeUsageProperty

namespace Omnifactotum.Validation.Annotations;

/// <summary>
///     Specifies how the annotated member is validated.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
[CLSCompliant(false)]
public sealed class MemberConstraintAttribute<TMemberConstraint> : BaseMemberConstraintAttribute, IMemberConstraintAttribute
    where TMemberConstraint : IMemberConstraint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MemberConstraintAttribute{TMemberConstraint}"/> class.
    /// </summary>
    public MemberConstraintAttribute()
        : base(typeof(TMemberConstraint))
    {
        // Nothing to do
    }
}
#endif