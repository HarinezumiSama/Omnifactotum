using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System
{
    /// <summary>
    ///     Contains extension methods for enumerations.
    /// </summary>
    public static class OmnifactotumEnumExtensions
    {
        /// <summary>
        ///     The type name format.
        /// </summary>
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
        public static string GetName([NotNull] this Enum value)
        {
            return GetNameInternal(value, null);
        }

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
        public static string GetQualifiedName([NotNull] this Enum value)
        {
            return GetNameInternal(value, false);
        }

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
        public static string GetFullName([NotNull] this Enum value)
        {
            return GetNameInternal(value, true);
        }

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
        ///     <c>true</c> if all bits specified in <paramref name="flags"/> are set
        ///     in <paramref name="enumerationValue"/>; otherwise, <c>false</c>.
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
        public static bool IsAllSet<TEnum>(this TEnum enumerationValue, TEnum flags)
            where TEnum : struct
        {
            return IsSetInternal(enumerationValue, flags, true);
        }

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
        ///     <c>true</c> if any of flags specified by the <paramref name="flags"/> parameter is set
        ///     in <paramref name="enumerationValue"/>; otherwise, <c>false</c>.
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
        public static bool IsAnySet<TEnum>(this TEnum enumerationValue, TEnum flags)
            where TEnum : struct
        {
            return IsSetInternal(enumerationValue, flags, false);
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
        ///     The collection of values to compare <paramref name="enumerationValue"/> to.
        /// </param>
        /// <returns>
        ///     <c>true</c> if any of values specified by the <paramref name="otherValues"/> equals to
        ///     <paramref name="enumerationValue"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     <typeparamref name="TEnum"/> is not an enumeration type.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="otherValues"/> is <c>null</c>.
        /// </exception>
        public static bool IsOneOf<TEnum>(this TEnum enumerationValue, IEnumerable<TEnum> otherValues)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    $@"The type {enumType.GetFullName().ToUIString()} is not an enumeration.",
                    nameof(TEnum));
            }

            if (otherValues == null)
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
        ///     <c>true</c> if any of values specified by the <paramref name="otherValues"/> equals to
        ///     <paramref name="enumerationValue"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     <typeparamref name="TEnum"/> is not an enumeration type.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="otherValues"/> is <c>null</c>.
        /// </exception>
        public static bool IsOneOf<TEnum>(this TEnum enumerationValue, params TEnum[] otherValues)
            where TEnum : struct
        {
            return IsOneOf(enumerationValue, (IEnumerable<TEnum>)otherValues);
        }

        /// <summary>
        ///     Ensures that the specified enumeration value is defined in the corresponding enumeration and
        ///     if it is not, throws <see cref="System.ComponentModel.InvalidEnumArgumentException"/>.
        /// </summary>
        /// <param name="enumerationValue">
        ///     The enumeration value to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified enumeration value is defined in the corresponding enumeration;
        ///     otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="enumerationValue"/> is <c>null</c>.
        /// </exception>
        public static bool IsDefined([NotNull] this Enum enumerationValue)
        {
            if (enumerationValue == null)
            {
                throw new ArgumentNullException(nameof(enumerationValue));
            }

            return Enum.IsDefined(enumerationValue.GetType(), enumerationValue);
        }

        /// <summary>
        ///     Ensures that the specified enumeration value is defined in the corresponding enumeration and
        ///     if it is not, throws <see cref="System.ComponentModel.InvalidEnumArgumentException"/>.
        /// </summary>
        /// <param name="enumerationValue">
        ///     The enumeration value to check.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="enumerationValue"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        ///     <paramref name="enumerationValue"/> is not defined in the corresponding enumeration.
        /// </exception>
        public static void EnsureDefined([NotNull] this Enum enumerationValue)
        {
            if (enumerationValue == null)
            {
                throw new ArgumentNullException(nameof(enumerationValue));
            }

            if (!IsDefined(enumerationValue))
            {
                throw new InvalidEnumArgumentException(
                    nameof(enumerationValue),
                    (int)(object)enumerationValue,
                    enumerationValue.GetType());
            }
        }

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
        public static NotImplementedException CreateEnumValueNotImplementedException(
            [NotNull] this Enum enumerationValue)
        {
            if (enumerationValue == null)
            {
                throw new ArgumentNullException(nameof(enumerationValue));
            }

            return new NotImplementedException(
                $@"The operation for the enumeration value {enumerationValue.GetQualifiedName().ToUIString()} is not implemented.");
        }

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
        public static NotSupportedException CreateEnumValueNotSupportedException([NotNull] this Enum enumerationValue)
        {
            if (enumerationValue == null)
            {
                throw new ArgumentNullException(nameof(enumerationValue));
            }

            return new NotSupportedException(
                $@"The operation for the enumeration value {enumerationValue.GetQualifiedName().ToUIString()} is not supported.");
        }

        /// <summary>
        ///     Gets the name of the specified enumeration value.
        /// </summary>
        /// <param name="value">
        ///     The enumeration value.
        /// </param>
        /// <param name="fullEnumName">
        ///     Specifies whether to get the full name of the enumeration value.
        /// </param>
        /// <returns>
        ///     The name of the enumeration value.
        /// </returns>
        private static string GetNameInternal([NotNull] Enum value, bool? fullEnumName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var enumType = value.GetType();
            var enumValueName = Enum.GetName(enumType, value) ?? value.ToString();

            return fullEnumName.HasValue
                ? string.Format(NameFormat, fullEnumName.Value ? enumType.FullName : enumType.Name, enumValueName)
                : enumValueName;
        }

        /// <summary>
        ///     Checks the flags.
        /// </summary>
        /// <typeparam name="TEnum">
        ///     The type of the enumeration.
        /// </typeparam>
        /// <param name="enumerationValue">
        ///     The enumeration value.
        /// </param>
        /// <param name="flags">
        ///     The flags.
        /// </param>
        /// <param name="all">
        ///     Specifies whether to check all flags.
        /// </param>
        /// <returns>
        ///   A <see cref="System.Boolean"/> value.
        /// </returns>
        private static bool IsSetInternal<TEnum>(this TEnum enumerationValue, TEnum flags, bool all)
            where TEnum : struct
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum || !enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(
                    $@"The type {enumType.GetFullName().ToUIString()} is not a bit-field enumeration.",
                    nameof(TEnum));
            }

            var underlyingType = Enum.GetUnderlyingType(enumType);
            if (underlyingType == typeof(ulong))
            {
                var castFlags = Convert.ToUInt64(flags);
                var andedValue = Convert.ToUInt64(enumerationValue) & castFlags;
                return all ? andedValue == castFlags : andedValue != 0;
            }
            else
            {
                var castFlags = Convert.ToInt64(flags);
                var andedValue = Convert.ToInt64(enumerationValue) & castFlags;
                return all ? andedValue == castFlags : andedValue != 0;
            }
        }
    }
}