using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the cache which is similar to a dictionary and leverages <see cref="WeakReference"/>
    ///     for the cached values.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the keys in the cache.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the values in the cache.
    /// </typeparam>
    public sealed class WeakReferenceBasedCache<TKey, TValue>
        where TValue : class
    {
        #region Constants and Fields

        private readonly Func<TKey, TValue> _valueFactory;
        private readonly Dictionary<TKey, WeakReference> _dictionary;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakReferenceBasedCache{TKey,TValue}"/> class
        ///     using the specified value factory and equality comparer for keys.
        /// </summary>
        /// <param name="valueFactory">
        ///     A reference to a method that creates a value for the specified key once needed.
        /// </param>
        /// <param name="keyEqualityComparer">
        ///     The equality comparer to use when comparing keys, or <c>null</c> to use
        ///     the default <see cref="EqualityComparer{T}"/> for the type of the key.
        /// </param>
        public WeakReferenceBasedCache(
            [NotNull] Func<TKey, TValue> valueFactory,
            [CanBeNull] IEqualityComparer<TKey> keyEqualityComparer)
        {
            #region Argument Check

            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            #endregion

            _valueFactory = valueFactory;
            _dictionary = new Dictionary<TKey, WeakReference>(keyEqualityComparer);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeakReferenceBasedCache{TKey,TValue}"/> class
        ///     using the specified value factory and default <see cref="EqualityComparer{T}"/> for the type of
        ///     the key.
        /// </summary>
        /// <param name="valueFactory">
        ///     A reference to a method that creates a value for the specified key once needed.
        /// </param>
        public WeakReferenceBasedCache([NotNull] Func<TKey, TValue> valueFactory)
            : this(valueFactory, null)
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the equality comparer used when comparing keys.
        /// </summary>
        /// <remarks>
        ///     This property is thread-safe.
        /// </remarks>
        [NotNull]
        public IEqualityComparer<TKey> KeyEqualityComparer
        {
            [DebuggerNonUserCode]
            get
            {
                return _dictionary.Comparer;
            }
        }

        /// <summary>
        ///     Gets the number of the items in the cache.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Count
        {
            [DebuggerNonUserCode]
            get
            {
                lock (_dictionary)
                {
                    return _dictionary.Count;
                }
            }
        }

        /// <summary>
        ///     Gets a value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to get a value for.
        /// </param>
        /// <returns>
        ///     A value associated with the specified key.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull]
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                #region Argument Check

                if (ReferenceEquals(key, null))
                {
                    throw new ArgumentNullException("key");
                }

                #endregion

                TValue result;
                lock (_dictionary)
                {
                    var valueReference = _dictionary.GetValueOrDefault(key);
                    if (valueReference == null)
                    {
                        result = CreateValue(key);
                        valueReference = new WeakReference(result, false);
                        _dictionary[key] = valueReference;
                    }
                    else if ((result = valueReference.Target as TValue) == null)
                    {
                        result = CreateValue(key);
                        valueReference.Target = result;
                    }
                }

                return result;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Clears the cache.
        /// </summary>
        public void Clear()
        {
            lock (_dictionary)
            {
                _dictionary.Clear();
            }
        }

        /// <summary>
        ///     Removes the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to remove.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove([NotNull] TKey key)
        {
            #region Argument Check

            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            lock (_dictionary)
            {
                return _dictionary.Remove(key);
            }
        }

        #endregion

        #region Private Methods

        private TValue CreateValue(TKey key)
        {
            return _valueFactory(key).EnsureNotNull();
        }

        #endregion
    }
}