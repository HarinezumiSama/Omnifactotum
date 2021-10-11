using System;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of the <see cref="String"/> type should not be <see langword="null"/> or blank.
    /// </summary>
    public sealed class NotNullOrWhiteSpaceStringConstraint : TypedMemberConstraintBase<string>
    {
        /// <inheritdoc />
        protected override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                AddError(validatorContext, memberContext, ValidationMessages.StringCannotBeNullOrBlank);
            }
        }
    }
}