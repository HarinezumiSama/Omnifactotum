#nullable enable

using System.Collections.Generic;
using System.Globalization;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Net
{
    /// <summary>
    ///     Contains extension methods for the <see cref="HttpStatusCode"/> enumeration.
    /// </summary>
    public static class OmnifactotumHttpStatusCodeExtensions
    {
        private static readonly Dictionary<int, string> ExtraHttpStatusCodeValueMap =
            new()
            {
                { 418, @"IAmATeapot" },
                { 422, @"UnprocessableEntity" },
                { 425, @"TooEarly" },
                { 429, @"TooManyRequests" },
                { 451, @"UnavailableForLegalReasons" }
            };

        /// <summary>
        ///     Converts the specified <see cref="HttpStatusCode"/> value to its UI representation.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="HttpStatusCode"/> value to convert.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified <see cref="HttpStatusCode"/> value.
        /// </returns>
        public static string ToUIString(this HttpStatusCode value)
        {
            var valueAsInt = (int)value;

            var valueAsString = Enum.IsDefined(typeof(HttpStatusCode), value)
                ? value.ToString()
                : ExtraHttpStatusCodeValueMap.TryGetValue(valueAsInt, out var stringValue)
                    ? stringValue
                    : null;

            return valueAsString is null
                ? valueAsInt.ToString(CultureInfo.InvariantCulture)
                : AsInvariant($@"{valueAsInt} {valueAsString}");
        }
    }
}