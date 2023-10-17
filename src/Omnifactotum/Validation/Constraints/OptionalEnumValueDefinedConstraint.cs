using System;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint specifying that the annotated member of the nullable enumeration type <typeparamref name="TEnum"/>
///     should be either <see langword="null"/> or a value defined in the enumeration type <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">
///     The type of the enumeration value to validate.
/// </typeparam>
public sealed class OptionalEnumValueDefinedConstraint<TEnum> : TypedMemberConstraintBase<TEnum?>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, TEnum? value)
    {
        if (value is null)
        {
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