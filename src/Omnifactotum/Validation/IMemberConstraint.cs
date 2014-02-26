using System;
using System.Linq;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents a constraint for a type member (that is, for a field or a property).
    ///     NOTE to implementers: implementation has to be stateless.
    /// </summary>
    public interface IMemberConstraint
    {
        #region Methods

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
        MemberConstraintValidationError Validate(MemberConstraintValidationContext context, object value);

        #endregion
    }
}