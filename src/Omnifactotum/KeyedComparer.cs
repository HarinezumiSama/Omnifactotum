using System;
using System.Collections;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents the comparer that uses a comparison key provided by the specified delegate.
/// </summary>
/// <typeparam name="T">
///     The type of the objects to compare.
/// </typeparam>
/// <typeparam name="TKey">
///     The type of the comparison key.
/// </typeparam>
public sealed class KeyedComparer<T, TKey> : IComparer<T>, IComparer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyedComparer{T,TKey}"/> class
    ///     with the specified key selector and key comparer.
    /// </summary>
    /// <param name="keySelector">
    ///     A reference to a method that provides a key for an object being compared.
    /// </param>
    /// <param name="keyComparer">
    ///     The comparer used for comparing object keys; or <see langword="null"/> to use the default
    ///     comparer for type <typeparamref name="TKey"/>.
    /// </param>
    public KeyedComparer(
        [NotNull] Func<T, TKey> keySelector,
        [CanBeNull] IComparer<TKey>? keyComparer = null)
    {
        KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        KeyComparer = keyComparer ?? Comparer<TKey>.Default;
    }

    /// <summary>
    ///     Gets a reference to a method that provides a key for an object being compared.
    /// </summary>
    [NotNull]
    public Func<T, TKey> KeySelector { get; }

    /// <summary>
    ///     Gets the comparer used to compare object keys.
    /// </summary>
    [NotNull]
    public IComparer<TKey> KeyComparer { get; }

    /// <summary>
    ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="left">
    ///     The first object to compare.
    /// </param>
    /// <param name="right">
    ///     The second object to compare.
    /// </param>
    /// <returns>
    ///     <para>
    ///         A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>Meaning</description>
    ///         </listheader>
    ///         <item>
    ///             <term>Less than zero</term>
    ///             <description><paramref name="left"/> is less than <paramref name="right"/></description>
    ///         </item>
    ///         <item>
    ///             <term>Zero</term>
    ///             <description><paramref name="left"/> equals <paramref name="right"/></description>
    ///         </item>
    ///         <item>
    ///             <term>Greater than zero</term>
    ///             <description><paramref name="left"/> is greater than <paramref name="right"/></description>
    ///         </item>
    ///     </list>
    /// </returns>
    public int Compare([CanBeNull] T? left, [CanBeNull] T? right)
        => ReferenceEquals(left, right) ? 0 : left is null ? -1 : right is null ? 1 : CompareInternal(left, right);

    /// <summary>
    ///     Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="left">
    ///     The first object to compare.
    /// </param>
    /// <param name="right">
    ///     The second object to compare.
    /// </param>
    /// <returns>
    ///     <para>
    ///         A signed integer that indicates the relative values of <paramref name="left" /> and <paramref name="right" />.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Value</term>
    ///             <description>Meaning</description>
    ///         </listheader>
    ///         <item>
    ///             <term>Less than zero</term>
    ///             <description><paramref name="left"/> is less than <paramref name="right"/></description>
    ///         </item>
    ///         <item>
    ///             <term>Zero</term>
    ///             <description><paramref name="left"/> equals <paramref name="right"/></description>
    ///         </item>
    ///         <item>
    ///             <term>Greater than zero</term>
    ///             <description><paramref name="left"/> is greater than <paramref name="right"/></description>
    ///         </item>
    ///     </list>
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="left"/> or <paramref name="right"/> is not assignable to type <typeparamref name="T"/>.
    /// </exception>
    int IComparer.Compare([CanBeNull] object? left, [CanBeNull] object? right)
    {
        if (ReferenceEquals(left, right))
        {
            return 0;
        }

        if (left is null)
        {
            return -1;
        }

        if (left is not T castX)
        {
            throw new ArgumentException($"Invalid value type {left.GetType().GetFullName().ToUIString()}.", nameof(left));
        }

        if (right is null)
        {
            return 1;
        }

        if (right is not T castY)
        {
            throw new ArgumentException($"Invalid value type {right.GetType().GetFullName().ToUIString()}.", nameof(right));
        }

        return CompareInternal(castX, castY);
    }

    private int CompareInternal(T left, T right)
    {
        var leftKey = KeySelector(left);
        var rightKey = KeySelector(right);

        return KeyComparer.Compare(leftKey, rightKey);
    }
}