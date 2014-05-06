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
        ///     Validates the specified value in scope of the specified memberContext.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of the validated member.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <returns>
        ///     <b>null</b> if validation succeeded; or a <see cref="MemberConstraintValidationError"/> instance
        ///     describing the validation error, if validation failed.
        /// </returns>
        public void Validate(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            object value)
        {
            #region Argument Check

            if (validatorContext == null)
            {
                throw new ArgumentNullException("validatorContext");
            }

            if (memberContext == null)
            {
                throw new ArgumentNullException("memberContext");
            }

            #endregion

            ValidateValue(validatorContext, memberContext, value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Validates the specified value is scope of the specified memberContext.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of the validated member.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        protected abstract void ValidateValue(
            [NotNull] ObjectValidatorContext validatorContext,
            [NotNull] MemberConstraintValidationContext memberContext,
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
        ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
        ///     and failure message and then adds the created error to the validator context.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of the validated member to create an error for.
        /// </param>
        /// <param name="failureMessage">
        ///     The message describing the validation error.
        /// </param>
        protected void AddError(
            [NotNull] ObjectValidatorContext validatorContext,
            [NotNull] MemberConstraintValidationContext memberContext,
            [NotNull] string failureMessage)
        {
            #region Argument Check

            if (memberContext == null)
            {
                throw new ArgumentNullException("memberContext");
            }

            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    "failureMessage");
            }

            #endregion

            var error = new MemberConstraintValidationError(memberContext, GetType(), failureMessage);
            validatorContext.Errors.Add(error);
        }

        /// <summary>
        ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
        ///     and default failure message and then adds the created error to the validator context.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of the validated member to create an error for.
        /// </param>
        protected void AddDefaultError(
            [NotNull] ObjectValidatorContext validatorContext,
            [NotNull] MemberConstraintValidationContext memberContext)
        {
            #region Argument Check

            if (memberContext == null)
            {
                throw new ArgumentNullException("memberContext");
            }

            #endregion

            var failureMessage = string.Format(
                CultureInfo.InvariantCulture,
                @"Validation of the constraint '{0}' failed.",
                GetType().GetQualifiedName());

            AddError(validatorContext, memberContext, failureMessage);
        }

        #endregion
    }
}