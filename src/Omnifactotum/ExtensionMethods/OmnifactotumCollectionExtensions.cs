using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnifactotum;
using Omnifactotum.Annotations;
using ExcludeFromCodeCoverageAttribute = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;

#if !NET7_0_OR_GREATER
using System.Collections.ObjectModel;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Collections.Generic;

/// <summary>
///     Contains extension methods for collections, that is, for <see cref="IEnumerable{T}"/>, <see cref="ICollection{T}"/>, <see cref="ICollection"/>,
///     <see cref="IList{T}"/>, <see cref="IAsyncEnumerable{T}"/>, <see cref="ConfiguredCancelableAsyncEnumerable{T}"/> etc.
/// </summary>
public static class OmnifactotumCollectionExtensions
{
    /// <summary>
    ///     Get the number of elements in the specified collection without enumerating all
    ///     its elements. If it's not possible to do so, <see langword="null"/> is returned.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to get the number of elements in.
    /// </param>
    /// <returns>
    ///     The number of elements in the specified collection if it was possible to determine it without
    ///     enumerating collection's elements; otherwise, <see langword="null"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static int? GetFastCount<T>([CanBeNull] [NoEnumeration] this IEnumerable<T>? collection)
        => collection switch
        {
            null => 0,
            ImmutableArray<T> { IsDefault: true } => 0,
            ////var enumerable => enumerable.TryGetNonEnumeratedCount(...),
            ICollection<T> castCollection => castCollection.Count,
            ICollection castCollection => castCollection.Count,
            _ => null
        };

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
    ///     <para><paramref name="collection"/> is <see langword="null"/>.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="action"/> is <see langword="null"/>.</para>
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static void DoForEach<T>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Action<T> action)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (collection.IsDefaultImmutableArray())
        {
            return;
        }

        foreach (var item in collection)
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
    ///     <para><paramref name="collection"/> is <see langword="null"/>.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="action"/> is <see langword="null"/>.</para>
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static void DoForEach<T>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Action<T, int> action)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (collection.IsDefaultImmutableArray())
        {
            return;
        }

        var index = 0;
        foreach (var item in collection)
        {
            action(item, index);
            index++;
        }
    }

    /// <summary>
    ///     Performs the specified asynchronous action for each element of the collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to perform an action for.
    /// </param>
    /// <param name="actionAsync">
    ///     A reference to a method representing the asynchronous action to perform on an item;
    ///     the first parameter is the item to perform the action on;
    ///     the second parameter is the token to be monitored for cancellation requests.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     <para><paramref name="collection"/> is <see langword="null"/>.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="actionAsync"/> is <see langword="null"/>.</para>
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static async Task DoForEachAsync<T>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Func<T, CancellationToken, Task> actionAsync,
        CancellationToken cancellationToken = default)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (actionAsync is null)
        {
            throw new ArgumentNullException(nameof(actionAsync));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (collection.IsDefaultImmutableArray())
        {
            return;
        }

        foreach (var item in collection)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await actionAsync(item, cancellationToken).ConfigureAwaitNoCapturedContext();
        }
    }

    /// <summary>
    ///     Performs the specified asynchronous action for each element of the collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to perform an action for.
    /// </param>
    /// <param name="actionAsync">
    ///     A reference to a method representing the asynchronous action to perform on an item;
    ///     the first parameter is the item to perform the action on;
    ///     the second parameter is the zero-based index of the element in the collection.
    ///     the third parameter is the token to be monitored for cancellation requests.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     <para><paramref name="collection"/> is <see langword="null"/>.</para>
    ///     <para>-or-</para>
    ///     <para><paramref name="actionAsync"/> is <see langword="null"/>.</para>
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static async Task DoForEachAsync<T>(
        [NotNull] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Func<T, int, CancellationToken, Task> actionAsync,
        CancellationToken cancellationToken = default)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (actionAsync is null)
        {
            throw new ArgumentNullException(nameof(actionAsync));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (collection.IsDefaultImmutableArray())
        {
            return;
        }

        var index = 0;
        foreach (var item in collection)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await actionAsync(item, index, cancellationToken).ConfigureAwaitNoCapturedContext();
            index++;
        }
    }

    /// <summary>
    ///     [OBSOLETE] Sets the items in the specified collection to the specified items.
    ///     The previously contained items are removed from the collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to set the items of.
    /// </param>
    /// <param name="items">
    ///     The items to put to the collection.
    /// </param>
    [Obsolete($"Use '{nameof(OmnifactotumCollectionExtensions)}.{nameof(ReplaceItems)}' instead.")]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static void SetItems<T>(
        [NotNull] this ICollection<T> collection,
        [NotNull] [InstantHandle] IEnumerable<T> items)
        => collection.ReplaceItems(items);

    /// <summary>
    ///     Replaces the items in the collection with the specified items.
    /// </summary>
    /// <typeparam name="TCollection">
    ///     The type of the collection to replace the items in.
    /// </typeparam>
    /// <typeparam name="T">
    ///     The type of the elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to replace the items in.
    /// </param>
    /// <param name="newItems">
    ///     The items to put to the collection.
    /// </param>
    /// <returns>
    ///     The original collection (the same reference as the <paramref name="collection"/> parameter).
    /// </returns>
    /// <remarks>
    ///     The previously contained items are removed from the collection, and the new items are added to it.
    /// </remarks>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         var list = new List<int>() { 1, 2, 3 };
    ///         // `list` contains 1, 2, and 3.
    ///
    ///         list.ReplaceItems(new [] { 6, 5, 4 });
    ///         // `list` contains 6, 5, and 4.
    /// ]]>
    ///     </code>
    /// </example>
    public static TCollection ReplaceItems<TCollection, T>(
        [NotNull] this TCollection collection,
        [NotNull] [InstantHandle] IEnumerable<T> newItems)
        where TCollection : ICollection<T>
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (newItems is null)
        {
            throw new ArgumentNullException(nameof(newItems));
        }

        collection.Clear();

        switch (collection)
        {
            case List<T> list:
                list.AddRange(newItems);
                break;

            default:
                newItems.DoForEach(collection.Add);
                break;
        }

        return collection;
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
    ///     collections, or <see langword="null"/> to use the default equality comparer for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if two specified collections contain identical items in any order or
    ///     they both are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool CollectionsEquivalent<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T>? collection,
        [CanBeNull] [InstantHandle] IEnumerable<T>? otherCollection,
        [CanBeNull] IEqualityComparer<T>? comparer)
    {
        var fastResult = CheckReferenceAndCountEquality(collection, otherCollection);
        if (fastResult.HasValue)
        {
            return fastResult.Value;
        }

        var wrapperComparer = new KeyWrapperEqualityComparer<T>(comparer);

        var map = CreateCountMap(collection!, comparer, wrapperComparer);
        var otherMap = CreateCountMap(otherCollection!, comparer, wrapperComparer);

        if (map.Count != otherMap.Count)
        {
            return false;
        }

        foreach (var pair in map)
        {
            if (!otherMap.TryGetValue(pair.Key, out var otherCount) || otherCount != pair.Value)
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
    ///     <see langword="true"/> if two specified collections contain identical items in any order or
    ///     they both are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool CollectionsEquivalent<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T>? collection,
        [CanBeNull] [InstantHandle] IEnumerable<T>? otherCollection)
        => CollectionsEquivalent(collection, otherCollection, null);

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
    ///     collections, or <see langword="null"/> to use the default equality comparer for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if two specified collections contain identical items in the same order or
    ///     they both are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool CollectionsEqual<T>(
        [CanBeNull] this IEnumerable<T>? collection,
        [CanBeNull] IEnumerable<T>? otherCollection,
        [CanBeNull] IEqualityComparer<T>? comparer)
    {
        var fastResult = CheckReferenceAndCountEquality(collection, otherCollection);
        if (fastResult.HasValue)
        {
            return fastResult.Value;
        }

        var actualComparer = comparer ?? EqualityComparer<T>.Default;

        using var enumerator = collection!.AvoidDefaultImmutableArray().GetEnumerator();
        using var otherEnumerator = otherCollection!.AvoidDefaultImmutableArray().GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (!otherEnumerator.MoveNext()
                || !actualComparer.Equals(enumerator.Current, otherEnumerator.Current))
            {
                return false;
            }
        }

        return !otherEnumerator.MoveNext();
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
    ///     <see langword="true"/> if two specified collections contain identical items in the same order or
    ///     they both are <see langword="null"/>; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool CollectionsEqual<T>(
        [CanBeNull] this IEnumerable<T>? collection,
        [CanBeNull] IEnumerable<T>? otherCollection)
        => CollectionsEqual(collection, otherCollection, null);

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
    ///     An equality comparer used to compare keys.
    /// </param>
    /// <returns>
    ///     A dictionary in which a key is a duplicated key from the source collection keys and a value is
    ///     the list of the corresponding duplicated items from the source collection.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static Dictionary<TKey, List<T>> FindDuplicates<T, TKey>(
        [NotNull] [InstantHandle] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Func<T, TKey> keySelector,
        [CanBeNull] IEqualityComparer<TKey>? comparer)
        where TKey : notnull
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (keySelector is null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        var resolvedComparer = comparer ?? EqualityComparer<TKey>.Default;

        return collection
            .AvoidDefaultImmutableArray()
            .GroupBy(keySelector, resolvedComparer)
            .Select(static group => KeyValuePair.Create(group.Key, group.ToList()))
            .Where(static group => group.Value.Count > 1)
            .ToDictionary(static item => item.Key, static item => item.Value, resolvedComparer);
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static Dictionary<TKey, List<T>> FindDuplicates<T, TKey>(
        [NotNull] [InstantHandle] this IEnumerable<T> collection,
        [NotNull] [InstantHandle] Func<T, TKey> keySelector)
        where TKey : notnull
        => FindDuplicates(collection, keySelector, null);

    /// <summary>
    ///     Safely disposes of each element in the specified collection.
    ///     If the collection is <see langword="null"/>, nothing is done.
    /// </summary>
    /// <typeparam name="TDisposable">
    ///     The type of the disposable elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     A collection of disposable elements.
    /// </param>
    /// <seealso cref="OmnifactotumDisposableExtensions.DisposeSafely{T}(T)"/>
    public static void DisposeCollectionItemsSafely<TDisposable>(
        [CanBeNull] [ItemCanBeNull] [InstantHandle] this IEnumerable<TDisposable?>? collection)
        where TDisposable : class, IDisposable
    {
        if (collection is null || collection.IsDefaultImmutableArray())
        {
            return;
        }

        foreach (var item in collection)
        {
            item.DisposeSafely();
        }
    }

    /// <summary>
    ///     Safely disposes of each element in the specified collection.
    ///     If the collection is <see langword="null"/>, nothing is done.
    /// </summary>
    /// <typeparam name="TDisposable">
    ///     The type of the disposable elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     A collection of disposable elements.
    /// </param>
    /// <seealso cref="OmnifactotumDisposableExtensions.DisposeSafely{T}(System.Nullable{T})"/>
    public static void DisposeCollectionItemsSafely<TDisposable>(
        [CanBeNull] [ItemCanBeNull] [InstantHandle] this IEnumerable<TDisposable?>? collection)
        where TDisposable : struct, IDisposable
    {
        if (collection is null || collection.IsDefaultImmutableArray())
        {
            return;
        }

        foreach (var item in collection)
        {
            item.DisposeSafely();
        }
    }

    /// <summary>
    ///     Avoids the specified collection being a <see langword="null"/> reference.
    ///     Returns the specified collection if it is not <see langword="null"/>; otherwise, returns an empty collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="source">
    ///     The collection to secure from a <see langword="null"/> reference.
    /// </param>
    /// <returns>
    ///     The source collection if it is not <see langword="null"/>; otherwise, an empty collection.
    /// </returns>
    [Obsolete($"Use '{nameof(OmnifactotumCollectionExtensions)}.{nameof(EmptyIfNull)}()' instead.")]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static IEnumerable<T> AvoidNull<T>([CanBeNull] [NoEnumeration] this IEnumerable<T>? source) => EmptyIfNull(source);

    /// <summary>
    ///     Returns an empty collection if the specified source collection is <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>;
    ///     otherwise, returns the specified source collection.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="source">
    ///     The source collection to handle.
    /// </param>
    /// <returns>
    ///     An empty collection if the specified source collection is <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>;
    ///     otherwise, returns the specified source collection.
    /// </returns>
    /// <seealso cref="ImmutableArray{T}.IsDefault"/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    [SuppressMessage("ReSharper", "UseCollectionExpression")]
    public static IEnumerable<T> EmptyIfNull<T>([CanBeNull] [NoEnumeration] this IEnumerable<T>? source)
        => source is null || source.IsDefaultImmutableArray() ? Enumerable.Empty<T>() : source;

    /// <summary>
    ///     Filters a sequence of nullable reference type elements and returns only those elements that are not <see langword="null"/>.
    /// </summary>
    /// <param name="source">
    ///     An <see cref="IEnumerable{T}" /> to filter.
    /// </param>
    /// <typeparam name="T">
    ///     The reference type that defines the corresponding nullable reference type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <returns>
    ///     A sequence containing only those elements from <paramref name="source" /> that are not <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" /> is <see langword="null" />.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [LinqTunnel]
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<T> WhereNotNull<T>([NotNull] [ItemCanBeNull] this IEnumerable<T?> source)
        where T : class
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source.AvoidDefaultImmutableArray())
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    /// <summary>
    ///     Filters a sequence of nullable value type elements and returns only those elements that are not <see langword="null"/>.
    /// </summary>
    /// <param name="source">
    ///     An <see cref="IEnumerable{T}" /> to filter.
    /// </param>
    /// <typeparam name="T">
    ///     The value type that defines the corresponding nullable value type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <returns>
    ///     A sequence containing only those elements from <paramref name="source" /> that are not <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source" /> is <see langword="null" />.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [LinqTunnel]
    [NotNull]
    public static IEnumerable<T> WhereNotNull<T>([NotNull] this IEnumerable<T?> source)
        where T : struct
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source.AvoidDefaultImmutableArray())
        {
            if (item is { } itemValue)
            {
                yield return itemValue;
            }
        }
    }

    /// <summary>
    ///     Gets an object that can be used to synchronize access to the specified collection.
    /// </summary>
    /// <param name="collection">
    ///     The collection to get a synchronization object of.
    /// </param>
    /// <returns>
    ///     An object that can be used to synchronize access to the specified collection.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static object GetSyncRoot([NotNull] [NoEnumeration] this ICollection collection)
        => collection is null ? throw new ArgumentNullException(nameof(collection)) : collection.SyncRoot;

    /// <summary>
    ///     <para>
    ///         Converts the specified collection of string values to its UI representation.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal: <b>&lt;null&gt;</b></description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 A string value containing UI representations of each item in the
    ///                 collection separated with comma and whitespace. (See <see cref="OmnifactotumStringExtensions.ToUIString"/>.)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="values">
    ///     The collection of string values to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified collection of string values.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         string[] values1 = null;
    ///         Console.WriteLine("Values are {0}.", values1.ToUIString()); // Output: Values are <null>.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         var values2 = new string[] { null, string.Empty, "Hello", "Class 'MyClass' is found in project \"MyProject\"." };
    ///         Console.WriteLine("Values are {0}.", values2.ToUIString()); // Output: Values are null, "", "Hello", "Class 'MyClass' is found in project ""MyProject"".".
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString([CanBeNull] [InstantHandle] this IEnumerable<string?>? values)
        => values?.AvoidDefaultImmutableArray().Select(value => value.ToUIString()).Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
            ?? OmnifactotumRepresentationConstants.NullCollectionRepresentation;

    /// <summary>
    ///     <para>
    ///         Converts the specified collection of nullable values to its UI representation.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal: <b>&lt;null&gt;</b></description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 A string value containing UI representations of each item in the
    ///                 collection separated with comma and whitespace. (See <see cref="OmnifactotumGenericObjectExtensions.ToUIString{T}(System.Nullable{T})"/>.)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <typeparam name="T">
    ///     The underlying value type of the nullable values in the collection.
    /// </typeparam>
    /// <param name="values">
    ///     The collection of nullable values to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified collection of nullable values.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         int?[] values;
    ///
    ///         values = null;
    ///         Console.WriteLine("Values are {0}.", values.ToUIString()); // Output: Values are <null>.
    ///
    ///         values = new int?[] { null, 42 };
    ///         Console.WriteLine("Values are {0}.", values.ToUIString()); // Output: Values are null, 42.
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString<T>([CanBeNull] [InstantHandle] this IEnumerable<T?>? values)
        where T : struct
        => values?.AvoidDefaultImmutableArray().Select(value => value.ToUIString()).Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
            ?? OmnifactotumRepresentationConstants.NullCollectionRepresentation;

    /// <summary>
    ///     <para>
    ///         Converts the specified collection of nullable values to its UI representation
    ///         using the specified format and format provider.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal: <b>&lt;null&gt;</b></description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 A string value containing UI representations of each item in the
    ///                 collection separated with comma and whitespace. (See <see cref="OmnifactotumGenericObjectExtensions.ToUIString{T}(System.Nullable{T},string,System.IFormatProvider)"/>.)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <typeparam name="T">
    ///     The underlying value type of the nullable values in the collection.
    /// </typeparam>
    /// <param name="values">
    ///     The collection of nullable values to convert.
    /// </param>
    /// <param name="format">
    ///     The format to use, or <see langword="null"/> to use the default format defined for the type <typeparamref name="T"/>.
    /// </param>
    /// <param name="formatProvider">
    ///     The provider to use to format the value, or <see langword="null"/> to obtain the format
    ///     information from the current locale setting of the operating system.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified collection of nullable values.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T?>? values,
        [CanBeNull] string? format,
        [CanBeNull] IFormatProvider? formatProvider)
        where T : struct, IFormattable
        => values?
                .AvoidDefaultImmutableArray()
                .Select(value => value.ToUIString(format, formatProvider))
                .Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
            ?? OmnifactotumRepresentationConstants.NullCollectionRepresentation;

    /// <summary>
    ///     <para>
    ///         Converts the specified collection of nullable values to its UI representation
    ///         using the specified format provider.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal: <b>&lt;null&gt;</b></description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 A string value containing UI representations of each item in the
    ///                 collection separated with comma and whitespace. (See <see cref="OmnifactotumGenericObjectExtensions.ToUIString{T}(System.Nullable{T},System.IFormatProvider)"/>.)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <typeparam name="T">
    ///     The underlying value type of the nullable values in the collection.
    /// </typeparam>
    /// <param name="values">
    ///     The collection of nullable values to convert.
    /// </param>
    /// <param name="formatProvider">
    ///     The provider to use to format the value, or <see langword="null"/> to obtain the format
    ///     information from the current locale setting of the operating system.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified collection of nullable values.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T?>? values,
        [CanBeNull] IFormatProvider? formatProvider)
        where T : struct, IFormattable
        => values.ToUIString(null, formatProvider);

    /// <summary>
    ///     <para>
    ///         Converts the specified collection of string key/value pairs to its UI representation.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>The input value</term>
    ///             <description>The result of the method</description>
    ///         </listheader>
    ///         <item>
    ///             <term><see langword="null"/></term>
    ///             <description>The literal: <b>&lt;null&gt;</b></description>
    ///         </item>
    ///         <item>
    ///             <term>not <see langword="null"/></term>
    ///             <description>
    ///                 A string value containing UI representations of each key/value pair in the
    ///                 collection separated with comma and whitespace. (See <see cref="OmnifactotumStringExtensions.ToUIString"/>.)
    ///             </description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="pairs">
    ///     The collection of string key/value pairs to convert.
    /// </param>
    /// <returns>
    ///     The UI representation of the specified collection of string key/value pairs.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         Dictionary<string, string?> nullDictionary = null;
    ///         Console.WriteLine("Values - {0}", nullDictionary.ToUIString()); // Output: Values - <null>
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         var dictionary = new Dictionary<string, string?> { { "Qwe", null }, { "asD", "zXc" }, { "uiOp", string.Empty } };
    ///         Console.WriteLine("Values - {0}", dictionary.ToUIString()); // Output: Values - [{ "Qwe": null }, { "asD": "zXc" }, { "uiOp": "" }]
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToUIString(this IEnumerable<KeyValuePair<string, string?>>? pairs)
    {
        if (pairs is null)
        {
            return OmnifactotumRepresentationConstants.NullCollectionRepresentation;
        }

        var resultBuilder = new StringBuilder()
            .Append('[');

        var addSeparator = false;
        foreach (var pair in pairs.AvoidDefaultImmutableArray())
        {
            if (addSeparator)
            {
                resultBuilder.Append(OmnifactotumRepresentationConstants.CollectionItemSeparator);
            }

            resultBuilder
                .Append("{\x0020")
                .Append(pair.Key.ToUIString())
                .Append(":\x0020")
                .Append(pair.Value.ToUIString())
                .Append("\x0020}");

            addSeparator = true;
        }

        resultBuilder.Append(']');

        return resultBuilder.ToString();
    }

