using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of type <see cref="String"/> should not be <b>null</b> or empty.
    /// </summary>
    public sealed class NotNullOrEmptyStringConstraint : TypedMemberConstraintBase<string>
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
            string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return null;
            }

            return CreateError(context, "The value must not be null or an empty string.").AsArray();
        }

        #endregion
    }
}