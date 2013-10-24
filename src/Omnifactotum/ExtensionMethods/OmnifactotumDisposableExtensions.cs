﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains extension methods for <see cref="IDisposable"/> interface.
    /// </summary>
    public static class OmnifactotumDisposableExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Calls the <see cref="IDisposable.Dispose"/> method of the specified instance implementing
        ///     the <see cref="IDisposable"/> interface, if this instance is not <b>null</b>;
        ///     otherwise, does nothing.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the disposable instance.
        /// </typeparam>
        /// <param name="disposable">
        ///     A reference to an object to dispose.
        /// </param>
        public static void DisposeSafely<T>(this T disposable)
            where T : IDisposable
        {
            if (!ReferenceEquals(disposable, null))
            {
                disposable.Dispose();
            }
        }

        #endregion
    }
}