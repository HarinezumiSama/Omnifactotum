using System.Runtime.CompilerServices;
using Omnifactotum;
using static Omnifactotum.FormattableStringFactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for nullable enumerations.
/// </summary>
public static class OmnifactotumNullableEnumExtensions
{
    /// <summary>
    ///     Creates a <see cref="System.NotImplementedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to create an exception for.
    /// </param>
    /// <returns>
    ///     A <see cref="System.NotImplementedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static NotImplementedException CreateEnumValueNotImplementedException<TEnum>(this TEnum? enumerationValue)
        where TEnum : struct, Enum
        => new(AsInvariant($@"The operation for {enumerationValue.InternalGetDescription()} is not implemented."));

    /// <summary>
    ///     Creates a <see cref="System.NotSupportedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to create an exception for.
    /// </param>
    /// <returns>
    ///     A <see cref="System.NotSupportedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static NotSupportedException CreateEnumValueNotSupportedException<TEnum>(this TEnum? enumerationValue)
        where TEnum : struct, Enum
        => new(AsInvariant($@"The operation for {enumerationValue.InternalGetDescription()} is not supported."));

    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    private static string InternalGetDescription<TEnum>(this TEnum? value)
        where TEnum : struct, Enum
        => value is { } enumerationValue
            ? $@"the enumeration value {enumerationValue.GetQualifiedName().ToUIString()}"
            : $@"the {OmnifactotumRepresentationConstants.NullValueRepresentation} value (type: {typeof(TEnum?).GetQualifiedName()})";
}