using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Omnifactotum;

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
    public static bool IsOriginatedFrom<TOriginatingException>([CanBeNull] this Exception? exception)
        where TOriginatingException : Exception
    {
        var currentException = exception;
        while (currentException is not null)
        {
            if (currentException is TOriginatingException)
            {
                return true;
            }

            currentException = currentException.InnerException;
        }

        return false;
    }

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

        var currentException = exception;
        while (currentException is not null)
        {
            if (originatingExceptionType.IsInstanceOfType(currentException))
            {
                return true;
            }

            currentException = currentException.InnerException;
        }

        return false;
    }
}