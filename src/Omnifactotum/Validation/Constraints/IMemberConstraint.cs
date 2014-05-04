﻿using System;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     <para>
    ///         Represents a constraint for a type member (that is, for a field or a property).
    ///     </para>
    ///     <para>
    ///         <b>NOTES</b>:
    ///         <list type="bullet">
    ///             <item>
    ///                 Implementation must be stateless in order to be reusable during validation.
    ///             </item>
    ///             <item>
    ///                 Implementation must have public parameterless constructor.
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    public interface IMemberConstraint
    {
        #region Methods

        /// <summary>
        ///     Validates the specified value in scope of the specified context.
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
        ///     <b>null</b> if validation succeeded; or a <see cref="MemberConstraintValidationError"/> instance
        ///     describing the validation error, if validation failed.
        /// </returns>
        MemberConstraintValidationError[] Validate(
            [NotNull] ObjectValidatorContext objectValidatorContext,
            [NotNull] MemberConstraintValidationContext context,
            object value);

        #endregion
    }
}