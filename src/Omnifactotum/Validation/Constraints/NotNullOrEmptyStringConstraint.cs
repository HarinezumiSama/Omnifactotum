using System;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of type <see cref="String"/> should not be <c>null</c> or empty.
    /// </summary>
    public sealed class NotNullOrEmptyStringConstraint : TypedMemberConstraintBase<string>
    {
        /// <summary>
        ///     Validates the specified strongly-typed value is scope of the specified context.
        /// </summary>
        /// <param name="validatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="memberContext">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        protected override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return;
            }

            AddError(validatorContext, memberContext, "The value must not be null or an empty string.");
        }
    }
}