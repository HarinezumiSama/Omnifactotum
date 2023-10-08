//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum;

/// <summary>
///     Provides the helper methods for <see cref="FormattableString"/>.
/// </summary>
internal static class FormattableStringFactotum
{
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    internal static string AsInvariant([NotNull] FormattableString value) => FormattableString.Invariant(value);
}