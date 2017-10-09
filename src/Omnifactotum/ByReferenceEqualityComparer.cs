using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the equality comparer that compares objects of the specified type by their references.
    ///     If the type <typeparamref name="T"/> represents a value type, then the objects are compared using
    ///     the default comparer for the type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of objects to compare.
    /// </typeparam>
    public sealed class ByReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly bool _isReferenceType = !typeof(T).IsValueType;

        /// <summary>
        ///     Prevents a default instance of the <see cref="ByReferenceEqualityComparer{T}"/> class
        ///     from being created.
        /// </summary>
        private ByReferenceEqualityComparer()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the sole instance of the <see cref="ByReferenceEqualityComparer{T}"/> class.
        /// </summary>
        public static ByReferenceEqualityComparer<T> Instance
        {
            get;
        } = new ByReferenceEqualityComparer<T>();

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
        ///     <c>true</c> if the specified objects are equal by reference; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(T x, T y)
        {
            return _isReferenceType ? ReferenceEquals(x, y) : EqualityComparer<T>.Default.Equals(x, y);
        }

        /// <summary>
        ///     Returns a hash code for the specified object, based on object's reference.
        /// </summary>
        /// <param name="obj">
        ///     The object for which a hash code is to be returned.
        /// </param>
        /// <returns>
        ///     A hash code for the specified object, based on object's reference.
        /// </returns>
        public int GetHashCode(T obj)
        {
            return _isReferenceType ? RuntimeHelpers.GetHashCode(obj) : obj.GetHashCodeSafely();
        }
    }
}