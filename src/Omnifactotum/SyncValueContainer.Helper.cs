using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

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
    ///     Cannot be <see langword="null"/> and must be of a reference type.
    /// </param>
    /// <returns>
    ///     A created and initialized instance of the <see cref="SyncValueContainer{T}" /> class.
    /// </returns>
    public static SyncValueContainer<T> Create<T>(T value, [NotNull] object syncObject) => new(value, syncObject);

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
    public static SyncValueContainer<T> Create<T>(T value) => new(value);
}