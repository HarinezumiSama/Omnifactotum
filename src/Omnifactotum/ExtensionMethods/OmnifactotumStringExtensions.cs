using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullWhen = System.Diagnostics.CodeAnalysis.NotNullWhenAttribute;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullableFlowAttribute
//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="string"/> class.
/// </summary>
public static class OmnifactotumStringExtensions
{
    /// <summary>
    ///     The default value of the minimum secured part length parameter in the <see cref="ToSecuredUIString"/> method.
    /// </summary>
    public const int DefaultMinimumSecuredPartLength = 16;

    /// <summary>
    ///     The default value of the logged part length parameter in the <see cref="ToSecuredUIString"/> method.
    /// </summary>
    public const int DefaultLoggedPartLength = 4;

    /// <summary>
    ///     Determines whether the specified string is <see langword="null"/> or an <see cref="String.Empty"/> string.
    /// </summary>
    /// <param name="value">
    ///     The string value to test.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified string is <see langword="null"/> or an <see cref="String.Empty"/> string;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [ContractAnnotation("null => true", true)]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsNullOrEmpty([NotNullWhen(false)] [CanBeNull] this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    ///     Determines whether a specified string is <see langword="null"/>, <see cref="String.Empty"/>,
    ///     or consists only of white-space characters.
    /// </summary>
    /// <param name="value">
    ///     The string value to test.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified value is <see langword="null"/> or <see cref="String.Empty"/>, or if it consists
    ///     exclusively of white-space characters; otherwise, <see langword="false"/>.
    /// </returns>
    [ContractAnnotation("null => true", true)]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] [CanBeNull] this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    ///     Converts the specified string value to an equivalent <see cref="Boolean"/> value.
    ///     If the specified string value cannot be converted, <see langword="null"/> is returned.
    /// </summary>
    /// <param name="value">
    ///     The string value to convert. Can be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     The <see cref="Nullable{Boolean}"/> representation of the specified string value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CanBeNull]
    public static bool? ToNullableBoolean([CanBeNull] this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (bool.TryParse(value, out var booleanResult))
        {
            return booleanResult;
        }

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intResult))
        {
            return intResult != 0;
        }

        return null;
    }

    /// <summary>
    ///     Converts the specified string value to an equivalent <see cref="Boolean"/> value.
    ///     If the specified string value cannot be converted, an exception is thrown.
    /// </summary>
    /// <param name="value">
    ///     The string value to convert.
    /// </param>
    /// <returns>
    ///     The <see cref="System.Boolean"/> representation of the specified string value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool ToBoolean([NotNull] this string value)
    {
        var proxyResult = ToNullableBoolean(value);
        if (!proxyResult.HasValue)
        {
            throw new ArgumentException(
                AsInvariant($"The specified value cannot be converted to {nameof(Boolean)}."),
                nameof(value));
        }

        return proxyResult.Value;
    }

    /// <summary>
    ///     Concatenates a specified <see cref="System.String"/> separator between each element of
    ///     the specified <see cref="System.String"/> collection, yielding a single concatenated string.
    /// </summary>
    /// <param name="values">
    ///     The <see cref="System.String"/> collection whose items to concatenate.
    /// </param>
    /// <param name="separator">
    ///     The separator to insert between each element of the <see cref="System.String"/> collection.
    ///     Can be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="System.String"/> consisting of the elements of <paramref name="values"/> delimited
    ///     by the <paramref name="separator"/> string.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string Join(
        [NotNull] [ItemCanBeNull] [InstantHandle] this IEnumerable<string?> values,
        [CanBeNull] string? separator)
        => string.Join(separator, (values ?? throw new ArgumentNullException(nameof(values))).AvoidDefaultImmutableArray());

    /// <summary>
    ///     Filters a sequence of string elements and returns only those that are not <see langword="null"/> and not an <see cref="string.Empty"/> string.
    /// </summary>
    /// <param name="source">
    ///     A sequence of string elements to filter.
    /// </param>
    /// <returns>
    ///     A sequence containing only those elements from <paramref name="source" />
    ///     that are not <see langword="null"/> and not an <see cref="string.Empty"/> string.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" /> is <see langword="null" />.
    /// </exception>
    [LinqTunnel]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<string> WhereNotEmpty([NotNull] [ItemCanBeNull] this IEnumerable<string?> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source.AvoidDefaultImmutableArray())
        {
            if (!item.IsNullOrEmpty())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    ///     Filters a sequence of string elements and returns only those that are not <see langword="null"/>, are not an <see cref="string.Empty"/> string,
    ///     and do not consist exclusively of white-space characters.
    /// </summary>
    /// <param name="source">
    ///     A sequence of string elements to filter.
    /// </param>
    /// <returns>
    ///     A sequence containing only those elements from <paramref name="source" />
    ///     that are not <see langword="null"/>, are not an <see cref="string.Empty"/> string, and do not consist exclusively of white-space characters.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" /> is <see langword="null" />.
    /// </exception>
    [LinqTunnel]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<string> WhereNotBlank([NotNull] [ItemCanBeNull] this IEnumerable<string?> source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source.AvoidDefaultImmutableArray())
        {
            if (!item.IsNullOrWhiteSpace())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    ///     Returns the specified string value if it is not <see langword="null"/> and not an empty string (<c>""</c>);
    ///     otherwise, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The string value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if it is not <see langword="null"/> and not an empty string (<c>""</c>).
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="value"/> is <see langword="null"/> or an empty string (<c>""</c>).
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    [NotNull]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static string EnsureNotEmpty(
        [CanBeNull] [System.Diagnostics.CodeAnalysis.NotNull] this string? value,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(value))]
#endif
        string? valueExpression = null)
        => string.IsNullOrEmpty(value)
            ? throw new ArgumentException(
                valueExpression is null
                    ? "The value is null or an empty string."
                    : $"The following expression is null or an empty string: {{ {valueExpression} }}.",
                nameof(value))
            : value;

    /// <summary>
    ///     Returns the specified string value if it is not <see langword="null"/> and not a blank string;
    ///     otherwise, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The string value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if it is not <see langword="null"/> and not a blank string.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="value"/> is <see langword="null"/> or a blank string.
    /// </exception>
    /// <remarks>
    ///     A blank string is a string that is empty or that consists only of white-space characters.
    /// </remarks>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    [NotNull]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static string EnsureNotBlank(
        [CanBeNull] [System.Diagnostics.CodeAnalysis.NotNull] this string? value,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(value))]
