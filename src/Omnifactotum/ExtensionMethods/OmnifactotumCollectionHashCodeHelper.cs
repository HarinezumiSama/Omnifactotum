using System.Linq;
using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System.Collections.Generic
{
    /// <summary>
    ///     Contains the extension methods for helping to compute the hash codes for collections of objects.
    /// </summary>
    public static class OmnifactotumCollectionHashCodeHelper
    {
        private const int NoElementsHashCode = 0;

        /// <summary>
        ///     Computes the hash code of the specified collection by combining hash codes of the
        ///     elements in the collection into a new hash code. Returns <c>0</c> if the specified
        ///     collection is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compute a hash code of.
        /// </param>
        /// <returns>
        ///     A hash code of the specified collection, or <c>0</c> if the specified collection is
        ///     <c>null</c> or empty.
        /// </returns>
        public static int ComputeCollectionHashCode<T>([CanBeNull] this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return NoElementsHashCode;
            }

            return collection.Aggregate(
                NoElementsHashCode,
                (current, item) => current.CombineHashCodeValues(item.GetHashCodeSafely()));
        }
    }
}