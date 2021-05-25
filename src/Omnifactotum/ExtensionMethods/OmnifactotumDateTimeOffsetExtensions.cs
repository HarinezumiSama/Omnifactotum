using System.Globalization;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="DateTimeOffset"/> structure.
    /// </summary>
    public static class OmnifactotumDateTimeOffsetExtensions
    {
        private const string FixedFormat = @"yyyy'-'MM'-'dd' 'HH':'mm':'ss' UTC'zzz";
        private const string PreciseFixedFormat = @"yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff' UTC'zzz";

        /// <summary>
        ///     Converts the specified <see cref="DateTimeOffset"/> value to its string
        ///     representation in the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified <see cref="DateTimeOffset"/> value.
        /// </returns>
        /// <example>
        ///     <code>
        ///         Console.WriteLine(new DateTimeOffset(2001, 2, 3, 7, 8, 9, 123, new TimeSpan(2, 30, 0)).ToFixedString())
        ///     </code>
        ///     <b>Result:</b> 2001-02-03 07:08:09 UTC+02:30
        /// </example>
        [Pure]
        public static string ToFixedString(this DateTimeOffset value)
            => value.ToString(FixedFormat, CultureInfo.InvariantCulture);

        /// <summary>
        ///     Converts the specified <see cref="DateTimeOffset"/> value to its precise string
        ///     representation in the format similar to extended ISO 8601.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A precise string representation of the specified <see cref="DateTimeOffset"/> value.
        /// </returns>
        /// <example>
        ///     <code>
        ///         Console.WriteLine(new DateTimeOffset(2001, 2, 3, 7, 8, 9, 123, new TimeSpan(2, 30, 0)).ToPreciseFixedString())
        ///     </code>
        ///     <b>Result:</b> 2001-02-03 07:08:09.1230000 UTC+02:30
        /// </example>
        [Pure]
        public static string ToPreciseFixedString(this DateTimeOffset value)
            => value.ToString(PreciseFixedFormat, CultureInfo.InvariantCulture);
    }
}