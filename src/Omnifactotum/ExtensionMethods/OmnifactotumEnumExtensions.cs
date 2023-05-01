using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for enumerations.
/// </summary>
public static class OmnifactotumEnumExtensions
{
    private const string EnumValueSeparator = ",\x0020";

    private static readonly string NameFormat = "{0}" + Type.Delimiter + "{1}";

    /// <summary>
    ///     Gets the name of the specified enumeration value.
    /// </summary>
    /// <param name="value">
    ///     The enumeration value to get the name of.
    /// </param>
    /// <returns>
    ///     The name of the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static string GetName<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => value.GetName(EnumNameMode.Short);

    /// <summary>
    ///     Gets the qualified name of the specified enumeration value in the following form:
    ///     <c>EnumerationName.EnumerationValueName</c>.
    /// </summary>
    /// <param name="value">
    ///     The enumeration value to get the qualified name of.
    /// </param>
    /// <returns>
    ///     The qualified name of the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static string GetQualifiedName<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => value.GetName(EnumNameMode.Qualified);

    /// <summary>
    ///     Gets the full name of the specified enumeration value in the following form:
    ///     <c>EnumerationNamespace.EnumerationName.EnumerationValueName</c>.
    /// </summary>
    /// <param name="value">
    ///     The enumeration value to get the full name of.
    /// </param>
    /// <returns>
    ///     The full name of the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static string GetFullName<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => value.GetName(EnumNameMode.Full);

    /// <summary>
    ///     Determines whether the specified enumeration value contains all the specified flags set.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumerationValue">
    ///     The enumeration value to check the flags in.
    /// </param>
    /// <param name="flags">
    ///     The combination of the bit flags to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if all bits specified in <paramref name="flags"/> are set
    ///     in <paramref name="enumerationValue"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     <para>
    ///         <typeparamref name="TEnum"/> is not an enumeration type.
    ///     </para>
    ///     <para>
    ///         -or-
    ///     </para>
    ///     <para>
    ///         The enumeration <typeparamref name="TEnum"/> is not a flag enumeration, that is,
    ///         <typeparamref name="TEnum"/> type is not marked by <see cref="System.FlagsAttribute"/>.
    ///     </para>
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsAllSet<TEnum>(this TEnum enumerationValue, TEnum flags)
        where TEnum : struct, Enum
        => IsSetInternal(enumerationValue, flags, true);

    /// <summary>
    ///     Determines whether the specified enumeration value contains any of the specified flags set.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumerationValue">
    ///     The enumeration value to check the flags in.
    /// </param>
    /// <param name="flags">
    ///     The combination of the bit flags to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if any of flags specified by the <paramref name="flags"/> parameter is set
    ///     in <paramref name="enumerationValue"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     <para>
    ///         <typeparamref name="TEnum"/> is not an enumeration type.
    ///     </para>
    ///     <para>
    ///         -or-
    ///     </para>
    ///     <para>
    ///         The enumeration <typeparamref name="TEnum"/> is not a flag enumeration, that is,
    ///         <typeparamref name="TEnum"/> type is not marked by <see cref="System.FlagsAttribute"/>.
    ///     </para>
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsAnySet<TEnum>(this TEnum enumerationValue, TEnum flags)
        where TEnum : struct, Enum
        => IsSetInternal(enumerationValue, flags, false);

    /// <summary>
    ///     Determines whether the specified enumeration value equals to one of other specified enumeration
    ///     values.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumerationValue">
    ///     The enumeration value to compare to <paramref name="otherValues"/>.
    /// </param>
    /// <param name="otherValues">
    ///     The collection of values to compare <paramref name="enumerationValue"/> to.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if any of values specified by the <paramref name="otherValues"/> equals to
    ///     <paramref name="enumerationValue"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     <typeparamref name="TEnum"/> is not an enumeration type.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="otherValues"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsOneOf<TEnum>(this TEnum enumerationValue, [NotNull] [InstantHandle] IEnumerable<TEnum> otherValues)
        where TEnum : struct, Enum
    {
        if (otherValues is null)
        {
            throw new ArgumentNullException(nameof(otherValues));
        }

        return otherValues.Any(otherValue => EqualityComparer<TEnum>.Default.Equals(enumerationValue, otherValue));
    }

