using Omnifactotum;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    /// <summary>
    ///     Contains extension methods for the <see cref="ISet{T}"/> type.
    /// </summary>
    public static class OmnifactotumSetExtensions
    {
        /// <summary>
        ///     Returns a read-only wrapper for the specified set.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the set.
        /// </typeparam>
        /// <param name="set">
        ///     The set to get a read-only wrapper for.
        /// </param>
        /// <returns>
        ///     A read-only wrapper for the specified set.
        /// </returns>
        [NotNull]
        public static ReadOnlySet<T> AsReadOnly<T>([NotNull] this ISet<T> set)
        {
            if (set == null)
            {
                throw new ArgumentNullException(nameof(set));
            }

            return new ReadOnlySet<T>(set);
        }
    }
}