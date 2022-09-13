using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="DateTime"/> structure.
/// </summary>
public static class OmnifactotumDateTimeExtensions
{
    /// <summary>
    ///     The format used in the <see cref="ToFixedString"/> method.
    /// </summary>
    //// ReSharper disable once ConvertToConstant.Global
    public static readonly string FixedStringFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";

    /// <summary>
    ///     The format used in the <see cref="ToFixedStringWithMilliseconds"/> method..
    /// </summary>
    public static readonly string FixedStringWithMillisecondsFormat = FixedStringFormat + "'.'fff";

    /// <summary>
    ///     The format used in the <see cref="ToPreciseFixedString"/> method..
    /// </summary>
    //// ReSharper disable once StringLiteralTypo :: Format specifier
    public static readonly string PreciseFixedStringFormat = FixedStringFormat + "'.'fffffff";

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTime"/> value to its string representation in the format similar to
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
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified <see cref="DateTime"/> value in the format similar to extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTime(2001, 2, 3, 7, 8, 9, 456).ToFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09</code>
    /// </example>
    /// <seealso cref="FixedStringFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static string ToFixedString(this DateTime value)
        => value.ToString(FixedStringFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTime"/> value to its string representation in the format similar to
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
    ///     A string representation of the specified <see cref="DateTime"/> value in the format similar to extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTime(2001, 2, 3, 7, 8, 9, 456).ToFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09.456</code>
    /// </example>
    /// <seealso cref="FixedStringWithMillisecondsFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static string ToFixedStringWithMilliseconds(this DateTime value)
        => value.ToString(FixedStringWithMillisecondsFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="DateTime"/> value to its precise string representation in the format
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
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A precise string representation of the specified <see cref="DateTime"/> value in the format similar to extended ISO 8601.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>Console.WriteLine(new DateTime(2001, 2, 3, 7, 8, 9, 456).ToFixedString());</code>
    ///     <b>Output:</b>
    ///     <code>2001-02-03 07:08:09.4560000</code>
    /// </example>
    /// <seealso cref="PreciseFixedStringFormat"/>
    [NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static string ToPreciseFixedString(this DateTime value)
        => value.ToString(PreciseFixedStringFormat, CultureInfo.InvariantCulture);

    /// <summary>
    ///     Returns the specified <see cref="DateTime"/> value if its <see cref="DateTime.Kind"/> is equal to the specified
    ///     <see cref="DateTimeKind"/>; otherwise, throws <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <param name="requiredKind">
    ///     The required <see cref="DateTimeKind"/> that <see cref="DateTime.Kind"/> of <paramref name="value"/> must be equal to.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if its <see cref="DateTime.Kind"/> is equal to the specified <see cref="DateTimeKind"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <see cref="DateTime.Kind"/> of <paramref name="value"/> is not equal to the specified <see cref="DateTimeKind"/>.
    /// </exception>
    [DebuggerStepThrough]
    public static DateTime EnsureKind(
        this DateTime value,
        DateTimeKind requiredKind,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression("value")]
#endif
        string? valueExpression = null)
    {
        if (value.Kind == requiredKind)
        {
            return value;
        }

        var details = valueExpression is null ? null : $"\x0020Expression: {{ {valueExpression} }}.";

        throw new ArgumentException(
            AsInvariant($@"The specified {nameof(DateTime)} value must be of the {requiredKind} kind, but is {value.Kind}.{details}"),
            nameof(value));

    }

    /// <summary>
    ///     Returns the specified <see cref="DateTime"/> value if its <see cref="DateTime.Kind"/> is equal
    ///     to <see cref="DateTimeKind.Utc"/>; otherwise, throws <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if its <see cref="DateTime.Kind"/> is equal to <see cref="DateTimeKind.Utc"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <see cref="DateTime.Kind"/> of <paramref name="value"/> is not equal to <see cref="DateTimeKind.Utc"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static DateTime EnsureUtc(
        this DateTime value,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression("value")]
#endif
        string? valueExpression = null)
        => value.EnsureKind(DateTimeKind.Utc, valueExpression);

    /// <summary>
    ///     Returns the specified <see cref="DateTime"/> value if its <see cref="DateTime.Kind"/> is equal
    ///     to <see cref="DateTimeKind.Local"/>; otherwise, throws <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if its <see cref="DateTime.Kind"/> is equal to <see cref="DateTimeKind.Local"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <see cref="DateTime.Kind"/> of <paramref name="value"/> is not equal to <see cref="DateTimeKind.Local"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static DateTime EnsureLocal(
        this DateTime value,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression("value")]
#endif
        string? valueExpression = null)
        => value.EnsureKind(DateTimeKind.Local, valueExpression);
}