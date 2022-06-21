#nullable enable

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the abstract immutable container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    public abstract class ValueCapsule<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueCapsule{T}"/> class using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        protected ValueCapsule(T value) => Value = value;

        /// <summary>
        ///     Gets the contained value.
        /// </summary>
        public T Value { get; }
    }
}