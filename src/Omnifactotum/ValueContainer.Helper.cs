using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of the <see cref="ValueContainer{T}"/> type
    ///     using the type inference in a friendly way.
    /// </summary>
    public static class ValueContainer
    {
        /// <summary>
        ///     Creates and initializes a new instance of the <see cref="ValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <returns>
        ///     A created and initialized instance of the <see cref="ValueContainer{T}" /> class.
        /// </returns>
        public static ValueContainer<T> Create<T>([CanBeNull] T value)
        {
            return new ValueContainer<T>(value);
        }
    }
}