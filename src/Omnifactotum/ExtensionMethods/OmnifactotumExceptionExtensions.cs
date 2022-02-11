#nullable enable

using System.Diagnostics;
using System.Threading;

//// Namespace is intentionally named so in order to simplify usage of extension methods
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
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
        public static bool IsFatal([CanBeNull] this Exception? exception)
            => exception is ThreadAbortException or OperationCanceledException or OutOfMemoryException or StackOverflowException;
    }
}