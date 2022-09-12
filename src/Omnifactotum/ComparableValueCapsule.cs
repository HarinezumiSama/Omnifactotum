using System;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     <para>
///         Represents the abstract immutable container that encapsulates a strongly-typed value and supports
///         comparison.
///     </para>
///     <para>
///         <b>IMPORTANT</b> note for inheritors: a derived class MUST NOT add any new fields or properties
///         influencing equality comparison.
///     </para>
/// </summary>
/// <typeparam name="T">
///     The type of an encapsulated value.
/// </typeparam>
public abstract class ComparableValueCapsule<T> : EquatableValueCapsule<T>, IComparable<ComparableValueCapsule<T>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ComparableValueCapsule{T}"/> class
    ///     using the specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to initialize this instance with.
    /// </param>
    protected ComparableValueCapsule(T value)
        : base(value)
    {
        // Nothing to do
    }

    /// <summary>
    ///     Gets the <see cref="IComparer{T}"/> used for comparing contained values.
    /// </summary>
    [DebuggerNonUserCode]
    [NotNull]
    public IComparer<T> ValueComparer => GetValueComparer().EnsureNotNull();

    /// <summary>
    ///     Determines whether the left <see cref="ComparableValueCapsule{T}"/> instance is less than
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the left <see cref="ComparableValueCapsule{T}"/> instance is less than
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(
        [CanBeNull] ComparableValueCapsule<T>? left,
        [CanBeNull] ComparableValueCapsule<T>? right)
        => Comparer<ComparableValueCapsule<T>>.Default.Compare(left!, right!) < 0;

    /// <summary>
    ///     Determines whether the left <see cref="ComparableValueCapsule{T}"/> instance is less than or equal
    ///     to the right <see cref="ComparableValueCapsule{T}"/> instance.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the left <see cref="ComparableValueCapsule{T}"/> instance is less than or equal to
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(
        [CanBeNull] ComparableValueCapsule<T>? left,
        [CanBeNull] ComparableValueCapsule<T>? right)
        => Comparer<ComparableValueCapsule<T>>.Default.Compare(left!, right!) <= 0;

    /// <summary>
    ///     Determines whether the left <see cref="ComparableValueCapsule{T}"/> instance is greater than
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the left <see cref="ComparableValueCapsule{T}"/> instance is greater than
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(
        [CanBeNull] ComparableValueCapsule<T>? left,
        [CanBeNull] ComparableValueCapsule<T>? right)
        => Comparer<ComparableValueCapsule<T>>.Default.Compare(left!, right!) > 0;

    /// <summary>
    ///     Determines whether the left <see cref="ComparableValueCapsule{T}"/> instance is greater than or equal
    ///     to the right <see cref="ComparableValueCapsule{T}"/> instance.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ComparableValueCapsule{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the left <see cref="ComparableValueCapsule{T}"/> instance is greater than or equal to
    ///     the right <see cref="ComparableValueCapsule{T}"/> instance; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(
        [CanBeNull] ComparableValueCapsule<T>? left,
        [CanBeNull] ComparableValueCapsule<T>? right)
        => Comparer<ComparableValueCapsule<T>>.Default.Compare(left!, right!) >= 0;

    /// <summary>
    ///     Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">
    ///     An object to compare with this object.
    ///     </param>
    /// <returns>
    ///     A value that indicates the relative order of the objects being compared.
    ///     See <see cref="IComparable{T}.CompareTo"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     The <paramref name="other"/> object's type differs from this instance's type.
    /// </exception>
    public int CompareTo([CanBeNull] ComparableValueCapsule<T>? other)
    {
        if (other is null)
        {
            return 1;
        }

        var otherType = other.GetType();
        if (GetType() != otherType)
        {
            throw new ArgumentException($@"Incompatible comparand type: {otherType.GetFullName().ToUIString()}.", nameof(other));
        }

        return ValueComparer.Compare(Value, other.Value);
    }

    /// <summary>
    ///     Gets the <see cref="IComparer{T}"/> used for comparing contained values.
    ///     Default implementation returns the default comparer for the type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    ///     The <see cref="IComparer{T}"/> used for comparing contained values.
    /// </returns>
    [DebuggerNonUserCode]
    [NotNull]
    protected virtual IComparer<T> GetValueComparer() => Comparer<T>.Default;
}