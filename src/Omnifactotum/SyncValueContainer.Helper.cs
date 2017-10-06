using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of the <see cref="SyncValueContainer{T}"/> type
    ///     using the type inference in a friendly way.
    /// </summary>
    public static class SyncValueContainer
    {
        /// <summary>
        ///     Creates and initializes a new instance of the <see cref="SyncValueContainer{T}" /> class
        ///     using the specified value and synchronization object.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <param name="syncObject">
        ///     The synchronization object used for thread-safe access.
        ///     Cannot be <c>null</c> and must be of a reference type.
        /// </param>
        /// <returns>
        ///     A created and initialized instance of the <see cref="SyncValueContainer{T}" /> class.
        /// </returns>
        public static SyncValueContainer<T> Create<T>([CanBeNull] T value, [NotNull] object syncObject)
        {
            return new SyncValueContainer<T>(value, syncObject);
        }

        /// <summary>
        ///     Creates and initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <returns>
        ///     A created and initialized instance of the <see cref="SyncValueContainer{T}" /> class.
        /// </returns>
        public static SyncValueContainer<T> Create<T>([CanBeNull] T value)
        {
            return new SyncValueContainer<T>(value);
        }
    }
}