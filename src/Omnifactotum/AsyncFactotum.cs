using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper methods and properties for asynchronous programming.
    /// </summary>
    public static partial class AsyncFactotum
    {
        /// <summary>
        ///     Encapsulates a method that logs the specified exception occurred during execution of an asynchronous
        ///     operation.
        /// </summary>
        /// <param name="exception">
        ///     The exception to log.
        /// </param>
        public delegate void LogErrorWithException([NotNull] Exception exception);

        /// <summary>
        ///     Encapsulates a method that logs the specified exception occurred during execution of an asynchronous
        ///     operation and the specified message.
        /// </summary>
        /// <param name="exception">
        ///     The exception to log.
        /// </param>
        /// <param name="message">
        ///     The message to log.
        /// </param>
        public delegate void LogErrorWithExceptionAndMessage([NotNull] Exception exception, [NotNull] string message);

        /// <summary>
        ///     Encapsulates a method that logs the specified message in case when an exception has occurred during
        ///     execution of an asynchronous operation.
        /// </summary>
        /// <param name="message">
        ///     The message to log.
        /// </param>
        public delegate void LogErrorWithMessage([NotNull] string message);

        /// <summary>
        ///     Wraps the <see cref="Trace.TraceError(string)"/> method since it is conditional and thus cannot be
        ///     used directly as a delegate.
        /// </summary>
        private static void TraceErrorInternal(string message)
        {
            Trace.TraceError(message);
        }

        private static Exception GetBaseException([NotNull] Exception exception)
        {
            return exception is AggregateException ? exception.GetBaseException() : exception;
        }

        private static Task<TResult> CreateAndStartComputeTask<TResult, TLoggingMethod>(
            [NotNull] this Func<TResult> taskFunction,
            [NotNull] MethodBase method,
            [NotNull] TLoggingMethod logError,
            [NotNull] Action<Task, MethodBase, TLoggingMethod> attachErrorLoggingTask)
        {
            var task = new Task<TResult>(taskFunction);
            attachErrorLoggingTask(task, method, logError);
            task.Start();

            return task;
        }

        private static Task CreateAndStartExecuteTask<TLoggingMethod>(
            [NotNull] this Action taskFunction,
            [NotNull] MethodBase method,
            [NotNull] TLoggingMethod logError,
            [NotNull] Action<Task, MethodBase, TLoggingMethod> attachErrorLoggingTask)
        {
            var task = new Task(taskFunction);
            attachErrorLoggingTask(task, method, logError);
            task.Start();

            return task;
        }

        private static void AttachErrorLoggingTask(
            [NotNull] Task task,
            [NotNull] MethodBase method,
            [NotNull] LogErrorWithExceptionAndMessage logError)
        {
            task.ContinueWith(
                t =>
                {
                    var baseException = GetBaseException(t.Exception.EnsureNotNull());

                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The asynchronous call {{ {0} }} has failed.",
                        method.GetFullSignature());

                    logError(baseException, message);
                },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        private static void AttachErrorLoggingTask(
            [NotNull] Task task,
            //// ReSharper disable once UnusedParameter.Local - Used to keep the method signature template the same
            [NotNull] MethodBase method,
            [NotNull] LogErrorWithException logError)
        {
            task.ContinueWith(
                t =>
                {
                    var baseException = GetBaseException(t.Exception.EnsureNotNull());
                    logError(baseException);
                },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }

        private static void AttachErrorLoggingTask(
            [NotNull] Task task,
            [NotNull] MethodBase method,
            [NotNull] LogErrorWithMessage logError)
        {
            task.ContinueWith(
                t =>
                {
                    var baseException = GetBaseException(t.Exception.EnsureNotNull());

                    var message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The asynchronous call {{ {0} }} has failed: {1}",
                        method.GetFullSignature(),
                        baseException);

                    logError(message);
                },
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}