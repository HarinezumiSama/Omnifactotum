using System;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of type <see cref="String"/> should not be <b>null</b> or empty.
    /// </summary>
    public sealed class NotNullOrEmptyStringConstraint : MemberConstraintBase
    {
        #region Protected Methods

        /// <summary>
        ///     Validates the specified value is scope of the specified context.
        /// </summary>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <returns>
        ///     <b>null</b> if validation succeeded; or a <see cref="MemberConstraintValidationError"/> instance
        ///     describing the validation error, if validation failed.
        /// </returns>
        protected override MemberConstraintValidationError ValidateInternal(
            MemberConstraintValidationContext context,
            object value)
        {
            var stringValue = CastTo<string>(value);
            if (!string.IsNullOrEmpty(stringValue))
            {
                return null;
            }

            return CreateError(context, "The value must not be null or an empty string.");
        }

        #endregion
    }
}