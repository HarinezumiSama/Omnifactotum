﻿using System;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint specifying that the annotated member of the nullable enumeration type <typeparamref name="TEnum"/>
///     should not be <see langword="null"/> and should have a value defined in the enumeration type <typeparamref name="TEnum"/>.
/// </summary>
/// <typeparam name="TEnum">
///     The type of the enumeration value to validate.
/// </typeparam>
public sealed class NullableEnumValueDefinedConstraint<TEnum> : TypedMemberConstraintBase<TEnum?>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, TEnum? value)
    {
        if (value is null)
        {
            AddError(memberContext, ValidationMessages.CannotBeNull);
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