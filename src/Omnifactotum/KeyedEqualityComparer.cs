using System;
using System.Collections;
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
    public sealed class KeyedEqualityComparer<T, TKey> : IEqualityComparer<T>, IEqualityComparer
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
        ///     The equality comparer to use when comparing objects' keys; or <c>null</c> to use the default
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
        ///     with the specified key selector and default key equality comparer.
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
        ///     <c>true</c> if the keys of the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals([CanBeNull] T x, [CanBeNull] T y)
        {
            if (!typeof(T).IsValueType)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return false;
                }
            }

            var keyX = this.KeySelector(x);
            var keyY = this.KeySelector(y);

            return this.KeyComparer.Equals(keyX, keyY);
        }

        /// <summary>
        ///     Returns a hash code for the key of the specified object.
        /// </summary>
        /// <param name="obj">
        ///     The object of type <typeparamref name="T"/> for which key a hash code is to be returned.
        /// </param>
        /// <returns>
        ///     A hash code for the key of the specified object.
        /// </returns>
        public int GetHashCode([CanBeNull] T obj)
        {
            if (!typeof(T).IsValueType && ReferenceEquals(obj, null))
            {
                return 0;
            }

            var key = this.KeySelector(obj);
            return ReferenceEquals(key, null) ? 0 : this.KeyComparer.GetHashCode(key);
        }

        #endregion

        #region IEqualityComparer Members

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="x">
        ///     The first object to compare.
        /// </param>
        /// <param name="y">
        ///     The second object to compare.
        /// </param>
        bool IEqualityComparer.Equals([CanBeNull] object x, [CanBeNull] object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            if ((x is T) && (y is T))
            {
                return Equals((T)x, (T)y);
            }

            throw new ArgumentException("Invalid argument type.");
        }

        /// <summary>
        ///     Returns a hash code for the key of the specified object.
        /// </summary>
        /// <param name="obj">
        ///     The object for which key a hash code is to be returned.
        /// </param>
        /// <returns>
        ///     A hash code for the key of the specified object.
        /// </returns>
        int IEqualityComparer.GetHashCode([CanBeNull] object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 0;
            }

            if (obj is T)
            {
                return GetHashCode((T)obj);
            }

            throw new ArgumentException("Invalid argument type.", "obj");
        }

        #endregion
    }
}