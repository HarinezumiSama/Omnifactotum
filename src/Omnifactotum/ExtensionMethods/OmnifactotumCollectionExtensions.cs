using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Collections.Generic;

/// <summary>
///     Contains extension methods for collections, that is, for <see cref="IEnumerable{T}"/>.
/// </summary>
[SuppressMessage("ReSharper", "UseDeconstruction", Justification = "Multiple target frameworks.")]
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
    public static int? GetFastCount<T>([CanBeNull] [NoEnumeration] this IEnumerable<T>? collection)
        => collection switch
        {
            null => 0,
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

        foreach (var item in collection)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await actionAsync(item, cancellationToken);
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

        var index = 0;
        foreach (var item in collection)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await actionAsync(item, index, cancellationToken);
            index++;
        }
    }

    /// <summary>
    ///     Sets the items in the specified collection to the specified items.
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
    public static void SetItems<T>(
        [NotNull] this ICollection<T> collection,
        [NotNull] [InstantHandle] IEnumerable<T> items)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        collection.Clear();

        if (collection is List<T> list)
        {
            list.AddRange(items);
            return;
        }

        items.DoForEach(collection.Add);
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

        using var enumerator = collection!.GetEnumerator();
        using var otherEnumerator = otherCollection!.GetEnumerator();

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
    ///     A equality comparer used to compare keys.
    /// </param>
    /// <returns>
    ///     A dictionary in which a key is a duplicated key from the source collection keys and a value is
    ///     the list of the corresponding duplicated items from the source collection.
    /// </returns>
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

        var actualComparer = comparer ?? EqualityComparer<TKey>.Default;

        return collection
            .GroupBy(keySelector, actualComparer)
            .Where(group => group.Count() > 1)
            .Select(group => OmnifactotumKeyValuePair.Create(group.Key, group.ToList()))
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
        [CanBeNull] [InstantHandle] this IEnumerable<TDisposable?>? collection)
        where TDisposable : class, IDisposable
    {
        if (collection is null)
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
        [CanBeNull] [InstantHandle] this IEnumerable<TDisposable?>? collection)
        where TDisposable : struct, IDisposable
    {
        if (collection is null)
        {
            return;
        }

        foreach (var item in collection)
        {
            item.DisposeSafely();
        }
    }

    /// <summary>
    ///     Avoids the specified collection to be a <see langword="null"/> reference: returns the specified collection
    ///     if it is not <see langword="null"/> or an empty collection otherwise.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="source">
    ///     The collection to secure from a <see langword="null"/> reference.
    /// </param>
    /// <returns>
    ///     The source collection if it is not <see langword="null"/>; otherwise, empty collection.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static IEnumerable<T> AvoidNull<T>([CanBeNull] [NoEnumeration] this IEnumerable<T>? source)
        => source ?? Enumerable.Empty<T>();

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
    [NotNull]
    [ItemNotNull]
    [LinqTunnel]
    [Pure]
    [System.Diagnostics.Contracts.Pure]
    public static IEnumerable<T> WhereNotNull<T>([NotNull] [ItemCanBeNull] this IEnumerable<T?> source)
        where T : class
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source)
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
    [NotNull]
    [LinqTunnel]
    [Pure]
    [System.Diagnostics.Contracts.Pure]
    public static IEnumerable<T> WhereNotNull<T>([NotNull] this IEnumerable<T?> source)
        where T : struct
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in source)
        {
            if (item is { } itemValue)
            {
                yield return itemValue;
            }
        }
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="HashSet{T}"/> class that uses the specified equality comparer
    ///     for the set type, contains elements copied from the specified collection, and has sufficient capacity
    ///     to accommodate the number of elements copied.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to create a hash set from.
    /// </param>
    /// <param name="comparer">
    ///     The comparer to initialize a hash set with.
    /// </param>
    /// <returns>
    ///     A created hash set.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static HashSet<T> ToHashSet<T>(
        [NotNull] [InstantHandle]
#if NETFRAMEWORK && !NET472_OR_GREATER
        this IEnumerable<T> collection,
#else
        IEnumerable<T> collection,
