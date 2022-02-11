#nullable enable

using System.Runtime.CompilerServices;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
    /// <summary>
    ///     Contains math extension methods for the numbers.
    /// </summary>
    public static class OmnifactotumMathExtensions
    {
        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Sqr(this int value) => unchecked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint Sqr(this uint value) => unchecked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long Sqr(this long value) => unchecked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong Sqr(this ulong value) => unchecked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float Sqr(this float value) => value * value;

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method SUPPRESSES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Sqr(this double value) => value * value;

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method ENFORCES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int SqrChecked(this int value) => checked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method ENFORCES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static uint SqrChecked(this uint value) => checked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method ENFORCES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long SqrChecked(this long value) => checked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method ENFORCES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong SqrChecked(this ulong value) => checked(value * value);

        /// <summary>
        ///     Returns the square of the specified number.
        ///     This method ENFORCES overflow-checking.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square of.
        /// </param>
        /// <returns>
        ///     The square of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal SqrChecked(this decimal value) => value * value;

        /// <summary>
        ///     Returns the square root of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square root of.
        /// </param>
        /// <returns>
        ///     The square root of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Sqrt(this double value) => Math.Sqrt(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte Abs(this sbyte value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [CLSCompliant(false)]
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short Abs(this short value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Abs(this int value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long Abs(this long value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float Abs(this float value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double Abs(this double value) => Math.Abs(value);

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal Abs(this decimal value) => Math.Abs(value);
    }
}