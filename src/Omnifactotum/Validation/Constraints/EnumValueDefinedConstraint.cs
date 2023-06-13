using System;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of an enumeration type should have a value defined in this enumeration type.
/// </summary>
/// <typeparam name="TEnum">
///     The type of the enumeration value to validate.
/// </typeparam>
public sealed class EnumValueDefinedConstraint<TEnum> : TypedMemberConstraintBase<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, TEnum value)
    {
        if (!value.IsDefined())
        {
            AddError(
                validatorContext,
                memberContext,
                AsInvariant($@"The value {FormatValue(value)} is not defined in the enumeration {typeof(TEnum).GetFullName().ToUIString()}."));
        }
    }
}