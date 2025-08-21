using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for <see cref="ReadOnlySpan{T}"/>.
/// </summary>
public static class OmnifactotumReadOnlySpanExtensions
{
    /// <summary>
    ///     Converts the specified span of bytes to its equivalent string representation that is encoded with hexadecimal characters.
    /// </summary>
    /// <param name="bytes">
    ///     The span of bytes to convert.
    /// </param>
    /// <param name="separator">
    ///     An optional separator used to split hexadecimal representation of each byte.
    /// </param>
    /// <param name="upperCase">
    ///     <see langword="true"/> to use upper case letters (<c>A..F</c>) in the resulting hexadecimal string;
    ///     <see langword="false"/> to use lower case letters (<c>a..f</c>) in the resulting hexadecimal string.
    /// </param>
    /// <returns>
    ///     A hexadecimal string representation of the specified span of bytes.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static unsafe string ToHexString(this ReadOnlySpan<byte> bytes, [CanBeNull] string? separator = null, bool upperCase = false)
    {
        var bytesLength = bytes.Length;
        if (bytesLength == 0)
        {
            return string.Empty;
        }

        var resolvedSeparator = separator ?? string.Empty;
        var resultLength = checked(bytesLength * 2 + (bytesLength - 1) * resolvedSeparator.Length);

        fixed (byte* bytesPointer = bytes)
        {
            var state = (Pointer: (IntPtr)bytesPointer, Length: bytesLength, Separator: resolvedSeparator, UpperCase: upperCase);

            return string.Create(
                resultLength,
                state,
                static (targetSpan, parameters) =>
                {
                    var bytes = new ReadOnlySpan<byte>((byte*)parameters.Pointer, parameters.Length);
                    var separatorSpan = parameters.Separator.AsSpan();
                    var separatorLength = separatorSpan.Length;
                    var hexAlphaBase = (char)((parameters.UpperCase ? 'A' : 'a') - 10);

                    for (int index = 0, targetIndex = 0; index < bytes.Length; index++)
                    {
                        if (separatorLength != 0 && index > 0)
                        {
                            separatorSpan.CopyTo(targetSpan.Slice(targetIndex, separatorLength));
                            targetIndex += separatorLength;
                        }

                        var @byte = bytes[index];
                        targetSpan[targetIndex++] = GetHexDigit((uint)(@byte >> 4), hexAlphaBase);
                        targetSpan[targetIndex++] = GetHexDigit(@byte, hexAlphaBase);
                    }
                });
        }

        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
        static char GetHexDigit(uint value, char hexAlphaBase)
        {
            var digit = value & 0x0F;
            return (char)(digit < 10 ? digit + '0' : digit + hexAlphaBase);
        }
    }

