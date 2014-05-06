using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Represents the strongly-typed constraint that ignores validation.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    public sealed class IgnoredConstraint<T> : TypedMemberConstraintBase<T>
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
        protected override void ValidateTypedValue(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            T value)
        {
            // Nothing to do
        }

        #endregion
    }
}