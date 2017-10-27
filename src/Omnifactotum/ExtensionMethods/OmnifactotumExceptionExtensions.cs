using System.Diagnostics;
using System.Threading;

//// Namespace is intentionally named so in order to simplify usage of extension methods
using Omnifactotum.Annotations;

//// ReSharper disable CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Exception"/> class.
    /// </summary>
    public static class OmnifactotumExceptionExtensions
    {
        /// <summary>
        ///     Determines whether the specified exception should NOT be handled by a user code.
        /// </summary>
        /// <param name="exception">
        ///     The exception to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified exception should NOT be handled by a user code; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerNonUserCode]
        public static bool IsFatal([CanBeNull] this Exception exception)
            => exception is ThreadAbortException
                || exception is OperationCanceledException
                || exception is OutOfMemoryException
                || exception is StackOverflowException;
    }
}