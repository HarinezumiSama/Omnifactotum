#nullable enable

using System;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the disposable wrapper for a strongly-typed object that implicitly implements or
    ///     might implement the <see cref="IDisposable"/> interface.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the underlying object to dispose of.
    /// </typeparam>
    public sealed class SmartDisposable<T> : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartDisposable{T}"/> class.
        /// </summary>
        /// <param name="instance">
        ///     The object to dispose of.
        /// </param>
        public SmartDisposable(T instance) => Instance = instance;

        /// <summary>
        ///     Gets the underlying object.
        /// </summary>
        public T Instance { get; }

        /// <summary>
        ///     Calls <see cref="IDisposable.Dispose"/> for the underlying object,
        ///     if it implements <see cref="IDisposable"/>; otherwise, does nothing.
        /// </summary>
        public void Dispose() => (Instance as IDisposable).DisposeSafely();
    }
}