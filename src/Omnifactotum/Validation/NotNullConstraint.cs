using System;
using System.Linq;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Specifies that the annotated member should not be <b>null</b>.
    /// </summary>
    public sealed class NotNullConstraint : MemberConstraintBase
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
            if (value != null)
            {
                return null;
            }

            return CreateError(context, "The value cannot be null.");
        }

        #endregion
    }
}