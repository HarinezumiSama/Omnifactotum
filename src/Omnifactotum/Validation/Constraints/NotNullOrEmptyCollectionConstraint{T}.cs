using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Specifies that the annotated member of type <see cref="ICollection{T}"/> should not be <see langword="null"/> or empty.
    /// </summary>
    public sealed class NotNullOrEmptyCollectionConstraint<T> : TypedMemberConstraintBase<ICollection<T>?>
    {
        /// <inheritdoc />
        protected override void ValidateTypedValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            ICollection<T>? value)
        {
            if (value is null)
            {
                AddError(validatorContext, memberContext, ValidationMessages.CannotBeNull);
            }
            else if (value.Count == 0)
            {
                AddError(validatorContext, memberContext, ValidationMessages.CollectionCannotBeEmpty);
            }
        }
    }
}