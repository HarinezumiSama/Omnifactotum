using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides utility methods for enumerations.
    /// </summary>
    public static class EnumFactotum
    {
        #region Public Methods

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
        public static TEnum GetValue<TEnum>(string enumValueName, bool ignoreCase)
            where TEnum : struct
        {
            #region Argument Check

            if (string.IsNullOrEmpty(enumValueName))
            {
                throw new ArgumentException("The value can be neither empty string nor null.", "enumValueName");
            }

            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    string.Format("The type must be an enumeration ({0}).", enumType.FullName),
                    //// ReSharper disable once NotResolvedInText
                    "TEnum");
            }

            #endregion

            return (TEnum)Enum.Parse(enumType, enumValueName, ignoreCase);
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
        public static TEnum GetValue<TEnum>(string enumValueName)
            where TEnum : struct
        {
            return GetValue<TEnum>(enumValueName, false);
        }

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
            where TEnum : struct
        {
            #region Argument Check

            var enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    string.Format("The type is not an enumeration ({0}).", enumType.FullName),
                    //// ReSharper disable once NotResolvedInText
                    "TEnum");
            }

            #endregion

            return (TEnum[])Enum.GetValues(typeof(TEnum));
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
            where TEnum : struct
        {
            #region Argument Check

            var enumType = typeof(TEnum);
            if (!enumType.IsEnum || !enumType.IsDefined(typeof(FlagsAttribute), false))
            {
                throw new ArgumentException(
                    string.Format("The type is not a flag enumeration ({0}).", enumType.FullName),
                    //// ReSharper disable once NotResolvedInText
                    "TEnum");
            }

            #endregion

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
                    if ((flag & ordinalValue) == ordinalValue)
                    {
                        resultList.Add(value);
                        break;
                    }
                }
            }

            var result = resultList.Distinct().OrderBy(Factotum.Identity).ToArray();
            return result;
        }

        #endregion
    }
}