//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullWhen = System.Diagnostics.CodeAnalysis.NotNullWhenAttribute;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System;

/// <summary>
///     Contains extension methods for the <see cref="SecureString"/> class.
/// </summary>
public static class OmnifactotumSecureStringExtensions
{
    /// <summary>
    ///     Determines whether the specified <see cref="SecureString"/> is <see langword="null"/> or its
    ///     <see cref="SecureString.Length"/> is zero.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="SecureString"/> value to test.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="SecureString"/> is <see langword="null"/> or its
    ///     <see cref="SecureString.Length"/> is zero; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [ContractAnnotation("null => true", true)]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool IsNullOrEmpty([NotNullWhen(false)] [CanBeNull] this SecureString? value)
        => value is null || value.Length == 0;

    /// <summary>
    ///     Converts the specified <see cref="SecureString"/> value to a plain text <see cref="string"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="SecureString"/> value to convert to a plain text <see cref="string"/>.
    /// </param>
    /// <returns>
    ///     <see langword="null"/> if the specified <see cref="SecureString"/> value is <see langword="null"/>; otherwise, a new
    ///     instance of <see cref="string"/> that contains the plain text from the specified <see cref="SecureString"/> value.
    /// </returns>
    [ContractAnnotation("null => null; notnull => notnull", true)]
    [CanBeNull]
    [Pure]
    [return: NotNullIfNotNull(@"value")]
    public static string? ToPlainText([CanBeNull] this SecureString? value)
    {
        switch (value)
        {
            case null:
                return null;

            case { Length: 0 }:
                return string.Empty;

            default:
                {
                    var intPtr = IntPtr.Zero;
                    try
                    {
                        intPtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                        return Marshal.PtrToStringUni(intPtr) ?? string.Empty;
                    }
                    finally
                    {
                        if (intPtr != IntPtr.Zero)
                        {
                            Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
                        }
                    }
                }
        }
    }
}