#endif
        string? valueExpression = null)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException(
                valueExpression is null
                    ? "The value is null or a blank string."
                    : $"The following expression is null or a blank string: {{ {valueExpression} }}.",
                nameof(value))
            : value;

    /// <summary>
    ///     Avoids the specified string value being a <see langword="null"/> reference.
    ///     Returns the specified string value if it is not <see langword="null"/>; otherwise, returns <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="source">
    ///     The string value to secure from a <see langword="null"/> reference.
    /// </param>
    /// <returns>
    ///     The source string value if it is not <see langword="null"/>; otherwise, <see cref="string.Empty"/>.
    /// </returns>
    [Obsolete($"Use '{nameof(OmnifactotumStringExtensions)}.{nameof(EmptyIfNull)}()' instead.")]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string AvoidNull([CanBeNull] this string? source) => EmptyIfNull(source);

    /// <summary>
    ///     Returns <see cref="string.Empty"/> if the specified string value is <see langword="null"/>; otherwise, returns the original string value.
    /// </summary>
    /// <param name="value">
    ///     The string value to handle.
    /// </param>
    /// <returns>
    ///     <see cref="string.Empty"/> if the specified string value is <see langword="null"/>; otherwise, returns the original string value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static string EmptyIfNull([CanBeNull] this string? value) => value ?? string.Empty;

    /// <summary>
    ///     <para>
    ///         Converts the specified string value to its UI representation.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal "<b>null</b>".</description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 An input value enclosed in the double quote characters (<c>"</c>). If the value
    ///                 contains one or more double quote characters, then each of them is
    ///                 duplicated in the result.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="value">
    ///     The string value to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified string.
    /// </returns>
    /// <seealso cref="OmnifactotumStringBuilderExtensions.AppendUIString"/>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         string value1 = null;
    ///         Console.WriteLine("Value is {0}.", value1.ToUIString()); // Output: Value is null.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value2 = string.Empty;
    ///         Console.WriteLine("Value is {0}.", value2.ToUIString()); // Output: Value is "".
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value3 = "Hello";
    ///         Console.WriteLine("Value is {0}.", value3.ToUIString()); // Output: Value is "Hello".
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value4 = "Class 'MyClass' is found in the project \"MyProject\".";
    ///         Console.WriteLine("Value is {0}.", value4.ToUIString()); // Output: Value is "Class 'MyClass' is found in the project ""MyProject"".".
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString([CanBeNull] this string? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.AsSpan().ToUIString();

    /// <summary>
    ///     <para>
    ///         Converts the specified string value to its secured UI representation.
    ///     </para>
    ///     <para>
    ///         Depending on the input string and the <paramref name="loggedPartLength"/> and <paramref name="minimumSecuredPartLength"/> parameters,
    ///         the resulting secured UI representation has one of the following formats:
    ///         <list type="bullet">
    ///             <item><c>"ABC...XYZ"</c></item>
    ///             <item><c>{ Length = NNN }</c></item>
    ///         </list>
    ///         where:
    ///         <list type="bullet">
    ///             <item><c>ABC</c> is the first <paramref name="loggedPartLength"/> characters of the input string.</item>
    ///             <item><c>XYZ</c> is the last <paramref name="loggedPartLength"/> characters of the input string.</item>
    ///             <item><c>NNN</c> is the length of the input string.</item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The string value to convert.
    /// </param>
    /// <param name="minimumSecuredPartLength">
    ///     The minimum length of the part of the input string that should be hidden from the resulting UI representation.
    /// </param>
    /// <param name="loggedPartLength">
    ///     The length of the part in the beginning and the part in the end of the input string that are displayed in the resulting UI representation.
    /// </param>
    /// <returns>
    ///     The secured UI representation of the specified string.
    /// </returns>
    /// <seealso cref="OmnifactotumStringBuilderExtensions.AppendSecuredUIString"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToSecuredUIString(
        [CanBeNull] this string? value,
        int loggedPartLength = DefaultLoggedPartLength,
        int minimumSecuredPartLength = DefaultMinimumSecuredPartLength)
    {
        if (loggedPartLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loggedPartLength), loggedPartLength, "The value must be greater than zero.");
        }

        if (minimumSecuredPartLength <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumSecuredPartLength), minimumSecuredPartLength, "The value must be greater than zero.");
        }

        if (value is null)
        {
            return OmnifactotumRepresentationConstants.NullValueRepresentation;
        }

        var minimumLoggedValueLength = checked(loggedPartLength * 2 + minimumSecuredPartLength);

        var result = value.Length >= minimumLoggedValueLength
            ? ToUIString($"{value.Substring(0, loggedPartLength)}...{value.Substring(value.Length - loggedPartLength)}")
            : $"{{ {nameof(value.Length)} = {value.Length} }}";

        return result;
    }

    /// <summary>
    ///     Removes all leading and trailing occurrences of a set of characters specified in an array
    ///     from the specified <see cref="System.String"/> object.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="System.String"/> value to trim. Can be <see langword="null"/>.
    /// </param>
    /// <param name="trimChars">
    ///     An array of Unicode characters to remove, or <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
    ///     parameter are removed from the start and end of the specified string.
    ///     If <paramref name="value"/> is <see langword="null"/>, <see cref="string.Empty"/> is returned.
    ///     If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, Unicode white-space characters
    ///     are removed instead.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string TrimSafely([CanBeNull] this string? value, [CanBeNull] params char[]? trimChars)
        => value?.Trim(trimChars) ?? string.Empty;

    /// <summary>
    ///     Removes all leading occurrences of a set of characters specified in an array
    ///     from the specified <see cref="System.String"/> object.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="System.String"/> value to trim. Can be <see langword="null"/>.
    /// </param>
    /// <param name="trimChars">
    ///     An array of Unicode characters to remove, or <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
    ///     parameter are removed from the start of the specified string.
    ///     If <paramref name="value"/> is <see langword="null"/>, <see cref="string.Empty"/> is returned.
    ///     If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, Unicode white-space characters
    ///     are removed instead.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string TrimStartSafely([CanBeNull] this string? value, [CanBeNull] params char[]? trimChars)
        => value?.TrimStart(trimChars) ?? string.Empty;

    /// <summary>
    ///     Removes all trailing occurrences of a set of characters specified in an array
    ///     from the specified <see cref="System.String"/> object.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="System.String"/> value to trim. Can be <see langword="null"/>.
    /// </param>
    /// <param name="trimChars">
    ///     An array of Unicode characters to remove, or <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
    ///     parameter are removed from the end of the specified string.
    ///     If <paramref name="value"/> is <see langword="null"/>, <see cref="string.Empty"/> is returned.
    ///     If <paramref name="trimChars"/> is <see langword="null"/> or an empty array, Unicode white-space characters
    ///     are removed instead.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string TrimEndSafely([CanBeNull] this string? value, [CanBeNull] params char[]? trimChars)
        => value?.TrimEnd(trimChars) ?? string.Empty;

    /// <summary>
    ///     Removes the prefix from the specified string value.
    /// </summary>
    /// <param name="value">
    ///     The string value to remove the prefix from.
    /// </param>
    /// <param name="prefix">
    ///     The prefix to remove from <paramref name="value"/>.
    /// </param>
    /// <param name="comparison">
    ///     One of the enumeration values that determines how the prefix is searched.
    /// </param>
    /// <returns>
    ///     The specified string value without the specified prefix if it starts with this prefix; otherwise, the original string value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <para><paramref name="value"/> is <see langword="null"/>.</para>
    ///     -or-
    ///     <para><paramref name="prefix"/> is <see langword="null"/>.</para>
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string TrimPrefix([NotNull] this string value, [NotNull] string prefix, StringComparison comparison)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (prefix is null)
        {
            throw new ArgumentNullException(nameof(prefix));
        }

        return !prefix.IsNullOrEmpty() && value.StartsWith(prefix, comparison) ? value.Substring(prefix.Length) : value;
    }

    /// <summary>
    ///     Removes the postfix from the specified string value.
    /// </summary>
    /// <param name="value">
    ///     The string value to remove the postfix from.
    /// </param>
    /// <param name="postfix">
    ///     The postfix to remove from <paramref name="value"/>.
    /// </param>
    /// <param name="comparison">
    ///     One of the enumeration values that determines how the postfix is searched.
    /// </param>
    /// <returns>
    ///     The specified string value without the specified postfix if it ends with this postfix; otherwise, the original string value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <para><paramref name="value"/> is <see langword="null"/>.</para>
    ///     -or-
    ///     <para><paramref name="postfix"/> is <see langword="null"/>.</para>
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string TrimPostfix([NotNull] this string value, [NotNull] string postfix, StringComparison comparison)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (postfix is null)
        {
            throw new ArgumentNullException(nameof(postfix));
        }

        return !postfix.IsNullOrEmpty() && value.EndsWith(postfix, comparison) ? value.Substring(0, value.Length - postfix.Length) : value;
    }

    /// <summary>
    ///     Shortens the specified <see cref="System.String"/> value if its length exceeds the specified length.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="System.String"/> value to shorten. Can be <see langword="null"/>.
    /// </param>
    /// <param name="maximumLength">
    ///     The maximum length of the resulting string.
    /// </param>
    /// <returns>
    ///     A <see cref="System.String"/> value containing the first <paramref name="maximumLength"/> characters
    ///     of the original string value if the latter is longer than <paramref name="maximumLength"/>;
    ///     otherwise, the original string value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="maximumLength"/> is less than zero.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string Shorten([CanBeNull] this string? value, int maximumLength)
    {
        if (maximumLength < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumLength),
                maximumLength,
                "The length must be a non-negative value.");
        }

        return value is null || maximumLength == 0
            ? string.Empty
            : value.Length <= maximumLength
                ? value
                : value.Substring(0, maximumLength);
    }

    /// <summary>
    ///     Replicates the specified string value the specified number of times.
    /// </summary>
    /// <param name="value">
    ///     The value to replicate. Can be <see langword="null"/>.
    /// </param>
    /// <param name="count">
    ///     The number of times to replicate the specified string value.
    /// </param>
    /// <returns>
    ///     The specified value repeated the specified number of times, or <see cref="String.Empty"/> if
    ///     <paramref name="value"/> is <see langword="null"/> or an empty string or <paramref name="count"/> is zero.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string Replicate([CanBeNull] this string? value, int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "The count must be greater than or equal to zero.");
        }

        return value is null || value.Length == 0 || count == 0
            ? string.Empty
            : new StringBuilder(checked(value.Length * count)).Insert(0, value, count).ToString();
    }

    /// <summary>
    ///     Determines whether the specified <see cref="string"/> value represents an absolute URI using a Web scheme, such as HTTP
    ///     or HTTPS.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="string"/> value to test.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="string"/> value represents an absolute URI using a Web scheme;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsWebUri([NotNullWhen(true)] [CanBeNull] this string? value)
        => Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.IsWebUri();

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="string"/> which is equal to the specified <see cref="string"/> starting with a single leading
    ///         forward slash character (<c>/</c>).
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="string"/> starts with exactly one forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="string"/> object is returned with a single forward slash character prepended.
    ///         Thus, if the specified <see cref="string"/> starts with multiple forward slash characters, a new <see cref="string"/> object
    ///         is returned with the number of the leading forward slash characters reduced to exactly one.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="string"/> value that needs to start with the single forward slash character.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> which is equal to the specified <see cref="string"/> starting with a single leading forward slash
    ///     character (<c>/</c>).
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string WithSingleLeadingSlash([NotNull] this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.StartsWith(OmnifactotumConstants.Slash, StringComparison.Ordinal)
            && !value.StartsWith(OmnifactotumConstants.DoubleSlash, StringComparison.Ordinal)
                ? value
                : OmnifactotumConstants.Slash + value.TrimStart(OmnifactotumConstants.SlashChar);
    }

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="string"/> which is equal to the specified <see cref="string"/> with the leading forward slash
    ///         characters (<c>/</c>) removed, if there were any.
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="string"/> does not start with a forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="string"/> object is returned with the leading forward slash characters
    ///         removed.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="string"/> value that needs to not start with any forward slash characters.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> which is equal to the specified <see cref="string"/> with the leading forward slash characters
    ///     (<c>/</c>) removed, if there were any.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string WithoutLeadingSlash([NotNull] this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.TrimStart(OmnifactotumConstants.SlashChar);
    }

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="string"/> which is equal to the specified <see cref="string"/> ending with a single trailing
    ///         forward slash character (<c>/</c>).
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="string"/> ends with exactly one forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="string"/> object is returned with a single forward slash character appended.
    ///         Thus, if the specified <see cref="string"/> ends with multiple forward slash characters, a new <see cref="string"/> object
    ///         is returned with the number of the trailing forward slash characters reduced to exactly one.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="string"/> value that needs to end with the single forward slash character.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> which is equal to the specified <see cref="string"/> ending with a single trailing forward slash
    ///     character (<c>/</c>).
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string WithSingleTrailingSlash([NotNull] this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.EndsWith(OmnifactotumConstants.Slash, StringComparison.Ordinal)
            && !value.EndsWith(OmnifactotumConstants.DoubleSlash, StringComparison.Ordinal)
                ? value
                : value.TrimEnd(OmnifactotumConstants.SlashChar) + OmnifactotumConstants.Slash;
    }

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="string"/> which is equal to the specified <see cref="string"/> with the trailing forward slash
    ///         characters (<c>/</c>) removed, if there were any.
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="string"/> does not end with a forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="string"/> object is returned with the trailing forward slash characters
    ///         removed.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="string"/> value that needs to not end with any forward slash characters.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> which is equal to the specified <see cref="string"/> with the trailing forward slash characters
    ///     (<c>/</c>) removed, if there were any.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string WithoutTrailingSlash([NotNull] this string value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.TrimEnd(OmnifactotumConstants.SlashChar);
    }

    /// <summary>
    ///     Converts the specified plain text value to <see cref="SecureString"/>.
    /// </summary>
    /// <param name="value">
    ///     The plain text value to convert to <see cref="SecureString"/>.
    /// </param>
    /// <returns>
    ///     <see langword="null"/> if the specified plain text value is <see langword="null"/>; otherwise, a new instance of
    ///     <see cref="SecureString"/> that contains the specified plain text value.
    /// </returns>
    [ContractAnnotation("null => null; notnull => notnull", true)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(value))]
    public static unsafe SecureString? ToSecureString([CanBeNull] this string? value)
    {
        switch (value)
        {
            case null:
                return null;

            case { Length: 0 }:
                return new SecureString();

            default:
                fixed (char* valuePointer = value)
                {
                    return new SecureString(valuePointer, value.Length);
                }
        }
    }

    /// <summary>
    ///     Transforms the multiline string using the specified transformation function for each line.
    /// </summary>
    /// <param name="value">
    ///     The multiline string to transform.
    /// </param>
    /// <param name="transformLine">
    ///     A reference to a method used to transform each line in the multiline string.
    /// </param>
    /// <param name="normalizeLineEndings">
    ///     <see langword="true"/> if all the line endings in <paramref name="value"/> to replace with <see cref="Environment.NewLine"/>
    ///     in the resulting string; <see langword="false"/> to keep the original line endings.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A transformed multiline string.
    /// </returns>
    /// <remarks>
    ///     See <see cref="OmnifactotumReadOnlySpanExtensions.TransformMultilineString"/> for examples.
    /// </remarks>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string TransformMultilineString(
        [NotNull] this string value,
        [NotNull] Func<string, int, string> transformLine,
        bool normalizeLineEndings = false,
        CancellationToken cancellationToken = default)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return value.AsSpan().TransformMultilineString(transformLine, normalizeLineEndings, cancellationToken);
    }

    /// <summary>
    ///     Converts the specified <see cref="string"/> to title case (except for words that are entirely in upper case, which are considered to be acronyms).
    /// </summary>
    /// <param name="value">
    ///     The string to convert to title case.
    /// </param>
    /// <param name="cultureInfo">
    ///     A <see cref="CultureInfo"/> to use for conversion, or <see langword="null"/> to use <see cref="CultureInfo.CurrentCulture"/>.
    /// </param>
    /// <returns>
    ///     The specified string converted to title case.
    /// </returns>
    /// <seealso cref="ToTitleCaseForced"/>
    /// <seealso cref="ToTitleCaseInvariant"/>
    /// <seealso cref="ToTitleCaseInvariantForced"/>
    [ContractAnnotation("value:null => null; value:notnull => notnull", true)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToTitleCase([CanBeNull] this string? value, [CanBeNull] CultureInfo? cultureInfo = null)
        => value is null ? null : (cultureInfo ?? CultureInfo.CurrentCulture).TextInfo.ToTitleCase(value);

    /// <summary>
    ///     Converts the specified <see cref="string"/> to title case (including words that are entirely in upper case).
    /// </summary>
    /// <param name="value">
    ///     The string to convert to title case.
    /// </param>
    /// <param name="cultureInfo">
    ///     A <see cref="CultureInfo"/> to use for conversion, or <see langword="null"/> to use <see cref="CultureInfo.CurrentCulture"/>.
    /// </param>
    /// <returns>
    ///     The specified string converted to title case.
    /// </returns>
    /// <seealso cref="ToTitleCase"/>
    /// <seealso cref="ToTitleCaseInvariant"/>
    /// <seealso cref="ToTitleCaseInvariantForced"/>
    [ContractAnnotation("value:null => null; value:notnull => notnull", true)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToTitleCaseForced([CanBeNull] this string? value, [CanBeNull] CultureInfo? cultureInfo = null)
        => value?.ToLower(cultureInfo).ToTitleCase(cultureInfo);

    /// <summary>
    ///     Converts the specified <see cref="string"/> to title case using <see cref="CultureInfo.InvariantCulture"/>
    ///     (except for words that are entirely in upper case, which are considered to be acronyms).
    /// </summary>
    /// <param name="value">
    ///     The string to convert to title case.
    /// </param>
    /// <returns>
    ///     The specified string converted to title case.
    /// </returns>
    /// <seealso cref="ToTitleCase"/>
    /// <seealso cref="ToTitleCaseForced"/>
    /// <seealso cref="ToTitleCaseInvariantForced"/>
    [ContractAnnotation("value:null => null; value:notnull => notnull", true)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToTitleCaseInvariant([CanBeNull] this string? value) => value.ToTitleCase(CultureInfo.InvariantCulture);

    /// <summary>
    ///     Converts the specified <see cref="string"/> to title case using <see cref="CultureInfo.InvariantCulture"/>
    ///     (including words that are entirely in upper case).
    /// </summary>
    /// <param name="value">
    ///     The string to convert to title case.
    /// </param>
    /// <returns>
    ///     The specified string converted to title case.
    /// </returns>
    /// <seealso cref="ToTitleCase"/>
    /// <seealso cref="ToTitleCaseForced"/>
    /// <seealso cref="ToTitleCaseInvariant"/>
    [ContractAnnotation("value:null => null; value:notnull => notnull", true)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ToTitleCaseInvariantForced([CanBeNull] this string? value) => value.ToTitleCaseForced(CultureInfo.InvariantCulture);
}