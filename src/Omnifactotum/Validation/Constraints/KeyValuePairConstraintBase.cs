using System;
using System.Collections.Generic;
using System.Linq;

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
        #region Constructors

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
            this.KeyConstraintType = keyConstraintType.EnsureValidMemberConstraintType();
            this.ValueConstraintType = valueConstraintType.EnsureValidMemberConstraintType();
        }

        #endregion

        #region Protected Properties

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

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Validates the specified strongly-typed value is scope of the specified context.
        /// </summary>
        /// <param name="objectValidatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        protected override sealed void ValidateTypedValue(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            KeyValuePair<TKey, TValue> value)
        {
            ValidateMember(
                objectValidatorContext,
                context,
                value,
                pair => pair.Key,
                this.KeyConstraintType);

            ValidateMember(
                objectValidatorContext,
                context,
                value,
                pair => pair.Value,
                this.ValueConstraintType);
        }

        #endregion
    }
}