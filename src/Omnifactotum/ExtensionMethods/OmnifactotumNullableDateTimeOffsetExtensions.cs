using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System;

/// <summary>
///     Contains extension methods for the <b>nullable</b> <see cref="DateTimeOffset"/> structure.
/// </summary>
public static class OmnifactotumNullableDateTimeOffsetExtensions
{
    /// <summary>
    ///     Converts the specified nullable <see cref="DateTimeOffset"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeOffsetExtensions.ToFixedString"/> if the the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTimeOffset"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeOffsetExtensions.ToFixedString"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToFixedString(this DateTimeOffset? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedString();

    /// <summary>
    ///     Converts the specified nullable <see cref="DateTimeOffset"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeOffsetExtensions.ToFixedStringWithMilliseconds"/> if the the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTimeOffset"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeOffsetExtensions.ToFixedStringWithMilliseconds"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToFixedStringWithMilliseconds(this DateTimeOffset? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedStringWithMilliseconds();

    /// <summary>
    ///     Converts the specified nullable <see cref="DateTimeOffset"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeOffsetExtensions.ToPreciseFixedString"/> if the the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTimeOffset"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeOffsetExtensions.ToPreciseFixedString"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToPreciseFixedString(this DateTimeOffset? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToPreciseFixedString();
}