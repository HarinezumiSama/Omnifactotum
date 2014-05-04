using System;
using System.Globalization;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     <para>Represents the basic implementation of the <see cref="IMemberConstraint"/> interface.</para>
    ///     <para><b>NOTE to implementers</b>: implementation has to be stateless.</para>
    /// </summary>
    public abstract class MemberConstraintBase : IMemberConstraint
    {
        #region IMemberConstraint Members

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
        public MemberConstraintValidationError[] Validate(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            object value)
        {
            #region Argument Check

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            #endregion

            return ValidateValue(objectValidatorContext, context, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Validates the specified value is scope of the specified context.
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
        protected abstract MemberConstraintValidationError[] ValidateValue(
            [NotNull] ObjectValidatorContext objectValidatorContext,
            [NotNull] MemberConstraintValidationContext context,
            object value);

        /// <summary>
        ///     Tries to cast the specified value to the specified target type and
        ///     if the value is not compatible with the target type, throws an exception with the detailed description.
        /// </summary>
        /// <typeparam name="TTarget">
        ///     The type to cast to.
        /// </typeparam>
        /// <param name="value">
        ///     The value to cast.
        /// </param>
        /// <returns>
        ///     The value cast to the specified target type.
        /// </returns>
        protected TTarget CastTo<TTarget>(object value)
        {
            var targetType = typeof(TTarget);
            if (value is TTarget || (!targetType.IsValueType && value == null))
            {
                return (TTarget)value;
            }

            var message = string.Format(
                CultureInfo.InvariantCulture,
                "The type of the value '{0}' is not compatible with the type '{1}' expected by the constraint '{2}'.",
                value.GetTypeSafely().GetFullName(),
                targetType.GetFullName(),
                GetType().GetQualifiedName());

            throw new InvalidOperationException(message);
        }

        /// <summary>
        ///     Creates a <see cref="MemberConstraintValidationError"/> instance using the specified arguments.
        /// </summary>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <param name="failureMessage">
        ///     The error message.
        /// </param>
        /// <returns>
        ///     A created <see cref="MemberConstraintValidationError"/> instance.
        /// </returns>
        protected MemberConstraintValidationError CreateError(
            [NotNull] MemberConstraintValidationContext context,
            [NotNull] string failureMessage)
        {
            #region Argument Check

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    "failureMessage");
            }

            #endregion

            return new MemberConstraintValidationError(context, GetType(), failureMessage);
        }

        /// <summary>
        ///     Creates a <see cref="MemberConstraintValidationError"/> instance using the specified arguments.
        /// </summary>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <returns>
        ///     A created <see cref="MemberConstraintValidationError"/> instance.
        /// </returns>
        protected MemberConstraintValidationError CreateDefaultError(
            [NotNull] MemberConstraintValidationContext context)
        {
            #region Argument Check

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            #endregion

            var failureMessage = string.Format(
                CultureInfo.InvariantCulture,
                @"Validation of the constraint '{0}' failed.",
                GetType().GetQualifiedName());

            return CreateError(context, failureMessage);
        }

        #endregion
    }
}