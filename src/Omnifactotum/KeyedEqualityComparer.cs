using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the equality comparer that uses a comparison key provided by the specified delegate.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the objects to compare.
    /// </typeparam>
    /// <typeparam name="TKey">
    ///     The type of the comparison key.
    /// </typeparam>
    public sealed class KeyedEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyedEqualityComparer{T,TKey}"/> class
        ///     with the specified key selector and key equality comparer.
        /// </summary>
        /// <param name="keySelector">
        ///     A reference to a method that provides a key for an object being compared.
        /// </param>
        /// <param name="keyComparer">
        ///     The equality comparer to use when comparing objects' keys; or <b>null</b> to use the default
        ///     equality comparer for type <typeparamref name="TKey"/>.
        /// </param>
        public KeyedEqualityComparer(
            [NotNull] Func<T, TKey> keySelector,
            [CanBeNull] IEqualityComparer<TKey> keyComparer)
        {
            #region Argument Check

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            #endregion

            this.KeySelector = keySelector;
            this.KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyedEqualityComparer{T,TKey}"/> class
        ///     with the specified key selector and default equality comparer.
        /// </summary>
        /// <param name="keySelector">
        ///     A reference to a method that provides a key for an object being compared.
        /// </param>
        public KeyedEqualityComparer([NotNull] Func<T, TKey> keySelector)
            : this(keySelector, null)
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a reference to a method that provides a key for an object being compared.
        /// </summary>
        [NotNull]
        public Func<T, TKey> KeySelector
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the equality comparer to use when comparing objects' keys.
        /// </summary>
        [NotNull]
        public IEqualityComparer<TKey> KeyComparer
        {
            get;
            private set;
        }

        #endregion

        #region IEqualityComparer<T> Members

        /// <summary>
        ///     Determines whether the keys of the specified objects are equal.
        /// </summary>
        /// <param name="x">
        ///     The first object of type <typeparamref name="T"/> to compare.
        /// </param>
        /// <param name="y">
        ///     The second object of type <typeparamref name="T"/> to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the keys of the specified objects are equal; otherwise, <b>false</b>.
        /// </returns>
        public bool Equals(T x, T y)
        {
            var keyX = this.KeySelector(x);
            var keyY = this.KeySelector(y);

            return this.KeyComparer.Equals(keyX, keyY);
        }

        /// <summary>
        ///     Returns a hash code for the key of the specified object.
        /// </summary>
        /// <param name="obj">
        ///     The object of type <typeparamref name="T"/> for which a hash code is to be returned.
        /// </param>
        /// <returns>
        ///     A hash code for the key of the specified object.
        /// </returns>
        public int GetHashCode(T obj)
        {
            var key = this.KeySelector(obj);
            return ReferenceEquals(key, null) ? 0 : this.KeyComparer.GetHashCode(key);
        }

        #endregion
    }
}