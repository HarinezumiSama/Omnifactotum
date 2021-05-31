#nullable enable

using System;
#if !NET40
using System.Globalization;
using System.Runtime.CompilerServices;
#endif
using Omnifactotum.Annotations;

namespace Omnifactotum
{
#if NET40

    /// <summary>
    ///     Provides the fake helper methods for interpolated strings since <c>System.FormattableString</c> is not available prior to
    ///     .NET Framework 4.6.
    /// </summary>
#else

    /// <summary>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>.NET version</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>.NET Framework 4.6.1+. NET Standard, .NET Core, and .NET 5+</term>
    ///             <description>Provides the helper methods for <see cref="FormattableString"/>.</description>
    ///         </item>
    ///         <item>
    ///             <term>.NET Framework prior to 4.6.1</term>
    ///             <description>
    ///                 Provides the fake helper methods for interpolated strings since <c>System.FormattableString</c> is not available
    ///                 at the run time.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
#endif
    internal static class FormattableStringFactotum
    {
#if NET40

        /// <summary>
        ///     Returns the specified string as is (the fake method for .NET Framework prior to 4.6.1).
        /// </summary>
        /// <param name="value">
        ///     The interpolated string value.
        /// </param>
        /// <returns>
        ///     The original string value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        internal static string AsInvariant([NotNull] string value) => value ?? throw new ArgumentNullException(nameof(value));

#else

        /// <summary>
        ///     Returns a result string in which arguments are formatted by using the conventions of the invariant culture.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="FormattableString"/> object to convert to a result string.
        /// </param>
        /// <returns>
        ///     The string that results from formatting the current instance by using the conventions of the invariant culture.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="CultureInfo.InvariantCulture"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string AsInvariant([NotNull] FormattableString value) => FormattableString.Invariant(value);
#endif
    }
}