#if !NET7_0_OR_GREATER

    /// <summary>
    ///     Creates a read-only wrapper for the specified list.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of values in the list.
    /// </typeparam>
    /// <param name="list">
    ///     The list to create a read-only wrapper for.
    /// </param>
    /// <returns>
    ///     A read-only wrapper for the specified list.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static ReadOnlyCollection<T> AsReadOnly<T>([NotNull] this IList<T> list)
        => list is null
            ? throw new ArgumentNullException(nameof(list))
            : new ReadOnlyCollection<T>(list.IsDefaultImmutableArray() ? Array.Empty<T>() : list);

#endif

#if !NET6_0_OR_GREATER

    /// <summary>
    ///     Splits the elements of a sequence into chunks of size at most <paramref name="size"/>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Every chunk except the last will be of size <paramref name="size"/>.
    ///         The last chunk will contain the remaining elements and may be of a smaller size.
    ///     </para>
    ///     <para>
    ///         This code is based on
    ///         <see href="https://github.com/dotnet/runtime/blob/6fdb82aea93465ee046c7f903a96d5c2027a3ecd/src/libraries/System.Linq/src/System/Linq/Chunk.cs"/>.
    ///     </para>
    /// </remarks>
    /// <param name="source">
    ///     An <see cref="IEnumerable{T}"/> whose elements to chunk.
    /// </param>
    /// <param name="size">
    ///     Maximum size of each chunk.
    /// </param>
    /// <typeparam name="TSource">
    ///     The type of the elements of source.
    /// </typeparam>
    /// <returns>
    ///     An <see cref="IEnumerable{T}"/> that contains the elements the input sequence split into chunks of size <paramref name="size"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="size"/> is less than or equal to 0.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static IEnumerable<TSource[]> Chunk<TSource>([NotNull] this IEnumerable<TSource> source, int size)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), size, "The value must be greater than zero.");
        }

        return CreateChunkIterator(source, size);
    }

