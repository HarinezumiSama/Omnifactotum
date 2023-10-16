using System.Runtime.CompilerServices;
using System.Threading;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for <see cref="Span{T}"/>.
/// </summary>
public static class OmnifactotumSpanExtensions
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToHexString(this Span<byte> bytes, [CanBeNull] string? separator = null, bool upperCase = false)
        => ((ReadOnlySpan<byte>)bytes).ToHexString(separator, upperCase);

    /// <summary>
    ///     Transforms the multiline string using the specified transformation function for each line.
    /// </summary>
    /// <param name="value">
    ///     The span of characters representing the multiline string to transform.
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
        this Span<char> value,
        [NotNull] Func<string, int, string> transformLine,
        bool normalizeLineEndings = false,
        CancellationToken cancellationToken = default)
        => ((ReadOnlySpan<char>)value).TransformMultilineString(transformLine, normalizeLineEndings, cancellationToken);
}