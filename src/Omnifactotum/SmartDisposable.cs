using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

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
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SmartDisposable{T}"/> class.
        /// </summary>
        /// <param name="instance">
        ///     The object to dispose of.
        /// </param>
        public SmartDisposable([CanBeNull] T instance)
        {
            this.Instance = instance;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the underlying object.
        /// </summary>
        public T Instance
        {
            get;
            private set;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Calls <see cref="IDisposable.Dispose"/> for the underlying object,
        ///     if it implements <see cref="IDisposable"/>; otherwise, does nothing.
        /// </summary>
        public void Dispose()
        {
            (this.Instance as IDisposable).DisposeSafely();
        }

        #endregion
    }
}