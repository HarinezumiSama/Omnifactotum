using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="char"/> structure.
/// </summary>
public static class OmnifactotumCharExtensions
{
    /// <summary>
    ///     Converts the specified character to its UI representation.
    /// </summary>
    /// <param name="value">
    ///     The character value to convert.
    /// </param>
    /// <returns>
    ///     An input value enclosed in the single quote characters (<c>'</c>). If the value
    ///     is the single quote character, then it is duplicated in the result.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         char value1 = ' ';
    ///         Console.WriteLine("Value is {0}.", value1.ToUIString()); // Output: Value is ' '.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         char value2 = 'H';
    ///         Console.WriteLine("Value is {0}.", value2.ToUIString()); // Output: Value is 'H'.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         char value3 = '\'';
    ///         Console.WriteLine("Value is {0}.", value3.ToUIString()); // Output: Value is ''''.
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString(this char value)
        => string.Concat(
            OmnifactotumConstants.SingleQuote,
            value == OmnifactotumConstants.SingleQuoteChar ? OmnifactotumConstants.DoubleSingleQuote : value.ToString(),
            OmnifactotumConstants.SingleQuote);
}