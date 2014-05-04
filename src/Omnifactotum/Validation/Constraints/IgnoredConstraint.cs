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
            T value)
        {
            return null;
        }

        #endregion
    }
}