﻿using System.Collections.Generic;
#if (NETFRAMEWORK && !NET40) || NETSTANDARD || NETCOREAPP
using System.Runtime.CompilerServices;
#endif
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of
    ///     the <see cref="KeyValuePair{TKey,TValue}"/> type using type inference in a friendly way.
    /// </summary>
    public static class OmnifactotumKeyValuePair
    {
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
#if (NETFRAMEWORK && !NET40) || NETSTANDARD || NETCOREAPP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>([CanBeNull] TKey key, [CanBeNull] TValue value)
            => new KeyValuePair<TKey, TValue>(key, value);
    }
}