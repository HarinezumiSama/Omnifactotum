using System;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of a nullable enumeration type should have a value defined in this nullable enumeration type.
/// </summary>
/// <typeparam name="TEnum">
///     The type of the enumeration value to validate.
/// </typeparam>
public sealed class NullableEnumValueDefinedConstraint<TEnum> : TypedMemberConstraintBase<TEnum?>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, TEnum? value)
    {
        if (value is null)
        {
            AddError(validatorContext, memberContext, ValidationMessages.CannotBeNull);
            return;
        }

        if (!value.Value.IsDefined())
        {
            AddError(
                validatorContext,
                memberContext,
                AsInvariant($@"The value {FormatValue(value.Value)} is not defined in the enumeration {typeof(TEnum).GetFullName().ToUIString()}."));
        }
    }
}