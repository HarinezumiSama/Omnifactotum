using System.Runtime.CompilerServices;
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
    private static readonly string SingleQuoteCharUIString = new(OmnifactotumConstants.SingleQuoteChar, 4);

    /// <summary>
    ///     Converts the specified character to its UI representation as follows.
    ///     The result is the input value enclosed in the single quotes (<c>'</c>).
    ///     If the value is the single quote character, then it is duplicated in the result.
    /// </summary>
    /// <param name="value">
    ///     The character value to convert.
    /// </param>
    /// <returns>
    ///     A UI string representation of the specified character.
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToUIString(this char value)
        => value == OmnifactotumConstants.SingleQuoteChar
            ? SingleQuoteCharUIString
            : new string(stackalloc[] { OmnifactotumConstants.SingleQuoteChar, value, OmnifactotumConstants.SingleQuoteChar });
}