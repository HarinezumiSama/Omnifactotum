namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member should not be <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    public class NotNullConstraint<T> : TypedMemberConstraintBase<T>
        where T : class
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
        protected sealed override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            T value)
        {
            if (value != null)
            {
                return;
            }

            AddError(validatorContext, memberContext, "The value cannot be null.");
        }
    }
}