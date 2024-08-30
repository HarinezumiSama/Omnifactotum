using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Omnifactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// Namespace is intentionally named so in order to simplify usage of extension methods
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="System.Exception"/> class.
/// </summary>
public static class OmnifactotumExceptionExtensions
{
    /// <summary>
    ///     <para>Determines whether the specified exception should NOT be handled by a user code.</para>
    ///     <para>
    ///         The following exceptions (and their descendants) are considered by this method as the ones that generally should not be
    ///         handled by a user code:
    ///         <list type="bullet">
    ///             <item><see cref="ThreadAbortException"/></item>
    ///             <item><see cref="OperationCanceledException"/></item>
    ///             <item><see cref="OutOfMemoryException"/></item>
    ///             <item><see cref="StackOverflowException"/></item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <param name="exception">
    ///     The exception to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified exception should NOT be handled by a user code; otherwise, <see langword="false"/>.
    /// </returns>
    /// <example>
    ///     <code>
    /// try
    /// {
    ///     ...
    /// }
    /// catch (Exception ex)
    ///     when (!ex.IsFatal())
    /// {
    ///     Logger.Write(...);
    /// }
    ///     </code>
    /// </example>
    [DebuggerNonUserCode]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsFatal([CanBeNull] this Exception? exception)
        => exception is ThreadAbortException or OperationCanceledException or OutOfMemoryException or StackOverflowException;

    /// <summary>
    ///     Determines whether the specified exception or any of its inner exceptions is <typeparamref name="TOriginatingException"/> (or its descendant).
    /// </summary>
    /// <param name="exception">
    ///     The exception to check.
    /// </param>
    /// <typeparam name="TOriginatingException">
    ///     The type of the originating exception to check <paramref name="exception"/> against.
    /// </typeparam>
    /// <returns>
    ///     <see langword="true"/> if the specified exception or any of its inner exceptions is <typeparamref name="TOriginatingException"/>
    ///     (or its descendant); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerNonUserCode]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsOriginatedFrom<TOriginatingException>([CanBeNull] this Exception? exception)
        where TOriginatingException : Exception
        => exception.EnumerateRecursively().Any(ex => ex is TOriginatingException);

    /// <summary>
    ///     Determines whether the specified exception or any of its inner exceptions is of the specified type (or its descendant).
    /// </summary>
    /// <param name="exception">
    ///     The exception to check.
    /// </param>
    /// <param name="originatingExceptionType">
    ///     The type of the originating exception to check <paramref name="exception"/> against.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified exception or any of its inner exceptions is of the specified type
    ///     (or its descendant); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerNonUserCode]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsOriginatedFrom([CanBeNull] this Exception? exception, [NotNull] Type originatingExceptionType)
    {
        if (originatingExceptionType is null)
        {
            throw new ArgumentNullException(nameof(originatingExceptionType));
        }

        if (!typeof(Exception).IsAssignableFrom(originatingExceptionType))
        {
            throw new ArgumentException($"Invalid exception type {originatingExceptionType.GetFullName().ToUIString()}.", nameof(originatingExceptionType));
        }

        return exception.EnumerateRecursively().Any(originatingExceptionType.IsInstanceOfType);
    }

    /// <summary>
    ///     Generates a sequence that contains the specified exception and all its inner exceptions (recursively).
    ///     If <paramref name="exception"/> is <see langword="null"/>, then the resulting sequence is empty.
    /// </summary>
    /// <param name="exception">
    ///     The exception to enumerate recursively.
    /// </param>
    /// <returns>
    ///     A sequence that contains the specified exception and all its inner exceptions (recursively).
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Breadth first traversal is performed
    ///         (thus, if <paramref name="exception"/> is not <see langword="null"/>, it comes first in the resulting sequence).
    ///     </para>
    ///     <para>
    ///         Both <see cref="Exception.InnerException"/> and <see cref="AggregateException.InnerExceptions"/> are considered when the sequence is generated.
    ///     </para>
    /// </remarks>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<Exception> EnumerateRecursively([CanBeNull] this Exception? exception)
    {
        var currentException = exception;
        while (currentException is not null)
        {
            yield return currentException;

            if (currentException is AggregateException aggregateException)
            {
                foreach (var innerException in aggregateException.InnerExceptions.SelectMany(innerException => innerException.EnumerateRecursively()))
                {
                    yield return innerException;
                }

                break;
            }

            currentException = currentException.InnerException;
        }
    }
}