#endif
        [CanBeNull] IEqualityComparer<T>? comparer)
        => collection is null ? throw new ArgumentNullException(nameof(collection)) : new HashSet<T>(collection, comparer);

    /// <summary>
    ///     Creates a new instance of the <see cref="HashSet{T}"/> class that uses the default equality comparer
    ///     for the set type, contains elements copied from the specified collection, and has sufficient capacity
    ///     to accommodate the number of elements copied.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the collection.
    /// </typeparam>
    /// <param name="collection">
    ///     The collection to create a hash set from.
    /// </param>
    /// <returns>
    ///     A created hash set.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static HashSet<T> ToHashSet<T>(
        [NotNull] [InstantHandle]
#if NETFRAMEWORK && !NET472_OR_GREATER
        this IEnumerable<T> collection
#else
        IEnumerable<T> collection
#endif
    ) => collection is null ? throw new ArgumentNullException(nameof(collection)) : new HashSet<T>(collection);

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
    ///         string[] values;
    ///
    ///         values = null;
    ///         Console.WriteLine("Values are {0}.", values.ToUIString()); // Output: Values are <null>.
    ///
    ///         values = new string[] { null, string.Empty, "Hello", "Class 'MyClass' is found in project \"MyProject\"." };
    ///         Console.WriteLine("Values are {0}.", values.ToUIString()); // Output: Values are null, "", "Hello", "Class 'MyClass' is found in project ""MyProject"".".
    /// ]]>
    ///     </code>
    /// </example>
    [NotNull]
    public static string ToUIString([CanBeNull] [InstantHandle] this IEnumerable<string?>? values)
        => values?.Select(value => value.ToUIString()).Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
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
    [NotNull]
    public static string ToUIString<T>([CanBeNull] [InstantHandle] this IEnumerable<T?>? values)
        where T : struct
        => values?.Select(value => value.ToUIString()).Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
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
    [NotNull]
    public static string ToUIString<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T?>? values,
        [CanBeNull] string? format,
        [CanBeNull] IFormatProvider? formatProvider)
        where T : struct, IFormattable
        => values?.Select(value => value.ToUIString(format, formatProvider)).Join(OmnifactotumRepresentationConstants.CollectionItemSeparator)
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
    [NotNull]
    public static string ToUIString<T>(
        [CanBeNull] [InstantHandle] this IEnumerable<T?>? values,
        [CanBeNull] IFormatProvider? formatProvider)
        where T : struct, IFormattable
        => values.ToUIString(null, formatProvider);

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
    [NotNull]
    public static ReadOnlyCollection<T> AsReadOnly<T>([NotNull] this IList<T> list)
        => list is null ? throw new ArgumentNullException(nameof(list)) : new ReadOnlyCollection<T>(list);

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
    ///         <see cref="!:https://github.com/dotnet/runtime/blob/6fdb82aea93465ee046c7f903a96d5c2027a3ecd/src/libraries/System.Linq/src/System/Linq/Chunk.cs"/>.
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
    public static IEnumerable<TSource[]> Chunk<TSource>([NotNull] this IEnumerable<TSource> source, int size)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), size, @"The value must be greater than zero.");
        }

        return CreateChunkIterator(source, size);
    }

    /// <remarks>
    ///     This code is based on
    ///     <see cref="!:https://github.com/dotnet/runtime/blob/6fdb82aea93465ee046c7f903a96d5c2027a3ecd/src/libraries/System.Linq/src/System/Linq/Chunk.cs"/>.
    /// </remarks>
    private static IEnumerable<TSource[]> CreateChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using var e = source.GetEnumerator();

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

    private static Dictionary<KeyWrapper<T>, int> CreateCountMap<T>(
        [NotNull] [InstantHandle] IEnumerable<T> collection,
        [CanBeNull] IEqualityComparer<T>? comparer,
        [NotNull] IEqualityComparer<KeyWrapper<T>> wrapperComparer)
        => collection
            .GroupBy(item => item, comparer)
            .Select(group => OmnifactotumKeyValuePair.Create(new KeyWrapper<T>(group.Key), group.Count()))
            .ToDictionary(item => item.Key, item => item.Value, wrapperComparer);

    private readonly struct KeyWrapper<T>
    {
        public KeyWrapper(T value) => Value = value;

        public T Value { get; }

        public override bool Equals(object? obj) => throw new NotSupportedException();

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