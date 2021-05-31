using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="TimeSpan"/> structure.
    /// </summary>
    public static class OmnifactotumTimeSpanExtensions
    {
        /// <summary>
        ///     Multiplies the specified <see cref="TimeSpan"/> by the specified coefficient.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="TimeSpan"/> value to multiply.
        /// </param>
        /// <param name="coefficient">
        ///     The coefficient to multiply by.
        /// </param>
        /// <returns>
        ///     A new <see cref="TimeSpan"/> representing the original interval multiplied by the coefficient.
        /// </returns>
        [Pure]
        public static TimeSpan Multiply(this TimeSpan value, decimal coefficient)
            => TimeSpan.FromTicks(Convert.ToInt64(value.Ticks * coefficient));

        /// <summary>
        ///     Divides the specified <see cref="TimeSpan"/> by the specified divisor.
        /// </summary>
        /// <param name="value">
        ///     The <see cref="TimeSpan"/> value to divide.
        /// </param>
        /// <param name="divisor">
        ///     The divisor to divide by.
        /// </param>
        /// <returns>
        ///     A new <see cref="TimeSpan"/> representing the original interval divided by the divisor.
        /// </returns>
        [Pure]
        public static TimeSpan Divide(this TimeSpan value, decimal divisor)
        {
            if (divisor == 0m)
            {
                throw new ArgumentException(@"The divisor cannot be zero.", nameof(divisor));
            }

            return TimeSpan.FromTicks(Convert.ToInt64(value.Ticks / divisor));
        }
    }
}