﻿using System.Threading;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage
namespace System;

/// <summary>
///     Provides helper functionality for creating instances of the <see cref="System.Lazy{T}"/> type using
///     type inference in a friendly way.
/// </summary>
public static class Lazy
{
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
    ///     <paramref name="valueFactory"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory) => new(valueFactory);

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
    ///     <see langword="true"/> to make this instance usable concurrently by multiple threads; <see langword="false"/>
    ///     to make this instance usable by only one thread at a time.
    /// </param>
    /// <returns>
    ///     A new <see cref="System.Lazy{T}"/> instance.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="valueFactory"/> is <see langword="null"/>.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory, bool isThreadSafe) => new(valueFactory, isThreadSafe);

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
    ///     <paramref name="valueFactory"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     <paramref name="mode"/> contains an invalid value.
    /// </exception>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static Lazy<T> Create<T>([NotNull] Func<T> valueFactory, LazyThreadSafetyMode mode) => new(valueFactory, mode);
}