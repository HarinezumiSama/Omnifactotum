//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable CheckNamespace

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
        public static int Sqr(this int value)
        {
            return unchecked(value * value);
        }

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
        public static uint Sqr(this uint value)
        {
            return unchecked(value * value);
        }

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
        public static long Sqr(this long value)
        {
            return unchecked(value * value);
        }

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
        public static ulong Sqr(this ulong value)
        {
            return unchecked(value * value);
        }

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
        public static float Sqr(this float value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return unchecked(value * value);
        }

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
        public static double Sqr(this double value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return unchecked(value * value);
        }

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
        public static decimal Sqr(this decimal value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return unchecked(value * value);
        }

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
        public static int SqrChecked(this int value)
        {
            return checked(value * value);
        }

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
        public static uint SqrChecked(this uint value)
        {
            return checked(value * value);
        }

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
        public static long SqrChecked(this long value)
        {
            return checked(value * value);
        }

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
        public static ulong SqrChecked(this ulong value)
        {
            return checked(value * value);
        }

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
        public static float SqrChecked(this float value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return checked(value * value);
        }

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
        public static double SqrChecked(this double value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return checked(value * value);
        }

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
        public static decimal SqrChecked(this decimal value)
        {
            //// ReSharper disable once RedundantOverflowCheckingContext - Just making sure
            return checked(value * value);
        }

        /// <summary>
        ///     Returns the square root of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the square root of.
        /// </param>
        /// <returns>
        ///     The square root of the specified number.
        /// </returns>
        public static double Sqrt(this double value)
        {
            return Math.Sqrt(value);
        }

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
        public static sbyte Abs(this sbyte value)
        {
            return Math.Abs(value);
        }

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
        public static short Abs(this short value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        public static int Abs(this int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        public static long Abs(this long value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        public static float Abs(this float value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        public static double Abs(this double value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        ///     Returns the absolute value of the specified number.
        /// </summary>
        /// <param name="value">
        ///     The value to compute the absolute value of.
        /// </param>
        /// <returns>
        ///     The absolute value of the specified number.
        /// </returns>
        public static decimal Abs(this decimal value)
        {
            return Math.Abs(value);
        }
    }
}