    /// <summary>
    ///     Determines whether the specified enumeration value equals to one of other specified enumeration
    ///     values.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumerationValue">
    ///     The enumeration value to compare to <paramref name="otherValues"/>.
    /// </param>
    /// <param name="otherValues">
    ///     An array of values to compare <paramref name="enumerationValue"/> to.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if any of values specified by the <paramref name="otherValues"/> equals to
    ///     <paramref name="enumerationValue"/>; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     <typeparamref name="TEnum"/> is not an enumeration type.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="otherValues"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsOneOf<TEnum>(this TEnum enumerationValue, [NotNull] params TEnum[] otherValues)
        where TEnum : struct, Enum
        => IsOneOf(enumerationValue, (IEnumerable<TEnum>)otherValues);

    /// <summary>
    ///     Checks if the specified enumeration value is defined in the corresponding enumeration.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified enumeration value is defined in the corresponding enumeration;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsDefined<TEnum>(this TEnum enumerationValue)
        where TEnum : struct, Enum
        => IsDefinedInternal(enumerationValue);

    /// <summary>
    ///     Ensures that the specified enumeration value is defined in the corresponding enumeration and
    ///     if it is not, throws <see cref="System.ComponentModel.InvalidEnumArgumentException"/>.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to check.
    /// </param>
    /// <param name="enumerationValueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="enumerationValue"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified enumeration value if it is defined; otherwise, <see cref="System.ComponentModel.InvalidEnumArgumentException"/> is thrown.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    ///     <paramref name="enumerationValue"/> is not defined in the corresponding enumeration.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static TEnum EnsureDefined<TEnum>(
        this TEnum enumerationValue,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(enumerationValue))]
#endif
        string? enumerationValueExpression = null)
        where TEnum : struct, Enum
    {
        if (IsDefinedInternal(enumerationValue))
        {
            return enumerationValue;
        }

        var details = Factotum.GetDefaultCallerArgumentExpressionDetails(enumerationValueExpression);

        throw new InvalidEnumArgumentException(
            AsInvariant(
                $@"The value {enumerationValue:D} is not defined in the enumeration {enumerationValue.GetType().GetFullName().ToUIString()}.{details}"));
    }

    /// <summary>
    ///     Creates a <see cref="System.NotImplementedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to create an exception for.
    /// </param>
    /// <param name="enumerationValueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="enumerationValue"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     A <see cref="System.NotImplementedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static NotImplementedException CreateEnumValueNotImplementedException<TEnum>(
        this TEnum enumerationValue,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(enumerationValue))]
#endif
        string? enumerationValueExpression = null)
        where TEnum : struct, Enum
    {
        var details = Factotum.GetDefaultCallerArgumentExpressionDetails(enumerationValueExpression);

        return new NotImplementedException(
            AsInvariant($@"The operation for the enumeration value {enumerationValue.GetQualifiedName().ToUIString()} is not implemented.{details}"));
    }

    /// <summary>
    ///     Creates a <see cref="System.NotSupportedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </summary>
    /// <param name="enumerationValue">
    ///     The enumeration value to create an exception for.
    /// </param>
    /// <param name="enumerationValueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="enumerationValue"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     A <see cref="System.NotSupportedException"/> with the descriptive message regarding
    ///     the specified enumeration value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static NotSupportedException CreateEnumValueNotSupportedException<TEnum>(
        this TEnum enumerationValue,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(enumerationValue))]
