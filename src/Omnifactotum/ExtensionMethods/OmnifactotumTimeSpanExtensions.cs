using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="TimeSpan"/> structure.
/// </summary>
public static class OmnifactotumTimeSpanExtensions
{
    private const string SignPrefixFormat = @"\-";
    private const string DaysPrefixFormat = @"d\.";
    private const string MillisecondsPostfixFormat = @"\.fff";
    private const string FractionalSecondsPostfixFormat = @"\.fffffff";

    private const string BaseFixedStringFormat = @"hh\:mm\:ss";
    private const string BaseFixedStringWithMillisecondsFormat = BaseFixedStringFormat + MillisecondsPostfixFormat;
    private const string BasePreciseFixedStringFormat = BaseFixedStringFormat + FractionalSecondsPostfixFormat;

    private static readonly string[] FixedStringFormats =
    {
        BaseFixedStringFormat,
        DaysPrefixFormat + BaseFixedStringFormat,
        SignPrefixFormat + BaseFixedStringFormat,
        SignPrefixFormat + DaysPrefixFormat + BaseFixedStringFormat
    };

    private static readonly string[] FixedStringWithMillisecondsFormats =
    {
        BaseFixedStringWithMillisecondsFormat,
        DaysPrefixFormat + BaseFixedStringWithMillisecondsFormat,
        SignPrefixFormat + BaseFixedStringWithMillisecondsFormat,
        SignPrefixFormat + DaysPrefixFormat + BaseFixedStringWithMillisecondsFormat
    };

    private static readonly string[] PreciseFixedStringFormats =
    {
        BasePreciseFixedStringFormat,
        DaysPrefixFormat + BasePreciseFixedStringFormat,
        SignPrefixFormat + BasePreciseFixedStringFormat,
        SignPrefixFormat + DaysPrefixFormat + BasePreciseFixedStringFormat
    };

    /// <summary>
    ///     Multiplies the specified <see cref="TimeSpan"/> by the specified coefficient.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="TimeSpan"/> value to multiply.
    /// </param>
    /// <param name="coefficient">
    ///     The coefficient to multiply by.
    /// </param>
    /// <returns>
    ///     A new <see cref="TimeSpan"/> representing the original interval multiplied by the coefficient.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static TimeSpan Multiply(this TimeSpan value, decimal coefficient)
        => TimeSpan.FromTicks(Convert.ToInt64(value.Ticks * coefficient));

    /// <summary>
    ///     Divides the specified <see cref="TimeSpan"/> by the specified divisor.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="TimeSpan"/> value to divide.
    /// </param>
    /// <param name="divisor">
    ///     The divisor to divide by.
    /// </param>
    /// <returns>
    ///     A new <see cref="TimeSpan"/> representing the original interval divided by the divisor.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static TimeSpan Divide(this TimeSpan value, decimal divisor)
    {
        if (divisor == 0m)
        {
            throw new ArgumentException(@"The divisor cannot be zero.", nameof(divisor));
        }

        return TimeSpan.FromTicks(Convert.ToInt64(value.Ticks / divisor));
    }

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="TimeSpan"/> value to its fixed string representation.
    ///     </para>
    ///
    ///     <para>The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="TimeSpan.Days"/> (present if not zero)</item>
    ///             <item><see cref="TimeSpan.Hours"/></item>
    ///             <item><see cref="TimeSpan.Minutes"/></item>
    ///             <item><see cref="TimeSpan.Seconds"/></item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A fixed string representation of the specified <see cref="TimeSpan"/> value.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>
    ///         Console.WriteLine(new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(1234567890123L).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(-1234567890123L).ToFixedString());
    ///     </code>
    ///     <b>Output:</b>
    ///     <code>
    ///         12:34:56
    ///         -12:34:56
    ///         1.12:34:56
    ///         -1.12:34:56
    ///         1.10:17:36
    ///         -1.10:17:36
    ///     </code>
    /// </example>
    [Omnifactotum.Annotations.NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToFixedString(this TimeSpan value) => InternalToFixedString(value, FixedStringFormats);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="TimeSpan"/> value to its fixed string representation.
    ///     </para>
    ///
    ///     <para>The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="TimeSpan.Days"/> (present if not zero)</item>
    ///             <item><see cref="TimeSpan.Hours"/></item>
    ///             <item><see cref="TimeSpan.Minutes"/></item>
    ///             <item><see cref="TimeSpan.Seconds"/></item>
    ///             <item><see cref="TimeSpan.Milliseconds"/></item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A fixed string representation of the specified <see cref="TimeSpan"/> value.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>
    ///         Console.WriteLine(new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(1234567890123L).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(-1234567890123L).ToFixedString());
    ///     </code>
    ///     <b>Output:</b>
    ///     <code>
    ///         12:34:56.789
    ///         -12:34:56.789
    ///         1.12:34:56.789
    ///         -1.12:34:56.789
    ///         1.10:17:36.789
    ///         -1.10:17:36.789
    ///     </code>
    /// </example>
    [Omnifactotum.Annotations.NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToFixedStringWithMilliseconds(this TimeSpan value) => InternalToFixedString(value, FixedStringWithMillisecondsFormats);

    /// <summary>
    ///     <para>
    ///         Converts the specified <see cref="TimeSpan"/> value to its precise fixed string representation.
    ///     </para>
    ///
    ///     <para>The result contains (in this order):
    ///         <list type="bullet">
    ///             <item><see cref="TimeSpan.Days"/> (present if not zero)</item>
    ///             <item><see cref="TimeSpan.Hours"/></item>
    ///             <item><see cref="TimeSpan.Minutes"/></item>
    ///             <item><see cref="TimeSpan.Seconds"/></item>
    ///             <item>The ten millionths of a second</item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A precise fixed string representation of the specified <see cref="TimeSpan"/> value.
    /// </returns>
    /// <example>
    ///     <b>Code:</b>
    ///     <code>
    ///         Console.WriteLine(new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(0, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(-new TimeSpan(1, 12, 34, 56, 789).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(1234567890123L).ToFixedString());
    ///         Console.WriteLine(TimeSpan.FromTicks(-1234567890123L).ToFixedString());
    ///     </code>
    ///     <b>Output:</b>
    ///     <code>
    ///         12:34:56.789
    ///         -12:34:56.789
    ///         1.12:34:56.789
    ///         -1.12:34:56.789
    ///         1.10:17:36.7890123
    ///         -1.10:17:36.7890123
    ///     </code>
    /// </example>
    [Omnifactotum.Annotations.NotNull]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToPreciseFixedString(this TimeSpan value) => InternalToFixedString(value, PreciseFixedStringFormats);

    //// ReSharper disable once SuggestBaseTypeForParameter :: Optimization
    private static string InternalToFixedString(this TimeSpan value, string[] formats)
    {
        // Bit 0 - days, bit 1 - sign
        var offset = (value.Days == 0 ? 0 : 0b1) + (value.Ticks >= 0 ? 0 : 0b10);

        var format = formats[offset];
        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}