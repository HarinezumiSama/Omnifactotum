using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains the extension methods for helping to compute the hash codes for objects.
    /// </summary>
    public static class OmnifactotumHashCodeHelper
    {
        private const int HashCodeMagicMultiplier = 397;

        /// <summary>
        ///     Combines two specified hash code values into a new hash code.
        /// </summary>
        /// <param name="previousHashCode">
        ///     The hash code of a value which is considered as previous.
        /// </param>
        /// <param name="nextHashCode">
        ///     The hash code of a certain value which is considered as next.
        /// </param>
        /// <returns>
        ///     A new hash code produced from the specified hash codes.
        /// </returns>
        [Pure]
        public static int CombineHashCodeValues(this int previousHashCode, int nextHashCode)
            => unchecked(previousHashCode * HashCodeMagicMultiplier) ^ nextHashCode;

        /// <summary>
        ///     Combines hash code obtained from two specified instances into a new hash code.
        /// </summary>
        /// <typeparam name="TPrevious">
        ///     The type of an instance which is considered as previous.
        /// </typeparam>
        /// <typeparam name="TNext">
        ///     The type of an instance which is considered as next.
        /// </typeparam>
        /// <param name="previous">
        ///     A certain previous instance to get the hash code from.
        /// </param>
        /// <param name="next">
        ///     A certain next instance to get the hash code from.
        /// </param>
        /// <returns>
        ///     A new hash code produced from the hash codes obtained from the specified instances.
        /// </returns>
        [Pure]
        public static int CombineHashCodes<TPrevious, TNext>(
            [CanBeNull] this TPrevious previous,
            [CanBeNull] TNext next)
            => CombineHashCodeValues(previous.GetHashCodeSafely(), next.GetHashCodeSafely());

        /// <summary>
        ///     Combines a hash code obtained at a previous step and a hash code of the specified
        ///     instance into a new hash code.
        /// </summary>
        /// <typeparam name="TNext">
        ///     The type of an instance which is considered as next.
        /// </typeparam>
        /// <param name="previousHashCode">
        ///     The hash code of a value which is considered as previous.
        /// </param>
        /// <param name="next">
        ///     A certain next instance to get the hash code from.
        /// </param>
        /// <returns>
        ///     A new hash code produced from a hash code obtained at a previous step and a hash code
        ///     of the specified instance.
        /// </returns>
        [Pure]
        public static int CombineHashCodes<TNext>(this int previousHashCode, [CanBeNull] TNext next)
            => CombineHashCodeValues(previousHashCode, next.GetHashCodeSafely());
    }
}