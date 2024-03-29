﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Omnifactotum;

/// <summary>
///     Represents the equality comparer that compares objects of the specified type by their references.
/// </summary>
/// <typeparam name="T">
///     The type of objects to compare.
/// </typeparam>
public sealed class ByReferenceEqualityComparer<T> : IEqualityComparer<T>
    where T : class
{
    private ByReferenceEqualityComparer()
    {
        // Nothing to do
    }

    /// <summary>
    ///     Gets the sole instance of the <see cref="ByReferenceEqualityComparer{T}"/> class.
    /// </summary>
    public static ByReferenceEqualityComparer<T> Instance { get; } = new();

    /// <summary>
    ///     Determines whether the specified objects are equal by reference.
    /// </summary>
    /// <param name="x">
    ///     The first object of type <typeparamref name="T"/> to compare.
    /// </param>
    /// <param name="y">
    ///     The second object of type <typeparamref name="T"/> to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified objects are equal by reference; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

    /// <summary>
    ///     Returns a hash code for the specified object, based on object's reference.
    /// </summary>
    /// <param name="obj">
    ///     The object for which a hash code is to be returned.
    /// </param>
    /// <returns>
    ///     A hash code for the specified object, based on object's reference.
    /// </returns>
    public int GetHashCode(T? obj) => obj is null ? 0 : RuntimeHelpers.GetHashCode(obj);
}