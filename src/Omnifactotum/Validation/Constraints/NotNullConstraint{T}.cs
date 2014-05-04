using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member should not be <b>null</b>.
    /// </summary>
    public class NotNullConstraint<T> : TypedMemberConstraintBase<T>
        where T : class
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
        protected sealed override MemberConstraintValidationError[] ValidateTypedValue(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            T value)
        {
            return value == null ? CreateError(context, "The value cannot be null.").AsArray() : null;
        }

        #endregion
    }
}