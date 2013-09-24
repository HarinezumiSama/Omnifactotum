using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains extension methods for the <see cref="DateTimeOffset"/> structure.
    /// </summary>
    public static class OmnifactotumDateTimeOffsetExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Converts the specified <see cref="DateTimeOffset"/> value to its string representation in
        ///     the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified <see cref="DateTimeOffset"/> value.
        /// </returns>
        [DebuggerNonUserCode]
        public static string ToFixedString(this DateTimeOffset value)
        {
            // Converting to a string similar to 2013-09-16 14:45:10 UTC-05:00
            return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss UTCzzz", CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Converts the specified <see cref="DateTimeOffset"/> value to its precise string representation in
        ///     the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A precise string representation of the specified <see cref="DateTimeOffset"/> value.
        /// </returns>
        [DebuggerNonUserCode]
        public static string ToPreciseFixedString(this DateTimeOffset value)
        {
            // Converting to a string similar to 2013-09-16 14:45:10.7654321 UTC-05:00
            return value.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff UTCzzz", CultureInfo.InvariantCulture);
        }

        #endregion
    }
}