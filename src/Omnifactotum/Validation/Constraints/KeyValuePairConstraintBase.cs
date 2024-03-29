﻿using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint for validating <see cref="KeyValuePair{TKey,TValue}"/> instances.
/// </summary>
/// <typeparam name="TKey">
///     The type of the key.
/// </typeparam>
/// <typeparam name="TValue">
///     The type of the value.
/// </typeparam>
public abstract class KeyValuePairConstraintBase<TKey, TValue> : TypedMemberConstraintBase<KeyValuePair<TKey, TValue>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyValuePairConstraintBase{TKey,TValue}" /> class.
    /// </summary>
    /// <param name="keyConstraintType">
    ///     The type specifying the key constraint.
    /// </param>
    /// <param name="valueConstraintType">
    ///     The type specifying the value constraint.
    /// </param>
    protected KeyValuePairConstraintBase([NotNull] Type keyConstraintType, [NotNull] Type valueConstraintType)
    {
        KeyConstraintType = keyConstraintType.ValidateAndRegisterMemberConstraintType();
        ValueConstraintType = valueConstraintType.ValidateAndRegisterMemberConstraintType();
    }

    /// <summary>
    ///     Gets the type specifying the key constraint.
    /// </summary>
    [NotNull]
    protected Type KeyConstraintType { get; }

    /// <summary>
    ///     Gets the type specifying the value constraint.
    /// </summary>
    [NotNull]
    protected Type ValueConstraintType { get; }

    /// <summary>
    ///     Validates the specified strongly-typed value is scope of the specified context.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of validation.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, KeyValuePair<TKey, TValue> value)
    {
        ValidateMember(memberContext, value, pair => pair.Key, KeyConstraintType);
        ValidateMember(memberContext, value, pair => pair.Value, ValueConstraintType);
    }
}