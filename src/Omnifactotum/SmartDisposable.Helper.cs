﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of the <see cref="SmartDisposable{T}"/> type
    ///     using type inference in a friendly way.
    /// </summary>
    [DebuggerNonUserCode]
    public static class SmartDisposable
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new instance of the <see cref="SmartDisposable{T}"/>
        ///     using the specified underlying object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the underlying object to dispose of.
        /// </typeparam>
        /// <param name="instance">
        ///     The object to dispose of.
        /// </param>
        /// <returns>
        ///     A new <see cref="SmartDisposable{T}"/> instance.
        /// </returns>
        public static SmartDisposable<T> Create<T>(T instance)
        {
            return new SmartDisposable<T>(instance);
        }

        #endregion
    }
}