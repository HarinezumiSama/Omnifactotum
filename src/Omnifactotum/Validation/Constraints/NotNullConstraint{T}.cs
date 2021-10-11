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
        /// <inheritdoc />
        protected sealed override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            T value)
        {
            if (value is null)
            {
                AddError(validatorContext, memberContext, ValidationMessages.CannotBeNull);
            }
        }
    }
}