#endif
        string? enumerationValueExpression = null)
        where TEnum : struct, Enum
    {
        var details = Factotum.GetDefaultCallerArgumentExpressionDetails(enumerationValueExpression);

        return new NotSupportedException(
            AsInvariant($@"The operation for the enumeration value {enumerationValue.GetQualifiedName().ToUIString()} is not supported.{details}"));
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static IEnumerable<TEnum> DecomposeEnumFlags<TEnum>(this TEnum enumValue)
        where TEnum : struct, Enum
    {
        var castEnumValue = enumValue.ToUlongInternal();
        var enumType = enumValue.GetType();

        if (castEnumValue == 0)
        {
            return new[] { enumValue };
        }

        var values = EnumFactotum.GetAllFlagValues<TEnum>();

        var result = new List<TEnum>(values.Length);

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var value in values)
        {
            var castValue = value.ToUlongInternal();

            //// ReSharper disable once InvertIf
            if (castValue != 0 && (castEnumValue & castValue) == castValue)
            {
                result.Add((TEnum)Enum.ToObject(enumType, castValue));
                castEnumValue &= ~castValue;
            }
        }

        if (castEnumValue != 0)
        {
            return new[] { enumValue };
        }

        return result;
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static string GetName<TEnum>(this TEnum value, EnumNameMode mode)
        where TEnum : struct, Enum
        => value.GetType().IsDefined(typeof(FlagsAttribute), false)
            ? value.DecomposeEnumFlags().Select(flag => flag.GetSingleValueName(mode)).Join(EnumValueSeparator)
            : value.GetSingleValueName(mode);

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static string GetSingleValueName<TEnum>(this TEnum value, EnumNameMode mode)
        where TEnum : struct, Enum
    {
        var enumType = value.GetType();
        var enumValueName = value.ToString();

        return mode switch
        {
            EnumNameMode.Short => enumValueName,
            EnumNameMode.Qualified => string.Format(CultureInfo.InvariantCulture, NameFormat, enumType.Name, enumValueName),
            EnumNameMode.Full => string.Format(CultureInfo.InvariantCulture, NameFormat, enumType.GetFullName(), enumValueName),
            _ => throw mode.CreateEnumValueNotImplementedException()
        };
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static bool IsSetInternal<TEnum>(this TEnum enumerationValue, TEnum flags, bool all)
        where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsDefined(typeof(FlagsAttribute), false))
        {
            throw new ArgumentException(
                AsInvariant($@"The type {enumType.GetFullName().ToUIString()} is not a bit-field enumeration."),
                nameof(TEnum));
        }

        var underlyingType = Enum.GetUnderlyingType(enumType);
        if (underlyingType == typeof(ulong))
        {
            var castFlags = Convert.ToUInt64(flags);
            var conjunctValue = Convert.ToUInt64(enumerationValue) & castFlags;
            return all ? conjunctValue == castFlags : conjunctValue != 0;
        }
        else
        {
            var castFlags = Convert.ToInt64(flags);
            var conjunctValue = Convert.ToInt64(enumerationValue) & castFlags;
            return all ? conjunctValue == castFlags : conjunctValue != 0;
        }
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    private static bool IsDefinedInternal<TEnum>(TEnum enumerationValue)
        where TEnum : struct, Enum
#if NET5_0_OR_GREATER
        => Enum.IsDefined(enumerationValue);
#else
        => Enum.IsDefined(enumerationValue.GetType(), enumerationValue);
#endif

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static ulong ToUlongInternal<TEnum>(this TEnum value)
        where TEnum : struct, Enum
    {
        var typeCode = Convert.GetTypeCode(value);

        switch (typeCode)
        {
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.Byte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return Convert.ToUInt64(value, CultureInfo.InvariantCulture);

            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                return (ulong)Convert.ToInt64(value, CultureInfo.InvariantCulture);

            case TypeCode.Empty:
            case TypeCode.Object:
            case TypeCode.DBNull:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.DateTime:
            case TypeCode.String:
                throw typeCode.CreateEnumValueNotSupportedException();

            default:
                throw typeCode.CreateEnumValueNotImplementedException();
        }
    }

    private enum EnumNameMode
    {
        Short,
        Qualified,
        Full
    }
}