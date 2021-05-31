#if NET40
using Omnifactotum;
#else
using System.Collections.ObjectModel;
#endif
using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System.Collections.Generic
{
    /// <summary>
    ///     Contains extension methods for the generic dictionary interface.
    /// </summary>
    public static class OmnifactotumDictionaryExtensions
    {
        /// <summary>
        ///     Gets the value associated with the specified key from the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of keys in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionary.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <param name="defaultValue">
        ///     The value to return in a case when the specified key is not found in the dictionary.
        /// </param>
        /// <returns>
        ///     The value associated with the specified key if the key is found; otherwise, the default value for
        ///     the type of the value parameter.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            [NotNull]
#if NETFRAMEWORK
                this
#endif
                IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            TValue defaultValue)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return dictionary.TryGetValue(key, out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     Gets the value associated with the specified key from the specified dictionary.
        ///     If the specified key is not found, returns the default value for
        ///     the type of the value parameter.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of keys in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionary.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <returns>
        ///     The value associated with the specified key if the key is found; otherwise, the default value for
        ///     the type of the value parameter.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            [NotNull]
#if NETFRAMEWORK
                this
#endif
                IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key)
        {
            return GetValueOrDefault(dictionary, key, default);
        }

        /// <summary>
        ///     Gets the value associated with the specified key from the specified dictionary.
        ///     If the specified key is not found, then a value is created, using the specified value factory, and
        ///     added to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of keys in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionary.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <param name="valueFactory">
        ///     A reference to a method used to create a value if the key is not found.
        /// </param>
        /// <returns>
        ///     A value that was associated with the specified key, or has been associated if it was not.
        /// </returns>
        public static TValue GetOrCreateValue<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key,
            [NotNull] [InstantHandle] Func<TKey, TValue> valueFactory)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (valueFactory is null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            if (dictionary.TryGetValue(key, out var result))
            {
                return result;
            }

            result = valueFactory(key);
            dictionary.Add(key, result);

            return result;
        }

        /// <summary>
        ///     Gets the value associated with the specified key from the specified dictionary.
        ///     If the specified key is not found, then a value is created, using the default constructor for
        ///     the type <typeparamref name="TValue"/>, and added to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of keys in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionary.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <returns>
        ///     A value that was associated with the specified key, or has been associated if it was not.
        /// </returns>
        public static TValue GetOrCreateValue<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary,
            [NotNull] TKey key)
            where TValue : new()
            => GetOrCreateValue(dictionary, key, obj => new TValue());

        /// <summary>
        ///     Creates a read-only wrapper for the specified dictionary.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of keys in the dictionary.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of values in the dictionary.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to create a read-only wrapper for.
        /// </param>
        /// <returns>
        ///     A read-only wrapper for the specified dictionary.
        /// </returns>
        [NotNull]
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}