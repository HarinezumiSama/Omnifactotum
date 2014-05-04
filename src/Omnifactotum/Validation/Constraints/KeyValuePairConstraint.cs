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
    /// <typeparam name="TKeyConstraint">
    ///     The type specifying the key constraint.
    /// </typeparam>
    /// <typeparam name="TValueConstraint">
    ///     The type specifying the value constraint.
    /// </typeparam>
    public sealed class KeyValuePairConstraint<TKey, TValue, TKeyConstraint, TValueConstraint>
        : TypedMemberConstraintBase<KeyValuePair<TKey, TValue>>
        where TKeyConstraint : TypedMemberConstraintBase<TKey>
        where TValueConstraint : TypedMemberConstraintBase<TValue>
    {
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
        /// <returns>
        ///     <list type="bullet">
        ///         <item><b>null</b> or an empty array, if validation succeeded;</item>
        ///         <item>
        ///             or an array of <see cref="MemberConstraintValidationError"/> instances describing
        ///             validation errors, if validation failed.
        ///         </item>
        ///     </list>
        /// </returns>
        protected override MemberConstraintValidationError[] ValidateTypedValue(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            KeyValuePair<TKey, TValue> value)
        {
            var keyErrors = ValidateMember(
                objectValidatorContext,
                context,
                value,
                pair => pair.Key,
                typeof(TKeyConstraint));

            var valueErrors = ValidateMember(
                objectValidatorContext,
                context,
                value,
                pair => pair.Value,
                typeof(TValueConstraint));

            return keyErrors.AvoidNull().Concat(valueErrors.AvoidNull()).ToArray();
        }

        #endregion
    }
}