using System.Runtime.CompilerServices;
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
}