﻿using System;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of the <see cref="String"/> type should not be blank. The string value is considered
    ///     blank if it is <see langword="null"/>, <see cref="string.Empty"/>, or consists only of white-space characters.
    /// </summary>
    /// <seealso cref="string.IsNullOrWhiteSpace"/>
    [Obsolete(
        nameof(NotNullOrWhiteSpaceStringConstraint) + " is deprecated. Use " + nameof(NotBlankStringConstraint) + " instead.",
        false)]
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