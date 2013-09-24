using System;
using System.Diagnostics;
using Omnifactotum;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System.Collections.Generic

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains extension methods for the generic dictionary interface.
    /// </summary>
    public static class OmnifactotumDictionaryExtensions
    {
        #region Public Methods

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
        [DebuggerNonUserCode]
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            #region Argument Check

            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            #endregion

            TValue result;
            if (!dictionary.TryGetValue(key, out result))
            {
                result = defaultValue;
            }

            return result;
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
        [DebuggerNonUserCode]
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return GetValueOrDefault(dictionary, key, default(TValue));
        }

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
        [DebuggerNonUserCode]
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        #endregion
    }
}