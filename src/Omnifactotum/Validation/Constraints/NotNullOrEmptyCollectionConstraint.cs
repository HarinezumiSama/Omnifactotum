using System.Collections;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of type <see cref="ICollection"/> should not be <see langword="null"/> or empty.
/// </summary>
public sealed class NotNullOrEmptyCollectionConstraint : TypedMemberConstraintBase<ICollection?>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        ICollection? value)
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