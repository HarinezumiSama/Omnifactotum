﻿using System;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System.Collections.Generic

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains extension methods for collections, that is, for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class OmnifactotumCollectionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Get the number of elements in the specified collection without enumerating all
        ///     its elements. If it's not possible to do so, <b>null</b> is returned.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to get the number of elements in.
        /// </param>
        /// <returns>
        ///     The number of elements in the specified collection if it was possible to determine it without
        ///     enumerating collection's elements; otherwise, <b>null</b>.
        /// </returns>
        public static int? GetFastCount<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return 0;
            }

            var typedCastCollection = collection as ICollection<T>;
            if (typedCastCollection != null)
            {
                return typedCastCollection.Count;
            }

            var castCollection = collection as ICollection;
            if (castCollection != null)
            {
                return castCollection.Count;
            }

            return null;
        }

        /// <summary>
        ///     Performs the specified action for each element of the collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to perform an action for.
        /// </param>
        /// <param name="action">
        ///     A reference to a method representing the action to perform on an item;
        ///     the parameter represents the item to perform the action on.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="collection"/> is <b>null</b>.</para>
        ///     <para>-or-</para>
        ///     <para><paramref name="action"/> is <b>null</b>.</para>
        /// </exception>
        public static void DoForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            #region Argument Check

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            #endregion

            foreach (T item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        ///     Performs the specified action for each element of the collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to perform an action for.
        /// </param>
        /// <param name="action">
        ///     A reference to a method representing the action to perform on an item;
        ///     the first parameter represents the item to perform the action on;
        ///     the second parameter represents the zero-based index of the element in the collection.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="collection"/> is <b>null</b>.</para>
        ///     <para>-or-</para>
        ///     <para><paramref name="action"/> is <b>null</b>.</para>
        /// </exception>
        public static void DoForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            #region Argument Check

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            #endregion

            int index = 0;
            foreach (T item in collection)
            {
                action(item, index);
                index++;
            }
        }

        /// <summary>
        ///     Determines whether two specified collections contain identical items in any order.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collections.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compare to another collection.
        /// </param>
        /// <param name="otherCollection">
        ///     The another collection to compare.
        /// </param>
        /// <param name="comparer">
        ///     An <see cref="System.Collections.Generic.IEqualityComparer{T}"/> to compare elements in the
        ///     collections, or <b>null</b> to use the default equality comparer for the type <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <b>true</b> if two specified collections contain identical items in any order or
        ///     they both are <b>null</b>; otherwise, <b>false</b>.
        /// </returns>
        public static bool CollectionsEquivalent<T>(
            this IEnumerable<T> collection,
            IEnumerable<T> otherCollection,
            IEqualityComparer<T> comparer)
        {
            var fastResult = CheckReferenceAndCountEquality(collection, otherCollection);
            if (fastResult.HasValue)
            {
                return fastResult.Value;
            }

            var map = CreateCountMap(collection, comparer);
            var otherMap = CreateCountMap(otherCollection, comparer);

            if (map.Count != otherMap.Count)
            {
                return false;
            }

            foreach (var pair in map)
            {
                int otherCount;
                if (!otherMap.TryGetValue(pair.Key, out otherCount) || (otherCount != pair.Value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether two specified collections contain identical items in any order
        ///     using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collections.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compare to another collection.
        /// </param>
        /// <param name="otherCollection">
        ///     The another collection to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if two specified collections contain identical items in any order or
        ///     they both are <b>null</b>; otherwise, <b>false</b>.
        /// </returns>
        public static bool CollectionsEquivalent<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            return CollectionsEquivalent(collection, otherCollection, null);
        }

        /// <summary>
        ///     Determines whether two specified collections contain identical items in the same order.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collections.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compare to another collection.
        /// </param>
        /// <param name="otherCollection">
        ///     The another collection to compare.
        /// </param>
        /// <param name="comparer">
        ///     An <see cref="System.Collections.Generic.IEqualityComparer{T}"/> to compare elements in the
        ///     collections, or <b>null</b> to use the default equality comparer for the type <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <b>true</b> if two specified collections contain identical items in the same order or
        ///     they both are <b>null</b>; otherwise, <b>false</b>.
        /// </returns>
        public static bool CollectionsEqual<T>(
            this IEnumerable<T> collection,
            IEnumerable<T> otherCollection,
            IEqualityComparer<T> comparer)
        {
            var fastResult = CheckReferenceAndCountEquality(collection, otherCollection);
            if (fastResult.HasValue)
            {
                return fastResult.Value;
            }

            var actualComparer = comparer ?? EqualityComparer<T>.Default;
            using (var enumerator = collection.GetEnumerator())
            {
                using (var otherEnumerator = otherCollection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!otherEnumerator.MoveNext()
                            || !actualComparer.Equals(enumerator.Current, otherEnumerator.Current))
                        {
                            return false;
                        }
                    }

                    if (otherEnumerator.MoveNext())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether two specified collections contain identical items in the same order
        ///     using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collections.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compare to another collection.
        /// </param>
        /// <param name="otherCollection">
        ///     The another collection to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if two specified collections contain identical items in the same order or
        ///     they both are <b>null</b>; otherwise, <b>false</b>.
        /// </returns>
        public static bool CollectionsEqual<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            return CollectionsEqual(collection, otherCollection, null);
        }

        /// <summary>
        ///     Finds the duplicate items in the specified source collection according to the specified key selector
        ///     using the specified equality comparer for keys.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the source collection.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key by which the duplicates are determined.
        /// </typeparam>
        /// <param name="collection">
        ///     The source collection to find the duplicates in.
        /// </param>
        /// <param name="keySelector">
        ///     A reference to a method that returns a key, for a particular item, by which the duplicates
        ///     are determined.
        /// </param>
        /// <param name="comparer">
        ///     A equality comparer used to compare keys.
        /// </param>
        /// <returns>
        ///     A dictionary in which a key is a duplicated key from the source collection keys and a value is
        ///     the list of the corresponding duplicated items from the source collection.
        /// </returns>
        public static Dictionary<TKey, List<T>> FindDuplicates<T, TKey>(
            this IEnumerable<T> collection,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            #region Argument Check

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            #endregion

            var actualComparer = comparer ?? EqualityComparer<TKey>.Default;
            return collection
                .GroupBy(keySelector, actualComparer)
                .Where(group => group.Count() > 1)
                .Select(group => KeyValuePair.Create(group.Key, group.ToList()))
                .ToDictionary(item => item.Key, item => item.Value, actualComparer);
        }

        /// <summary>
        ///     Finds the duplicate items in the specified source collection according to the specified key selector
        ///     using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the source collection.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key by which the duplicates are determined.
        /// </typeparam>
        /// <param name="collection">
        ///     The source collection to find the duplicates in.
        /// </param>
        /// <param name="keySelector">
        ///     A reference to a method that returns a key, for a particular item, by which the duplicates
        ///     are determined.
        /// </param>
        /// <returns>
        ///     A dictionary in which a key is a duplicated key from the source collection keys and a value is
        ///     the list of the corresponding duplicated items from the source collection.
        /// </returns>
        public static Dictionary<TKey, List<T>> FindDuplicates<T, TKey>(
            this IEnumerable<T> collection,
            Func<T, TKey> keySelector)
        {
            return FindDuplicates(collection, keySelector, null);
        }

        /// <summary>
        ///     Safely disposes of each element of the specified collection.
        /// </summary>
        /// <typeparam name="TDisposable">
        ///     The type of the disposable elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     A collection of disposable elements.
        /// </param>
        /// <seealso cref="OmnifactotumDisposableExtensions.DisposeSafely{T}"/>
        public static void DisposeCollectionItemsSafely<TDisposable>(this IEnumerable<TDisposable> collection)
            where TDisposable : IDisposable
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                item.DisposeSafely();
            }
        }

        /// <summary>
        ///     Avoids the specified collection to be a <b>null</b> reference: returns the specified collection
        ///     if it is not <b>null</b> or an empty collection otherwise.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="source">
        ///     The collection to secure from a <b>null</b> reference.
        /// </param>
        /// <returns>
        ///     The source collection if it is not <b>null</b>; otherwise, empty collection.
        /// </returns>
        public static IEnumerable<T> AvoidNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static T[] ToArraySmart<T>(this IEnumerable<T> collection)
        {
            #region Argument Check

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            #endregion

            return collection as T[] ?? collection.ToArray();
        }

        /// <summary>
        ///     Gets an object that can be used to synchronize access to the specified collection.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the collection object.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to get a synchronization object of.
        /// </param>
        /// <returns>
        ///     An object that can be used to synchronize access to the specified collection.
        /// </returns>
        public static object GetSyncRoot<T>(this T collection)
            where T : ICollection
        {
            #region Argument Check

            if (ReferenceEquals(collection, null))
            {
                throw new ArgumentNullException("collection");
            }

            #endregion

            return collection.SyncRoot;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Checks the reference and count equality.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="otherCollection">
        ///     The other collection.
        /// </param>
        /// <returns>
        ///     The result of the check.
        /// </returns>
        private static bool? CheckReferenceAndCountEquality<T>(
            IEnumerable<T> collection,
            IEnumerable<T> otherCollection)
        {
            if (ReferenceEquals(collection, otherCollection))
            {
                return true;
            }

            if (ReferenceEquals(collection, null) || ReferenceEquals(otherCollection, null))
            {
                return false;
            }

            var collectionFastCount = GetFastCount(collection);
            var otherCollectionFastCount = GetFastCount(otherCollection);
            if (collectionFastCount.HasValue
                && otherCollectionFastCount.HasValue
                && collectionFastCount.Value != otherCollectionFastCount.Value)
            {
                return false;
            }

            return null;
        }

        /// <summary>
        ///     Creates the count map.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        /// <param name="comparer">
        ///     The comparer.
        /// </param>
        /// <returns>
        ///     The count map.
        /// </returns>
        private static Dictionary<T, int> CreateCountMap<T>(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            return collection
                .GroupBy(item => item, comparer)
                .Select(group => new KeyValuePair<T, int>(group.Key, group.Count()))
                .ToDictionary(item => item.Key, item => item.Value, comparer);
        }

        #endregion
    }
}