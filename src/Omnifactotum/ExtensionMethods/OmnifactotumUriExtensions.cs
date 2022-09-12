//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullWhen = System.Diagnostics.CodeAnalysis.NotNullWhenAttribute;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System;

/// <summary>
///     Contains extension methods for the <see cref="Uri"/> class.
/// </summary>
public static class OmnifactotumUriExtensions
{
    private static readonly HashSet<string> WebSchemes = new(StringComparer.Ordinal)
    {
        Uri.UriSchemeHttp,
        Uri.UriSchemeHttps
    };

    /// <summary>
    ///     Determines whether the specified <see cref="Uri"/> is an absolute URI using a Web scheme, such as HTTP or HTTPS.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value to test.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="Uri"/> is an absolute URI using a Web scheme; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    [Pure]
    [DebuggerStepThrough]
    [ContractAnnotation("null => false", true)]
    public static bool IsWebUri([NotNullWhen(true)] [CanBeNull] this Uri? value)
        => value is { IsAbsoluteUri: true } && WebSchemes.Contains(value.Scheme);

    /// <summary>
    ///     Returns the specified value if it is an absolute URI using a Web scheme, such as HTTP or HTTPS;
    ///     otherwise, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value to check.
    /// </param>
    /// <returns>
    ///     The specified value if it is an absolute URI using a Web scheme, such as HTTP or HTTPS.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The specified value is <see langword="null"/> or is not an absolute URI or it is not using a Web scheme, such as HTTP or HTTPS.
    /// </exception>
    //// ReSharper disable once RedundantNullableFlowAttribute
    [return: NotNullIfNotNull(@"value")]
    [NotNull]
    [DebuggerStepThrough]
    [ContractAnnotation("null => stop", true)]
    public static Uri EnsureWebUri(
        [CanBeNull] this Uri? value)
        => value.IsWebUri()
            ? value
            : throw new ArgumentException($@"{value.ToUIString()} is not an absolute URI using a Web scheme.", nameof(value));

    /// <summary>
    ///     Returns the specified value if it is an absolute URI; otherwise, throws an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value to check.
    /// </param>
    /// <returns>
    ///     The specified value if it is an absolute URI.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The specified value is <see langword="null"/> or is not an absolute URI.
    /// </exception>
    //// ReSharper disable once RedundantNullableFlowAttribute
    [return: NotNullIfNotNull(@"value")]
    [NotNull]
    [DebuggerStepThrough]
    [ContractAnnotation("null => stop", true)]
    public static Uri EnsureAbsoluteUri(
        [CanBeNull] this Uri? value)
        => value is { IsAbsoluteUri: true }
            ? value
            : throw new ArgumentException($@"{value.ToUIString()} is not an absolute URI.", nameof(value));

    /// <summary>
    ///     <para>Converts the specified <see cref="Uri"/> to its UI representation.</para>
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
    ///                 A string representation of the input value enclosed in the double quote characters ("). If the value
    ///                 contains one or more double quote characters, then each of them is duplicated in the result.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified <see cref="Uri"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumStringExtensions.ToUIString(string?)"/>
    [NotNull]
    [Pure]
    public static string ToUIString([CanBeNull] this Uri? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.ToString().ToUIString();

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="Uri"/> which is equal to the specified <see cref="Uri"/> ending with a single trailing forward
    ///         slash character ("/").
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="Uri"/> ends with exactly one forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="Uri"/> object is returned with a single forward slash character appended. If
    ///         the specified <see cref="Uri"/> ends with multiple forward slash characters, a new <see cref="Uri"/> object is
    ///         returned with the number of the trailing forward slash characters reduced to exactly one.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value that needs to end with the single forward slash character.
    /// </param>
    /// <returns>
    ///     A <see cref="Uri"/> which is equal to the specified <see cref="Uri"/> ending with a single trailing forward slash
    ///     character ("/").
    /// </returns>
    [NotNull]
    [Pure]
    public static Uri WithSingleTrailingSlash([NotNull] this Uri value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var uriString = value.ToString();
        var resultUriString = uriString.WithSingleTrailingSlash();

        return ReferenceEquals(resultUriString, uriString)
            ? value
            : new Uri(resultUriString, value.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
    }

    /// <summary>
    ///     <para>
    ///         Returns a <see cref="Uri"/> which is equal to the specified <see cref="Uri"/> with the trailing forward slash
    ///         characters ("/") removed, if there were any.
    ///     </para>
    ///     <para>
    ///         If the specified <see cref="Uri"/> does not end with a forward slash character, then the original object is
    ///         returned; otherwise, a new <see cref="Uri"/> object is returned with the trailing forward slash characters removed.
    ///     </para>
    /// </summary>
    /// <param name="value">
    ///     The <see cref="Uri"/> value that needs to not end with any forward slash characters.
    /// </param>
    /// <returns>
    ///     A <see cref="Uri"/> which is equal to the specified <see cref="Uri"/> with the trailing forward slash characters ("/")
    ///     removed, if there were any.
    /// </returns>
    [NotNull]
    [Pure]
    public static Uri WithoutTrailingSlash([NotNull] this Uri value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var uriString = value.ToString();
        var resultUriString = uriString.WithoutTrailingSlash();

        return ReferenceEquals(resultUriString, uriString)
            ? value
            : new Uri(resultUriString, value.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
    }
}