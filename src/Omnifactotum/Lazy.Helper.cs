using System;
using System.Diagnostics;
using System.Threading;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Provides helper functionality for creating instances of the <see cref="System.Lazy{T}"/> type using
    ///     type inference in a friendly way.
    /// </summary>
    [DebuggerNonUserCode]
    public static class Lazy
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new instance of the <see cref="System.Lazy{T}"/> class. When lazy initialization
        ///     occurs, the specified initialization function is used.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of an object that is being lazily initialized.
        /// </typeparam>
        /// <param name="valueFactory">
        ///     The delegate that is invoked to produce the lazily initialized value when it is needed.
        /// </param>
        /// <returns>
        ///     A new <see cref="System.Lazy{T}"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory)
        {
            return new Lazy<T>(valueFactory);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="System.Lazy{T}"/> class. When lazy initialization
        ///     occurs, the specified initialization function and initialization mode are used.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of an object that is being lazily initialized.
        /// </typeparam>
        /// <param name="valueFactory">
        ///     The delegate that is invoked to produce the lazily initialized value when it is needed.
        /// </param>
        /// <param name="isThreadSafe">
        ///     <c>true</c> to make this instance usable concurrently by multiple threads; <c>false</c>
        ///     to make this instance usable by only one thread at a time.
        /// </param>
        /// <returns>
        ///     A new <see cref="System.Lazy{T}"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory, bool isThreadSafe)
        {
            return new Lazy<T>(valueFactory, isThreadSafe);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="System.Lazy{T}"/> class that uses the specified
        ///     initialization function and thread-safety mode.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of an object that is being lazily initialized.
        /// </typeparam>
        /// <param name="valueFactory">
        ///     The delegate that is invoked to produce the lazily initialized value when it is needed.
        /// </param>
        /// <param name="mode">
        ///     The thread safety mode.
        /// </param>
        /// <returns>
        ///     A new <see cref="System.Lazy{T}"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     <paramref name="mode"/> contains an invalid value.
        /// </exception>
        public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory, LazyThreadSafetyMode mode)
        {
            return new Lazy<T>(valueFactory, mode);
        }

        #endregion
    }
}