#endif

    /// <summary>
    ///     Configures an async iteration so that the awaits on the tasks returned from this iteration do not attempt
    ///     to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="source">
    ///     The source <see cref="IAsyncEnumerable{T}"/> to iterate.
    /// </param>
    /// <returns>
    ///     The configured enumerable.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static ConfiguredCancelableAsyncEnumerable<T> ConfigureAwaitNoCapturedContext<T>([NotNull] this IAsyncEnumerable<T> source)
        => (source ?? throw new ArgumentNullException(nameof(source))).ConfigureAwait(false);

    /// <summary>
    ///     Configures an async iteration so that the awaits on the tasks returned from this iteration do not attempt
    ///     to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="source">
    ///     The source <see cref="ConfiguredCancelableAsyncEnumerable{T}"/> to iterate.
    /// </param>
    /// <returns>
    ///     The configured enumerable.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static ConfiguredCancelableAsyncEnumerable<T> ConfigureAwaitNoCapturedContext<T>(this in ConfiguredCancelableAsyncEnumerable<T> source)
        => source.ConfigureAwait(false);

    /// <summary>
    ///     Asynchronously enumerates the specified <see cref="IAsyncEnumerable{T}"/> and returns a <see cref="List{T}"/> containing the elements yielded.
    /// </summary>
    /// <param name="source">
    ///     The source collection to enumerate.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the objects to iterate.
    /// </typeparam>
    /// <returns>
    ///     A task whose result is a <see cref="List{T}"/> containing the elements yielded.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static async Task<List<T>> EnumerateToListAsync<T>([NotNull] this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var result = new List<T>();
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwaitNoCapturedContext())
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.Add(item);
        }

        return result;
    }

    /// <summary>
    ///     Asynchronously enumerates the specified <see cref="IAsyncEnumerable{T}"/> and returns an array containing the elements yielded.
    /// </summary>
    /// <param name="source">
    ///     The source collection to enumerate.
    /// </param>
    /// <param name="cancellationToken">
    ///     The token to monitor for cancellation requests.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the objects to iterate.
    /// </typeparam>
    /// <returns>
    ///     A task whose result is an array containing the elements yielded.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static async Task<T[]> EnumerateToArrayAsync<T>([NotNull] this IAsyncEnumerable<T> source, CancellationToken cancellationToken = default)
        => (await source.EnumerateToListAsync(cancellationToken)).ToArray();

    /// <summary>
    ///     Flattens the specified sequence of sequences into one sequence.
    /// </summary>
    /// <param name="collections">
    ///     A sequence of sequences to flatten.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the elements in each sequence in the <paramref name="collections"/> parameter.
    /// </typeparam>
    /// <returns>
    ///     An <see cref="IEnumerable{T}"/> whose elements are the result of flattening the specified sequence of sequences into one sequence.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [LinqTunnel]
    [NotNull]
    public static IEnumerable<T> Flatten<T>([NotNull] this IEnumerable<IEnumerable<T>> collections)
    {
        if (collections is null)
        {
            throw new ArgumentNullException(nameof(collections));
        }

        return collections.AvoidDefaultImmutableArray().SelectMany(enumerable => enumerable.AvoidDefaultImmutableArray());
    }

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
    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static bool? CheckReferenceAndCountEquality<T>(
        [CanBeNull] [NoEnumeration] IEnumerable<T>? collection,
        [CanBeNull] [NoEnumeration] IEnumerable<T>? otherCollection)
    {
        if (ReferenceEquals(collection, otherCollection))
        {
            return true;
        }

        if (collection is null || otherCollection is null)
        {
            return false;
        }

        if (GetFastCount(collection) is { } count && GetFastCount(otherCollection) is { } otherCount && count != otherCount)
        {
            return false;
        }

        return null;
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static Dictionary<KeyWrapper<T>, int> CreateCountMap<T>(
        [NotNull] [InstantHandle] IEnumerable<T> collection,
        [CanBeNull] IEqualityComparer<T>? comparer,
        [NotNull] IEqualityComparer<KeyWrapper<T>> wrapperComparer)
        => collection
            .AvoidDefaultImmutableArray()
            .GroupBy(item => item, comparer)
            .Select(group => KeyValuePair.Create(new KeyWrapper<T>(group.Key), group.Count()))
            .ToDictionary(item => item.Key, item => item.Value, wrapperComparer);

#if !NET6_0_OR_GREATER

    /// <remarks>
    ///     This code is based on
    ///     <see href="https://github.com/dotnet/runtime/blob/6fdb82aea93465ee046c7f903a96d5c2027a3ecd/src/libraries/System.Linq/src/System/Linq/Chunk.cs"/>.
    /// </remarks>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static IEnumerable<TSource[]> CreateChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using var e = source.AvoidDefaultImmutableArray().GetEnumerator();

        while (e.MoveNext())
        {
            var chunk = new TSource[size];
            chunk[0] = e.Current;

            var i = 1;
            for (; i < chunk.Length && e.MoveNext(); i++)
            {
                chunk[i] = e.Current;
            }

            if (i == chunk.Length)
            {
                yield return chunk;
            }
            else
            {
                Array.Resize(ref chunk, i);
                yield return chunk;

                yield break;
            }
        }
    }

#endif

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    internal static bool IsDefaultImmutableArray<T>([NoEnumeration] this IEnumerable<T> collection) => collection is ImmutableArray<T> { IsDefault: true };

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    internal static IEnumerable<T> AvoidDefaultImmutableArray<T>([NoEnumeration] this IEnumerable<T> collection)
        => collection is ImmutableArray<T> { IsDefault: true } ? Enumerable.Empty<T>() : collection;

    private readonly struct KeyWrapper<T>
    {
        public KeyWrapper(T value) => Value = value;

        public T Value { get; }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object? obj) => throw new NotSupportedException();

        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => throw new NotSupportedException();
    }

    private sealed class KeyWrapperEqualityComparer<T> : IEqualityComparer<KeyWrapper<T>>
    {
        public KeyWrapperEqualityComparer(IEqualityComparer<T>? equalityComparer)
            => EqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;

        private IEqualityComparer<T> EqualityComparer { get; }

        public bool Equals(KeyWrapper<T> left, KeyWrapper<T> right) => EqualityComparer.Equals(left.Value, right.Value);

        public int GetHashCode(KeyWrapper<T> obj) => obj.Value is null ? 0 : EqualityComparer.GetHashCode(obj.Value);
    }
}