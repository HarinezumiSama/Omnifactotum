using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <b>nullable</b> <see cref="DateTime"/> structure.
/// </summary>
public static class OmnifactotumNullableDateTimeExtensions
{
    /// <summary>
    ///     Converts the specified nullable <see cref="DateTime"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeExtensions.ToFixedString"/> if the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTime"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeExtensions.ToFixedString"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToFixedString(this DateTime? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedString();

    /// <summary>
    ///     Converts the specified nullable <see cref="DateTime"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeExtensions.ToFixedStringWithMilliseconds"/> if the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTime"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeExtensions.ToFixedStringWithMilliseconds"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [NotNull]
    public static string ToFixedStringWithMilliseconds(this DateTime? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedStringWithMilliseconds();

    /// <summary>
    ///     Converts the specified nullable <see cref="DateTime"/> value to its string representation using
    ///     <see cref="OmnifactotumDateTimeExtensions.ToPreciseFixedString"/> if the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <returns>
    ///     A string representation of the specified nullable <see cref="DateTime"/> value.
    /// </returns>
    /// <seealso cref="OmnifactotumDateTimeExtensions.ToPreciseFixedString"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    [NotNull]
    public static string ToPreciseFixedString(this DateTime? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToPreciseFixedString();
}