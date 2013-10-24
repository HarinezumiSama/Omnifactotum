﻿using System.Diagnostics;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System.Collections.Generic

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Provides helper functionality for creating instances of
    ///     the <see cref="KeyValuePair{TKey,TValue}"/> type using type inference in a friendly way.
    /// </summary>
    [DebuggerNonUserCode]
    public static class KeyValuePair
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new <see cref="KeyValuePair{TKey,TValue}"/> using the specified key and value.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of the key.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="key">
        ///     The key to initialize a <see cref="KeyValuePair{TKey,TValue}"/> with.
        /// </param>
        /// <param name="value">
        ///     The value to initialize a <see cref="KeyValuePair{TKey,TValue}"/> with.
        /// </param>
        /// <returns>
        ///     A new <see cref="KeyValuePair{TKey,TValue}"/> having the specified key and value set.
        /// </returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        #endregion
    }
}