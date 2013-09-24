using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides utility methods for enumerations.
    /// </summary>
    public static class EnumHelper
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

            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    string.Format("The type must be an enumeration ({0}).", enumType.FullName),
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

            Type enumType = typeof(TEnum);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(
                    string.Format("The type must be an enumeration ({0}).", enumType.FullName),
                    "TEnum");
            }

            #endregion

            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        #endregion
    }
}