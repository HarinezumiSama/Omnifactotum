using System;
using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Represents a base constraint for validating <see cref="KeyValuePair{TKey,TValue}"/> instances.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value.
    /// </typeparam>
    public abstract class KeyValuePairConstraintBase<TKey, TValue>
        : TypedMemberConstraintBase<KeyValuePair<TKey, TValue>>
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
        protected KeyValuePairConstraintBase(Type keyConstraintType, Type valueConstraintType)
        {
            KeyConstraintType = keyConstraintType.EnsureValidMemberConstraintType();
            ValueConstraintType = valueConstraintType.EnsureValidMemberConstraintType();
        }

        /// <summary>
        ///     Gets the type specifying the key constraint.
        /// </summary>
        protected Type KeyConstraintType
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the type specifying the value constraint.
        /// </summary>
        protected Type ValueConstraintType
        {
            get;
            private set;
        }

        /// <summary>
        ///     Validates the specified strongly-typed value is scope of the specified context.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        protected sealed override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            KeyValuePair<TKey, TValue> value)
        {
            ValidateMember(
                validatorContext,
                memberContext,
                value,
                pair => pair.Key,
                KeyConstraintType);

            ValidateMember(
                validatorContext,
                memberContext,
                value,
                pair => pair.Value,
                ValueConstraintType);
        }
    }
}