    /// <summary>
    ///     Transforms the multiline string using the specified transformation function for each line.
    /// </summary>
    /// <param name="value">
    ///     The read-only span of characters representing the multiline string to transform.
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
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         var value1 = "A\rB\nC\r\nD".AsSpan();
    ///         var transformedValue1 = value1.TransformMultilineString((s, i) => $"{i}-{s}", false);
    ///         // `transformedValue1` is equal to "0-A\r1-B\n2-C\r\n3-D"
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         var value2 = "A\rB\nC\r\nD".AsSpan();
    ///         var transformedValue2 = value2.TransformMultilineString((s, i) => $"{i}-{s}", true);
    ///         // On Windows, `transformedValue2` is equal to "0-A\r\n1-B\r\n2-C\r\n3-D"
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string TransformMultilineString(
        this ReadOnlySpan<char> value,
        [NotNull] Func<string, int, string> transformLine,
        bool normalizeLineEndings = false,
        CancellationToken cancellationToken = default)
    {
        if (transformLine is null)
        {
            throw new ArgumentNullException(nameof(transformLine));
        }

        if (value.Length == 0)
        {
            return string.Empty;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var resultBuilder = new StringBuilder();

        const char CarriageReturn = '\r';
        const char LineFeed = '\n';

        var newLineSpan = normalizeLineEndings ? Environment.NewLine.AsSpan() : default;

        var position = 0;
        var index = -1;
        while (position < value.Length)
        {
            cancellationToken.ThrowIfCancellationRequested();

            checked
            {
                index++;
            }

            string line;
            ReadOnlySpan<char> lineSeparator;

            var remainingValueSpan = value[position..];

            var lineLength = remainingValueSpan.IndexOfAny(CarriageReturn, LineFeed);
            if (lineLength < 0)
            {
                line = new string(remainingValueSpan);
                lineSeparator = ReadOnlySpan<char>.Empty;

                position += remainingValueSpan.Length;
            }
            else
            {
                line = new string(remainingValueSpan[..lineLength]);

                var ch = remainingValueSpan[lineLength];
                position += lineLength + 1;

                var lineSeparatorLength = 1;
                if (ch == CarriageReturn && position < value.Length && value[position] == LineFeed)
                {
                    position++;
                    lineSeparatorLength++;
                }

                lineSeparator = normalizeLineEndings ? newLineSpan : remainingValueSpan.Slice(lineLength, lineSeparatorLength);
            }

            var updatedLine = transformLine(line, index);
            resultBuilder.Append(updatedLine);
            resultBuilder.Append(lineSeparator);
        }

        return resultBuilder.ToString();
    }

    /// <summary>
    ///     <para>
    ///         Converts the specified span of characters to its UI representation.
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
    ///     The span of characters to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified span of characters.
    /// </returns>
    /// <seealso cref="OmnifactotumStringExtensions.ToUIString"/>
    ///// <example>
    /////     <code>
    ///// <![CDATA[
    /////         ReadOnlySpan<char> value1 = string.Empty;
    /////         Console.WriteLine("Value is {0}.", value1.ToUIString()); // Output: Value is "".
    ///// ]]>
    /////     </code>
    /////     <code>
    ///// <![CDATA[
    /////         ReadOnlySpan<char> value2 = "Hello";
    /////         Console.WriteLine("Value is {0}.", value2.ToUIString()); // Output: Value is "Hello".
    ///// ]]>
    /////     </code>
    /////     <code>
    ///// <![CDATA[
    /////         ReadOnlySpan<char> value3 = "Class 'MyClass' is found in the project \"MyProject\".";
    /////         Console.WriteLine("Value is {0}.", value3.ToUIString()); // Output: Value is "Class 'MyClass' is found in the project ""MyProject"".".
    ///// ]]>
    /////     </code>
    ///// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString(this ReadOnlySpan<char> value)
    {
        switch (value)
        {
            case { Length: 0 }:
                return OmnifactotumConstants.DoubleDoubleQuote;

            case [OmnifactotumConstants.DoubleQuoteChar]:
                return "\"\"\"\"";

            case [var ch]:
                return string.Create(
                    3,
                    ch,
                    static (span, ch) =>
                    {
                        span[0] = OmnifactotumConstants.DoubleQuoteChar;
                        span[1] = ch;
                        span[2] = OmnifactotumConstants.DoubleQuoteChar;
                    });
        }

        const int MaxStackBufferLength = 1024 / sizeof(char);

        var firstDoubleQuoteCharIndex = value.IndexOf(OmnifactotumConstants.DoubleQuoteChar);
        if (firstDoubleQuoteCharIndex < 0)
        {
            var resultValueLength = value.Length + 2;

#if NET9_0_OR_GREATER
            return string.Create(
                resultValueLength,
                value,
                static (resultStringSpan, sourceValueSpan) =>
                {
                    resultStringSpan[0] = OmnifactotumConstants.DoubleQuoteChar;
                    sourceValueSpan.CopyTo(resultStringSpan[1..]);
                    resultStringSpan[^1] = OmnifactotumConstants.DoubleQuoteChar;
                });
#else
            unsafe
            {
                fixed (char* valuePointer = &value.GetPinnableReference())
                {
                    return string.Create(
                        resultValueLength,
                        (DataPointer: (IntPtr)valuePointer, DataLength: value.Length),
                        static (resultStringSpan, state) =>
                        {
                            var sourceValueSpan = new ReadOnlySpan<char>((char*)state.DataPointer, state.DataLength);

                            resultStringSpan[0] = OmnifactotumConstants.DoubleQuoteChar;
                            sourceValueSpan.CopyTo(resultStringSpan[1..]);
                            resultStringSpan[^1] = OmnifactotumConstants.DoubleQuoteChar;
                        });
                }
            }
#endif
        }

        var requiredBufferLength = value.Length * 2 + 2;
        var resultBuffer = requiredBufferLength > MaxStackBufferLength ? new char[requiredBufferLength] : stackalloc char[requiredBufferLength];

        var resultLength = 0;
        resultBuffer[resultLength++] = OmnifactotumConstants.DoubleQuoteChar;

        var copiedSpan = value[..(firstDoubleQuoteCharIndex + 1)];
        copiedSpan.CopyTo(resultBuffer[resultLength..]);
        resultLength += copiedSpan.Length;

        resultBuffer[resultLength++] = OmnifactotumConstants.DoubleQuoteChar;

        for (var index = firstDoubleQuoteCharIndex + 1; index < value.Length; index++)
        {
            var ch = value[index];

            resultBuffer[resultLength++] = ch;
            if (ch == OmnifactotumConstants.DoubleQuoteChar)
            {
                resultBuffer[resultLength++] = OmnifactotumConstants.DoubleQuoteChar;
            }
        }

        resultBuffer[resultLength++] = OmnifactotumConstants.DoubleQuoteChar;

        return new string(resultBuffer[..resultLength]);
    }
}