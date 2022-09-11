using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
    /// <summary>
    ///     Contains extension methods for the <b>nullable</b> <see cref="TimeSpan"/> structure.
    /// </summary>
    public static class OmnifactotumNullableTimeSpanExtensions
    {
        /// <summary>
        ///     Converts the specified nullable <see cref="TimeSpan"/> value to its string representation using
        ///     <see cref="OmnifactotumTimeSpanExtensions.ToFixedString"/> if the value is not <see langword="null"/>;
        ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified nullable <see cref="TimeSpan"/> value.
        /// </returns>
        [Omnifactotum.Annotations.NotNull]
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFixedString(this TimeSpan? value)
            => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedString();

        /// <summary>
        ///     Converts the specified nullable <see cref="TimeSpan"/> value to its string representation using
        ///     <see cref="OmnifactotumTimeSpanExtensions.ToFixedStringWithMilliseconds"/> if the value is not <see langword="null"/>;
        ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified nullable <see cref="TimeSpan"/> value.
        /// </returns>
        [Omnifactotum.Annotations.NotNull]
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToFixedStringWithMilliseconds(this TimeSpan? value)
            => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToFixedStringWithMilliseconds();

        /// <summary>
        ///     Converts the specified nullable <see cref="TimeSpan"/> value to its string representation using
        ///     <see cref="OmnifactotumTimeSpanExtensions.ToPreciseFixedString"/> if the value is not <see langword="null"/>;
        ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to convert.
        /// </param>
        /// <returns>
        ///     A string representation of the specified nullable <see cref="TimeSpan"/> value.
        /// </returns>
        [Omnifactotum.Annotations.NotNull]
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToPreciseFixedString(this TimeSpan? value)
            => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToPreciseFixedString();
    }
}