using System;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint specifying that the annotated member of the nullable enumeration type <typeparamref name="TEnum"/>
///     may be <see langword="null"/>, but otherwise should be defined in the enumeration type <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">
///     The type of the enumeration value to validate.
/// </typeparam>
public sealed class OptionalEnumValueDefinedConstraint<TEnum> : TypedMemberConstraintBase<TEnum?>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, TEnum? value)
    {
        if (value is null)
        {
            return;
        }

        if (!value.Value.IsDefined())
        {
            AddError(
                memberContext,
                AsInvariant($"The value {FormatValue(value.Value)} is not defined in the enumeration '{NonNullableValueTypeQualifiedName}'."));
        }
    }
}