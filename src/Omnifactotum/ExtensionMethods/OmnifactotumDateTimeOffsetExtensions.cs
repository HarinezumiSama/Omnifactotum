using System.Diagnostics;
using System.Globalization;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="DateTimeOffset"/> structure.
/// </summary>
public static class OmnifactotumDateTimeOffsetExtensions
{
    /// <summary>
    ///     The format used in the <see cref="ToFixedString"/> method.
    /// </summary>
    //// ReSharper disable once MemberCanBePrivate.Global
    public static readonly string FixedStringFormat =
        OmnifactotumDateTimeExtensions.FixedStringFormat + TimeZoneFormatPart;

    /// <summary>
    ///     The format used in the <see cref="ToFixedStringWithMilliseconds"/> method..
    /// </summary>
    //// ReSharper disable once MemberCanBePrivate.Global
    public static readonly string FixedStringWithMillisecondsFormat =
        OmnifactotumDateTimeExtensions.FixedStringWithMillisecondsFormat + TimeZoneFormatPart;

    /// <summary>
    ///     The format used in the <see cref="ToPreciseFixedString"/> method.
    /// </summary>
    //// ReSharper disable once MemberCanBePrivate.Global
    public static readonly string PreciseFixedStringFormat =
        OmnifactotumDateTimeExtensions.PreciseFixedStringFormat + TimeZoneFormatPart;

    private const string TimeZoneFormatPart = "' UTC'zzz";

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTimeOffset"/> value to its string representation in the format similar to
    ///         extended ISO 8601.
    ///     </para>
    ///
    ///     <para>The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="DateTime.Year"/></item>
    ///             <item><see cref="DateTime.Month"/></item>
    ///             <item><see cref="DateTime.Day"/></item>
    ///             <item><see cref="DateTime.Hour"/></item>
    ///             <item><see cref="DateTime.Minute"/></item>
    ///             <item><see cref="DateTime.Second"/></item>
    ///             <item>Time zone specifier</item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified <see cref="DateTimeOffset"/> value in the format similar to extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0)).ToFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09 UTC+02:30</code>
    /// </example>
    /// <seealso cref="FixedStringFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    public static string ToFixedString(this DateTimeOffset value)
        => value.ToString(FixedStringFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTimeOffset"/> value to its string representation in the format similar to
    ///         extended ISO 8601.
    ///     </para>
    ///
    ///     <para>
    ///         The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="DateTime.Year"/></item>
    ///             <item><see cref="DateTime.Month"/></item>
    ///             <item><see cref="DateTime.Day"/></item>
    ///             <item><see cref="DateTime.Hour"/></item>
    ///             <item><see cref="DateTime.Minute"/></item>
    ///             <item><see cref="DateTime.Second"/></item>
    ///             <item><see cref="DateTime.Millisecond"/></item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified <see cref="DateTimeOffset"/> value in the format similar to extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0)).ToFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09.456 UTC+02:30</code>
    /// </example>
    /// <seealso cref="FixedStringWithMillisecondsFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    public static string ToFixedStringWithMilliseconds(this DateTimeOffset value)
        => value.ToString(FixedStringWithMillisecondsFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTimeOffset"/> value to its precise string representation in the format
    ///         similar to extended ISO 8601.
    ///     </para>
    ///
    ///     <para>
    ///         The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="DateTime.Year"/></item>
    ///             <item><see cref="DateTime.Month"/></item>
    ///             <item><see cref="DateTime.Day"/></item>
    ///             <item><see cref="DateTime.Hour"/></item>
    ///             <item><see cref="DateTime.Minute"/></item>
    ///             <item><see cref="DateTime.Second"/></item>
    ///             <item>The ten millionths of a second</item>
    ///             <item>Time zone specifier</item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A precise string representation of the specified <see cref="DateTimeOffset"/> value in the format similar to
    ///     extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0)).ToPreciseFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09.4560000 UTC+02:30</code>
    /// </example>
    /// <seealso cref="PreciseFixedStringFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    public static string ToPreciseFixedString(this DateTimeOffset value)
        => value.ToString(PreciseFixedStringFormat, CultureInfo.InvariantCulture);
}