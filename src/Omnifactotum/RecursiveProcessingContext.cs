using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the context of the recursive processing.
    /// </summary>
    /// <typeparam name="T">
    ///     The types of the instances being processed recursively.
    /// </typeparam>
    public sealed class RecursiveProcessingContext<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecursiveProcessingContext{T}"/> class
        ///     using the specified equality comparer for the instances being processed recursively.
        /// </summary>
        /// <param name="equalityComparer">
        ///     The equality comparer to use for eliminating duplicated instances,
        ///     or <c>null</c> to use <see cref="ByReferenceEqualityComparer{T}"/>.
        /// </param>
        public RecursiveProcessingContext([CanBeNull] IEqualityComparer<T> equalityComparer)
        {
            var actualComparer = equalityComparer ?? ByReferenceEqualityComparer<T>.Instance;
            ItemsBeingProcessed = typeof(T).IsValueType ? null : new HashSet<T>(actualComparer);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RecursiveProcessingContext{T}"/> class
        ///     using the by-reference equality comparer for the instances being processed recursively.
        /// </summary>
        /// <seealso cref="ByReferenceEqualityComparer{T}"/>
        public RecursiveProcessingContext()
            : this(null)
        {
            // Nothing to do
        }

        internal HashSet<T> ItemsBeingProcessed
        {
            get;
        }
    }
}