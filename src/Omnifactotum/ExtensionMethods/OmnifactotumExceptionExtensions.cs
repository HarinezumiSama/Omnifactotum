using System.Diagnostics;
using System.Threading;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Exception"/> class.
    /// </summary>
    public static class OmnifactotumExceptionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Determines whether the specified exception should NOT be handled by a user code.
        /// </summary>
        /// <param name="exception">
        ///     The exception to check.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the specified exception should NOT be handled by a user code; otherwise, <b>false</b>.
        /// </returns>
        [DebuggerNonUserCode]
        public static bool IsFatal(this Exception exception)
        {
            return exception is ThreadAbortException
                || exception is OperationCanceledException
                || exception is OutOfMemoryException
                || exception is StackOverflowException;
        }

        #endregion
    }
}