using System;
using System.Collections;
using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a read-only wrapper for the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of items in the collection.
    /// </typeparam>
    [Serializable]
    public sealed class ReadOnlyItemCollection<T>
        : ICollection<T>
#if !NET40
            ,
            IReadOnlyCollection<T>
#endif
    {
        private readonly ICollection<T> _collection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyItemCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">
        ///     The original collection to wrap.
        /// </param>
        public ReadOnlyItemCollection([NotNull] ICollection<T> collection)
            => _collection = collection ?? throw new ArgumentNullException(nameof(collection));

        /// <inheritdoc />
        public int Count => _collection.Count;

#if !NET40
        /// <inheritdoc />
        int IReadOnlyCollection<T>.Count => _collection.Count;
#endif

        /// <inheritdoc />
        bool ICollection<T>.IsReadOnly => true;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        /// <inheritdoc />
        void ICollection<T>.Add(T item) => throw CreateReadOnlyInstanceException();

        /// <inheritdoc />
        void ICollection<T>.Clear() => throw CreateReadOnlyInstanceException();

        /// <inheritdoc />
        public bool Contains(T item) => _collection.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        bool ICollection<T>.Remove(T item) => throw CreateReadOnlyInstanceException();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private NotSupportedException CreateReadOnlyInstanceException()
            => new NotSupportedException($@"The {GetType().GetQualifiedName()} instance cannot be modified because it is read-only.");
    }
}