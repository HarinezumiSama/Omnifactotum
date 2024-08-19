using System;
using System.Collections;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents the equality comparer that uses a comparison key provided by the specified delegate.
/// </summary>
/// <typeparam name="T">
///     The type of the objects to compare.
/// </typeparam>
/// <typeparam name="TKey">
///     The type of the comparison key.
/// </typeparam>
public sealed class KeyedEqualityComparer<T, TKey> : IEqualityComparer<T>, IEqualityComparer
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KeyedEqualityComparer{T,TKey}"/> class
    ///     with the specified key selector and key equality comparer.
    /// </summary>
    /// <param name="keySelector">
    ///     A reference to a method that provides a key for an object being compared.
    /// </param>
    /// <param name="keyComparer">
    ///     The equality comparer used for comparing object keys; or <see langword="null"/> to use the default
    ///     equality comparer for type <typeparamref name="TKey"/>.
    /// </param>
    public KeyedEqualityComparer(
        [NotNull] Func<T, TKey> keySelector,
        [CanBeNull] IEqualityComparer<TKey>? keyComparer = null)
    {
        KeySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
    }

    /// <summary>
    ///     Gets a reference to a method that provides a key for an object being compared.
    /// </summary>
    [NotNull]
    public Func<T, TKey> KeySelector { get; }

    /// <summary>
    ///     Gets the equality comparer used for comparing object keys.
    /// </summary>
    [NotNull]
    public IEqualityComparer<TKey> KeyComparer { get; }

    /// <summary>
    ///     Determines whether the specified objects are equal.
    /// </summary>
    /// <param name="left">
    ///     The first object of type <typeparamref name="T"/> to compare.
    /// </param>
    /// <param name="right">
    ///     The second object of type <typeparamref name="T"/> to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals([CanBeNull] T? left, [CanBeNull] T? right)
    {
        if (typeof(T).IsValueType)
        {
            if (left is null || right is null)
            {
                return left is null && right is null;
            }
        }
        else
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }
        }

        var leftKey = KeySelector(left);
        var rightKey = KeySelector(right);

        return KeyComparer.Equals(leftKey, rightKey);
    }

    /// <summary>
    ///     Returns a hash code for the specified object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="obj">
    ///     The object of type <typeparamref name="T"/> for which a hash code is to be returned.
    /// </param>
    /// <returns>
    ///     A hash code for the specified object of type <typeparamref name="T"/>.
    /// </returns>
    public int GetHashCode([CanBeNull] T? obj)
    {
        if (obj is null)
        {
            return 0;
        }

        var key = KeySelector(obj);
        return key is null ? 0 : KeyComparer.GetHashCode(key);
    }

    /// <summary>
    ///     Determines whether the specified objects are equal.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    /// <param name="left">
    ///     The first object to compare.
    /// </param>
    /// <param name="right">
    ///     The second object to compare.
    /// </param>
    bool IEqualityComparer.Equals([CanBeNull] object? left, [CanBeNull] object? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left is T castLeft && right is T castRight && Equals(castLeft, castRight);
    }

    /// <summary>
    ///     Returns a hash code for the key of the specified object.
    /// </summary>
    /// <param name="obj">
    ///     The object for which key a hash code is to be returned.
    /// </param>
    /// <returns>
    ///     A hash code for the key of the specified object.
    /// </returns>
    int IEqualityComparer.GetHashCode([CanBeNull] object? obj)
        => obj is null
            ? 0
            : obj is T castObj
                ? GetHashCode(castObj)
                : obj.GetHashCode();
}