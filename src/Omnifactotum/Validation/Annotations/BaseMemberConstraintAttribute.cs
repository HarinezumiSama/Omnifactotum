﻿using System;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Annotations;

/// <summary>
///     The base class for member constraint attributes.
/// </summary>
[CLSCompliant(false)]
public abstract class BaseMemberConstraintAttribute : BaseValidatableMemberAttribute, IBaseMemberConstraintAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseMemberConstraintAttribute"/> class.
    /// </summary>
    /// <param name="constraintType">
    ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
    ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute. The type must
    ///     have parameterless constructor.
    /// </param>
    private protected BaseMemberConstraintAttribute([NotNull] Type constraintType)
        => ConstraintType = constraintType.ValidateAndRegisterMemberConstraintType();

    /// <summary>
    ///     Gets the type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
    ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute.
    /// </summary>
    [NotNull]
    public Type ConstraintType { get; }

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
    /// </returns>
    public override string ToString()
        => AsInvariant($"{{ {GetType().GetQualifiedName()}: {nameof(ConstraintType)} = {ConstraintType.GetQualifiedName().ToUIString()} }}");
}