﻿using System;
using System.Collections.Generic;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of the <see cref="KeyedEqualityComparer{T,TKey}"/>
    ///     class using the type inference in a friendly way.
    /// </summary>
    public static class KeyedEqualityComparer
    {
        #region For<T> Class

        /// <summary>
        ///     Provides methods for creating instances of the <see cref="KeyedEqualityComparer{T,TKey}"/> class
        ///     for the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type to create <see cref="KeyedEqualityComparer{T,TKey}"/> instances for.
        /// </typeparam>
        public static class For<T>
        {
            #region Public Methods

            /// <summary>
            ///     Creates and initializes a new instance of the <see cref="KeyedEqualityComparer{T,TKey}"/>
            ///     class with the specified key selector and key equality comparer.
            /// </summary>
            /// <param name="keySelector">
            ///     A reference to a method that provides a key for an object being compared.
            /// </param>
            /// <param name="keyComparer">
            ///     The equality comparer to use when comparing objects' keys; or <c>null</c> to use the default
            ///     equality comparer for type <typeparamref name="TKey"/>.
            /// </param>
            /// <returns>
            ///     A created and initialized instance of the <see cref="KeyedEqualityComparer{T,TKey}"/>.
            /// </returns>
            public static KeyedEqualityComparer<T, TKey> Create<TKey>(
                [NotNull] Func<T, TKey> keySelector,
                [CanBeNull] IEqualityComparer<TKey> keyComparer)
            {
                return new KeyedEqualityComparer<T, TKey>(keySelector, keyComparer);
            }

            /// <summary>
            ///     Creates and initializes a new instance of the <see cref="KeyedEqualityComparer{T,TKey}"/>
            ///     class with the specified key selector and default key equality comparer.
            /// </summary>
            /// <param name="keySelector">
            ///     A reference to a method that provides a key for an object being compared.
            /// </param>
            /// <returns>
            ///     A created and initialized instance of the <see cref="KeyedEqualityComparer{T,TKey}"/>.
            /// </returns>
            public static KeyedEqualityComparer<T, TKey> Create<TKey>([NotNull] Func<T, TKey> keySelector)
            {
                return new KeyedEqualityComparer<T, TKey>(keySelector);
            }

            #endregion
        }

        #endregion
    }
}