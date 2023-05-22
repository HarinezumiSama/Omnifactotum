using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Omnifactotum;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Net;

/// <summary>
///     Contains extension methods for the <see cref="HttpStatusCode"/> enumeration.
/// </summary>
public static class OmnifactotumHttpStatusCodeExtensions
{
    private const int MinSuccessfulHttpStatusCode = 200;
    private const int MaxSuccessfulHttpStatusCode = 299;

    private static readonly Dictionary<int, string> ExtraHttpStatusCodeValueMap =
        new()
        {
            { 418, @"IAmATeapot" },
            { 425, @"TooEarly" },
#if !NET5_0_OR_GREATER
            { 422, @"UnprocessableEntity" },
            { 429, @"TooManyRequests" },
            { 451, @"UnavailableForLegalReasons" }
#endif
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
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         Console.WriteLine("Status code: {0}.", HttpStatusCode.NotFound.ToUIString()); // Output: Status code: 404 NotFound.
    /// ]]>
    ///     </code>
    /// </example>
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
            : AsInvariant($"{valueAsInt:D}\x0020{valueAsString}");
    }

    /// <summary>
    ///     Gets a <see cref="bool"/> value that indicates if the HTTP status code is successful (that is, in the range <c>200 .. 299</c>, inclusive).
    /// </summary>
    /// <param name="value">
    ///     The <see cref="HttpStatusCode"/> value to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="HttpStatusCode"/> value is in the range <c>200 .. 299</c>, inclusive;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsSuccessful(this HttpStatusCode value) => (int)value is >= MinSuccessfulHttpStatusCode and <= MaxSuccessfulHttpStatusCode;
}