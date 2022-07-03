#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Abstractions;
using Omnifactotum.Annotations;
using ExcludeFromCodeCoverageAttribute = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the mutable thread-safe container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    [Serializable]
    [DebuggerDisplay(@"{ToDebuggerString(),nq}")]
    public sealed class SyncValueContainer<T> : IValueContainer<T>, IEquatable<SyncValueContainer<T>>
    {
        private T _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}" /> class
        ///     using the specified value and synchronization object.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <param name="syncObject">
        ///     The synchronization object used for thread-safe access.
        ///     Cannot be <see langword="null"/> and must be of a reference type.
        /// </param>
        public SyncValueContainer(T value, [NotNull] object syncObject)
        {
            if (syncObject is null)
            {
                throw new ArgumentNullException(nameof(syncObject));
            }

            if (syncObject.GetType().IsValueType)
            {
                throw new ArgumentException("The synchronization object cannot be a value type object.");
            }

            SyncObject = syncObject;
            _value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        public SyncValueContainer(T value)
            : this(value, new object())
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the synchronization object used for thread-safe access.
        /// </summary>
        [NotNull]
        public object SyncObject { get; }

        /// <summary>
        ///     Gets or sets the contained value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            [DebuggerNonUserCode]
            get
            {
                lock (SyncObject)
                {
                    return _value;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (SyncObject)
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="SyncValueContainer{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the two specified <see cref="SyncValueContainer{T}"/> instances are equal;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(SyncValueContainer<T>? left, SyncValueContainer<T>? right) => Equals(left, right);

        /// <summary>
        ///     Determines whether the two specified <see cref="SyncValueContainer{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the two specified <see cref="SyncValueContainer{T}"/> instances are not equal;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(SyncValueContainer<T>? left, SyncValueContainer<T>? right) => !(left == right);

        /// <summary>
        ///     Returns a <see cref="String" /> that represents this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="String" /> that represents this <see cref="SyncValueContainer{T}"/> instance.
        /// </returns>
        public override string ToString() => AsInvariant($@"{{ {nameof(Value)} = {Value.ToStringSafelyInvariant()} }}");

        /// <summary>
        ///     Determines whether the specified <see cref="Object" /> is equal to
        ///     this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this <see cref="SyncValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="Object" /> is equal to
        ///     this <see cref="SyncValueContainer{T}"/> instance; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object? obj) => Equals(obj as SyncValueContainer<T>);

        /// <summary>
        ///     Returns a hash code for this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this <see cref="SyncValueContainer{T}"/> instance.
        /// </returns>
        public override int GetHashCode()
            //// ReSharper disable once NonReadonlyMemberInGetHashCode :: Specific use-case
            => Value.GetHashCodeSafely();

        /// <summary>
        ///     Determines whether the current <see cref="SyncValueContainer{T}"/> instance is equal to another instance
        ///     of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this <see cref="SyncValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the current <see cref="SyncValueContainer{T}"/> instance is equal to
        ///     the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(SyncValueContainer<T>? other) => Equals(this, other);

        //// ReSharper disable SuggestBaseTypeForParameter :: Performance
        private static bool Equals(SyncValueContainer<T>? left, SyncValueContainer<T>? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(left.Value, right.Value);
        }
        //// ReSharper restore SuggestBaseTypeForParameter

        [ExcludeFromCodeCoverage]
        private string ToDebuggerString() => AsInvariant($@"{{ {nameof(Value)} = {_value.ToStringSafelyInvariant()} }}");
    }
}