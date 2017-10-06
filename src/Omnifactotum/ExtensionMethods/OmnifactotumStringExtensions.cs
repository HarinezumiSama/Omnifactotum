using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Omnifactotum;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.String"/> class.
    /// </summary>
    public static class OmnifactotumStringExtensions
    {
        /// <summary>
        ///     The double quote symbol.
        /// </summary>
        private const string DoubleQuote = "\"";

        /// <summary>
        ///     Determines whether the specified string is <c>null</c> or an <see cref="String.Empty"/> string.
        /// </summary>
        /// <param name="value">
        ///     The string value to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified string is <c>null</c> or an <see cref="String.Empty"/> string;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty([CanBeNull] this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        ///     Determines whether a specified string is <c>null</c>, <see cref="String.Empty"/>,
        ///     or consists only of white-space characters.
        /// </summary>
        /// <param name="value">
        ///     The string value to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified value is <c>null</c> or <see cref="String.Empty"/>, or if it consists
        ///     exclusively of white-space characters; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace([CanBeNull] this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        ///     Converts the specified string value to an equivalent <see cref="Boolean"/> value.
        ///     If the specified string value cannot be converted, <c>null</c> is returned.
        /// </summary>
        /// <param name="value">
        ///     The string value to convert. Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="Nullable{Boolean}"/> representation of the specified string value.
        /// </returns>
        [CanBeNull]
        public static bool? ToNullableBoolean([CanBeNull] this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            bool booleanResult;
            if (bool.TryParse(value, out booleanResult))
            {
                return booleanResult;
            }

            int intResult;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intResult))
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
        public static bool ToBoolean(this string value)
        {
            var proxyResult = ToNullableBoolean(value);
            if (!proxyResult.HasValue)
            {
                throw new ArgumentException("The specified value cannot be converted to Boolean.", "value");
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
        ///     Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> consisting of the elements of <paramref name="values"/> delimited
        ///     by the <paramref name="separator"/> string.
        /// </returns>
        [NotNull]
        public static string Join([NotNull] this IEnumerable<string> values, [CanBeNull] string separator)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            return string.Join(separator, values);
        }

        /// <summary>
        ///     Avoids the specified string value to be a <c>null</c> reference: returns the specified string value
        ///     if it is not <c>null</c> or an empty string otherwise.
        /// </summary>
        /// <param name="source">
        ///     The string value to secure from a <c>null</c> reference.
        /// </param>
        /// <returns>
        ///     The source string value if it is not <c>null</c>; otherwise, empty string.
        /// </returns>
        [NotNull]
        public static string AvoidNull([CanBeNull] this string source)
        {
            return source ?? string.Empty;
        }

        /// <summary>
        ///     <para>
        ///         Converts the specified string to its UI representation.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>The input value</term>
        ///             <description>The result of the method</description>
        ///         </listheader>
        ///         <item>
        ///             <term><c>null</c></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <c>null</c></term>
        ///             <description>
        ///                 An input value enclosed in the double quote characters ("). If the value
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
        /// <example>
        ///     <code>
        /// <![CDATA[
        ///         string value;
        ///
        ///         value = null;
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is null.
        ///
        ///         value = string.Empty;
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is "".
        ///
        ///         value = "Hello";
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is "Hello".
        ///
        ///         value = "Class 'MyClass' is found in project \"MyProject\".";
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is "Class 'MyClass' is found in project ""MyProject"".".
        /// ]]>
        ///     </code>
        /// </example>
        [NotNull]
        public static string ToUIString([CanBeNull] this string value)
        {
            const string DoubleDoubleQuote = DoubleQuote + DoubleQuote;

            return value == null
                ? OmnifactotumConstants.NullValueRepresentation
                : DoubleQuote + value.Replace(DoubleQuote, DoubleDoubleQuote) + DoubleQuote;
        }

        /// <summary>
        ///     Removes all leading and trailing occurrences of a set of characters specified in an array
        ///     from the specified <see cref="System.String"/> object.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="System.String"/> value to trim. Can be <c>null</c>.
        /// </param>
        /// <param name="trimChars">
        ///     An array of Unicode characters to remove, or <c>null</c>.
        /// </param>
        /// <returns>
        ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
        ///     parameter are removed from the start and end of the specified string.
        ///     If <paramref name="trimChars"/> is <c>null</c> or an empty array, Unicode white-space characters
        ///     are removed instead.
        /// </returns>
        [NotNull]
        public static string TrimSafely([CanBeNull] this string value, [CanBeNull] params char[] trimChars)
        {
            return value == null ? string.Empty : value.Trim(trimChars);
        }

        /// <summary>
        ///     Removes all leading occurrences of a set of characters specified in an array
        ///     from the specified <see cref="System.String"/> object.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="System.String"/> value to trim. Can be <c>null</c>.
        /// </param>
        /// <param name="trimChars">
        ///     An array of Unicode characters to remove, or <c>null</c>.
        /// </param>
        /// <returns>
        ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
        ///     parameter are removed from the start of the specified string.
        ///     If <paramref name="trimChars"/> is <c>null</c> or an empty array, Unicode white-space characters
        ///     are removed instead.
        /// </returns>
        [NotNull]
        public static string TrimStartSafely([CanBeNull] this string value, [CanBeNull] params char[] trimChars)
        {
            return value == null ? string.Empty : value.TrimStart(trimChars);
        }

        /// <summary>
        ///     Removes all trailing occurrences of a set of characters specified in an array
        ///     from the specified <see cref="System.String"/> object.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="System.String"/> value to trim. Can be <c>null</c>.
        /// </param>
        /// <param name="trimChars">
        ///     An array of Unicode characters to remove, or <c>null</c>.
        /// </param>
        /// <returns>
        ///     The string that remains after all occurrences of the characters in the <paramref name="trimChars"/>
        ///     parameter are removed from the end of the specified string.
        ///     If <paramref name="trimChars"/> is <c>null</c> or an empty array, Unicode white-space characters
        ///     are removed instead.
        /// </returns>
        [NotNull]
        public static string TrimEndSafely([CanBeNull] this string value, [CanBeNull] params char[] trimChars)
        {
            return value == null ? string.Empty : value.TrimEnd(trimChars);
        }

        /// <summary>
        ///     Shortens the specified <see cref="System.String"/> value if its length exceeds the specified length.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="System.String"/> value to shorten. Can be <c>null</c>.
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
        [NotNull]
        public static string Shorten([CanBeNull] this string value, int maximumLength)
        {
            if (maximumLength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "maximumLength",
                    maximumLength,
                    "The length must be a non-negative value.");
            }

            if (string.IsNullOrEmpty(value) || maximumLength == 0)
            {
                return string.Empty;
            }

            return value.Length <= maximumLength ? value : value.Substring(0, maximumLength);
        }

        /// <summary>
        ///     Replicates the specified string value the specified number of times.
        /// </summary>
        /// <param name="value">
        ///     The value to replicate. Can be <c>null</c>.
        /// </param>
        /// <param name="count">
        ///     The number of times to replicate the specified string value.
        /// </param>
        /// <returns>
        ///     The specified value repeated the specified number of times, or <see cref="String.Empty"/> if
        ///     <paramref name="value"/> is <c>null</c> or an empty string or <paramref name="count"/> is zero.
        /// </returns>
        [NotNull]
        public static string Replicate([CanBeNull] this string value, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "count",
                    count,
                    "The count must be a non-negative value.");
            }

            if (string.IsNullOrEmpty(value) || count == 0)
            {
                return string.Empty;
            }

            return new StringBuilder(checked(value.Length * count)).Insert(0, value, count).ToString();
        }
    }
}