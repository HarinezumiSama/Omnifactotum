using System;
using System.Collections.Generic;
using System.Linq;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

namespace Omnifactotum;

/// <summary>
///     Provides utility methods for enumerations.
/// </summary>
public static class EnumFactotum
{
    /// <summary>
    ///     Gets the value of the enumeration of the specified type
    ///     by the string representation of an enumeration value.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumValueName">
    ///     The string representation of an enumeration value.
    /// </param>
    /// <param name="ignoreCase">
    ///     A <see cref="System.Boolean"/> value that indicates whether to ignore the letter case.
    /// </param>
    /// <returns>
    ///     A strongly-typed value of the enumeration.
    /// </returns>
    public static TEnum GetValue<TEnum>([NotNull] string enumValueName, bool ignoreCase)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrEmpty(enumValueName))
        {
            throw new ArgumentException("The value can be neither empty string nor null.", nameof(enumValueName));
        }

        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
        {
            throw new ArgumentException(
                AsInvariant($@"The type {enumType.GetFullName().ToUIString()} is not an enumeration."),
                nameof(TEnum));
        }

        if (!Enum.TryParse<TEnum>(enumValueName, ignoreCase, out var result))
        {
            throw new ArgumentException(
                AsInvariant($@"Cannot parse the value {enumValueName.ToUIString()} as {typeof(TEnum).GetFullName().ToUIString()}"),
                nameof(enumValueName));
        }

        return result;
    }

    /// <summary>
    ///     Gets the value of the enumeration of the specified type by the string representation of
    ///     an enumeration value. This method is case-sensitive.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The system type of the enumeration.
    /// </typeparam>
    /// <param name="enumValueName">
    ///     The string representation of an enumeration value.
    /// </param>
    /// <returns>
    ///     A strongly-typed value of the enumeration.
    /// </returns>
    public static TEnum GetValue<TEnum>([NotNull] string enumValueName)
        where TEnum : struct, Enum
        => GetValue<TEnum>(enumValueName, false);

    /// <summary>
    ///     Gets all values of the specified enumeration.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The type of the enumeration.
    /// </typeparam>
    /// <returns>
    ///     A collection of the enumeration values.
    /// </returns>
    public static TEnum[] GetAllValues<TEnum>()
        where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum)
        {
            throw new ArgumentException(
                AsInvariant($@"The type {enumType.GetFullName().ToUIString()} is not an enumeration."),
                nameof(TEnum));
        }

        return (TEnum[])Enum.GetValues(enumType);
    }

    /// <summary>
    ///     Gets all the flag values defined in the specified enumeration.
    /// </summary>
    /// <typeparam name="TEnum">
    ///     The type of the enumeration.
    /// </typeparam>
    /// <returns>
    ///     An array of all the flag values defined in the specified enumeration.
    /// </returns>
    public static TEnum[] GetAllFlagValues<TEnum>()
        where TEnum : struct, Enum
    {
        var enumType = typeof(TEnum);
        if (!enumType.IsEnum || !enumType.IsDefined(typeof(FlagsAttribute), false))
        {
            throw new ArgumentException(
                AsInvariant($@"The type {enumType.GetFullName().ToUIString()} is not a bit-field enumeration."),
                nameof(TEnum));
        }

        const int MaxBitCount = sizeof(ulong) * 8;

        var underlyingType = Enum.GetUnderlyingType(enumType);

        var resultList = new List<TEnum>(MaxBitCount);
        var values = GetAllValues<TEnum>();
        foreach (var value in values)
        {
            var ordinalValue = underlyingType == typeof(ulong)
                ? Convert.ToUInt64(value)
                : (ulong)Convert.ToInt64(value);

            if (ordinalValue == 0)
            {
                continue;
            }

            for (var offset = 0; offset < MaxBitCount; offset++)
            {
                var flag = 1UL << offset;

                //// ReSharper disable once InvertIf
                if ((flag & ordinalValue) == ordinalValue)
                {
                    resultList.Add(value);
                    break;
                }
            }
        }

        var result = resultList.Distinct().OrderBy(Factotum.For<TEnum>.IdentityMethod).ToArray();
        return result;
    }
}