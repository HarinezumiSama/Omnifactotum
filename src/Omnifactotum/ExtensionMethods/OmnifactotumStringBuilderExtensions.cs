using System.Text;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullWhen = System.Diagnostics.CodeAnalysis.NotNullWhenAttribute;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;

//// ReSharper disable RedundantNullableFlowAttribute
//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="string"/> class.
/// </summary>
public static class OmnifactotumStringBuilderExtensions
{
    /// <summary>
    ///     The default value of the minimum secured part length parameter in the <see cref="AppendSecuredUIString"/> method.
    /// </summary>
    public const int DefaultMinimumSecuredPartLength = OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength;

    /// <summary>
    ///     The default value of the logged part length parameter in the <see cref="AppendSecuredUIString"/> method.
    /// </summary>
    public const int DefaultLoggedPartLength = OmnifactotumStringExtensions.DefaultLoggedPartLength;

    /// <summary>
    ///     <para>
    ///         Converts the specified string value to its UI representation and appends to the specified <see cref="StringBuilder"/>.
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
    /// <param name="stringBuilder">
    ///     The <see cref="StringBuilder"/> to append the converted string to.
    /// </param>
    /// <param name="value">
    ///     The string value to convert and append to <see cref="StringBuilder"/>.
    /// </param>
    /// <returns>
    ///     A reference passed in the <paramref name="stringBuilder"/> parameter.
    /// </returns>
    /// <seealso cref="OmnifactotumStringExtensions.ToUIString"/>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         string value1 = null;
    ///         Console.WriteLine("Value is {0}.", new StringBuilder().AppendUIString(value1)); // Output: Value is null.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value2 = string.Empty;
    ///         Console.WriteLine("Value is {0}.", new StringBuilder().AppendUIString(value2)); // Output: Value is "".
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value3 = "Hello";
    ///         Console.WriteLine("Value is {0}.", new StringBuilder().AppendUIString(value3)); // Output: Value is "Hello".
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         string value4 = "Class 'MyClass' is found in project \"MyProject\".";
    ///         Console.WriteLine("Value is {0}.", new StringBuilder().AppendUIString(value4)); // Output: Value is "Class 'MyClass' is found in project ""MyProject"".".
    /// ]]>
    ///     </code>
    /// </example>
    [NotNull]
    public static StringBuilder AppendUIString([NotNull] this StringBuilder stringBuilder, [CanBeNull] string? value)
    {
        if (stringBuilder is null)
        {
            throw new ArgumentNullException(nameof(stringBuilder));
        }

        return value is null
            ? stringBuilder.Append(OmnifactotumRepresentationConstants.NullValueRepresentation)
            : stringBuilder
                .Append(OmnifactotumConstants.DoubleQuote)
                .Append(value.Replace(OmnifactotumConstants.DoubleQuote, OmnifactotumConstants.DoubleDoubleQuote))
                .Append(OmnifactotumConstants.DoubleQuote);
    }

    /// <summary>
    ///     <para>
    ///         Converts the specified string value to its secured UI representation and appends to the specified <see cref="StringBuilder"/>.
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
    /// <param name="stringBuilder">
    ///     The <see cref="StringBuilder"/> to append the converted string to.
    /// </param>
    /// <param name="value">
    ///     The string value to convert and append to <see cref="StringBuilder"/>.
    /// </param>
    /// <param name="minimumSecuredPartLength">
    ///     The minimum length of the part of the input string that should be hidden from the resulting UI representation.
    /// </param>
    /// <param name="loggedPartLength">
    ///     The length of the part in the beginning and the part in the end of the input string that are displayed in the resulting UI representation.
    /// </param>
    /// <returns>
    ///     A reference passed in the <paramref name="stringBuilder"/> parameter.
    /// </returns>
    /// <seealso cref="OmnifactotumStringExtensions.ToSecuredUIString"/>
    [NotNull]
    public static StringBuilder AppendSecuredUIString(
        [NotNull] this StringBuilder stringBuilder,
        [CanBeNull] string? value,
        int loggedPartLength = DefaultLoggedPartLength,
        int minimumSecuredPartLength = DefaultMinimumSecuredPartLength)
    {
        if (stringBuilder is null)
        {
            throw new ArgumentNullException(nameof(stringBuilder));
        }

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
            return stringBuilder.Append(OmnifactotumRepresentationConstants.NullValueRepresentation);
        }

        var minimumLoggedValueLength = checked(loggedPartLength * 2 + minimumSecuredPartLength);

        return value.Length >= minimumLoggedValueLength
            ? stringBuilder.AppendUIString($"{value.Substring(0, loggedPartLength)}...{value.Substring(value.Length - loggedPartLength)}")
            : stringBuilder.Append($"{{ {nameof(value.Length)} = {value.Length} }}");
    }
}