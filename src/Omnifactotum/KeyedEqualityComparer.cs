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
            KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
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

        /// <summary>
        ///     Gets a reference to a method that provides a key for an object being compared.
        /// </summary>
        [NotNull]
        public Func<T, TKey> KeySelector
        {
            get;
        }

        /// <summary>
        ///     Gets the equality comparer to use when comparing objects' keys.
        /// </summary>
        [NotNull]
        public IEqualityComparer<TKey> KeyComparer
        {
            get;
        }

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
        public bool Equals(T x, T y)
        {
            if (!typeof(T).IsValueType)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }
            }

            var keyX = KeySelector(x);
            var keyY = KeySelector(y);

            return KeyComparer.Equals(keyX, keyY);
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
            if (!typeof(T).IsValueType && obj is null)
            {
                return 0;
            }

            var key = KeySelector(obj);
            return key is null ? 0 : KeyComparer.GetHashCode(key);
        }

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

            if (x is null || y is null)
            {
                return false;
            }

            return x is T castX && y is T castY && Equals(castX, castY);
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
            //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
            => obj is null ? 0 : obj is T castObj ? GetHashCode(castObj) : obj.GetHashCode();
    }
}