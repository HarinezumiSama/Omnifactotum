using System;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     <para>Represents the basic implementation of the <see cref="IMemberConstraint"/> interface.</para>
    ///     <para><b>NOTE to implementers</b>: implementation has to be stateless.</para>
    /// </summary>
    public abstract class MemberConstraintBase : IMemberConstraint
    {
        /// <inheritdoc />
        public void Validate(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, object? value)
        {
            if (validatorContext is null)
            {
                throw new ArgumentNullException(nameof(validatorContext));
            }

            if (memberContext is null)
            {
                throw new ArgumentNullException(nameof(memberContext));
            }

            ValidateValue(validatorContext, memberContext, value);
        }

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
            [CanBeNull] object? value);

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
        protected TTarget CastTo<TTarget>([CanBeNull] object? value)
        {
            var targetType = typeof(TTarget);

            return value switch
            {
                TTarget target => target,

                null => !targetType.IsValueType || targetType.IsNullable()
                    ? (TTarget)value!
                    : throw new InvalidOperationException(
                        AsInvariant(
                            $@"The null value is not compatible with the type {targetType.GetFullName().ToUIString()} expected by the constraint {
                                GetType().GetQualifiedName().ToUIString()}.")),

                _ => throw new InvalidOperationException(
                    AsInvariant(
                        $@"The type of the value {value.GetTypeSafely().GetFullName().ToUIString()} is not compatible with the type {
                            targetType.GetFullName().ToUIString()} expected by the constraint {GetType().GetQualifiedName().ToUIString()}."))
            };
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
            if (memberContext is null)
            {
                throw new ArgumentNullException(nameof(memberContext));
            }

            if (string.IsNullOrWhiteSpace(failureMessage))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    nameof(failureMessage));
            }

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
            if (memberContext is null)
            {
                throw new ArgumentNullException(nameof(memberContext));
            }

            var failureMessage = AsInvariant($@"Validation of the constraint {GetType().GetQualifiedName().ToUIString()} failed.");
            AddError(validatorContext, memberContext, failureMessage);
        }
    }
}