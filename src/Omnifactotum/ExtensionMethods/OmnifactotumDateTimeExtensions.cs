using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="DateTime"/> structure.
    /// </summary>
    public static class OmnifactotumDateTimeExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the specified <see cref="DateTime"/> value to its string representation in
        ///     the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified <see cref="DateTime"/> value.
        /// </returns>
        [DebuggerNonUserCode]
        public static string ToFixedString(this DateTime value)
        {
            // Converting to a string similar to 2013-09-16 14:45:10
            return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts the specified <see cref="DateTime"/> value to its precise string representation in
        ///     the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A precise string representation of the specified <see cref="DateTime"/> value.
        /// </returns>
        [DebuggerNonUserCode]
        public static string ToPreciseFixedString(this DateTime value)
        {
            // Converting to a string similar to 2013-09-16 14:45:10.7654321
            return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}