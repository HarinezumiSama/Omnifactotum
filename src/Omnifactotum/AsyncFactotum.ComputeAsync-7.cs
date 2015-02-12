using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper methods and properties for asynchronous programming.
    /// </summary>
    public static partial class AsyncFactotum
    {
        #region Public Methods: ComputeAsync[7]

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified options and exception
        ///     logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="options">
        ///     The options of asynchronous execution, or <c>null</c> to use the default options.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [CanBeNull] AsyncOptions options,
            [NotNull] LogErrorWithExceptionAndMessage logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            #region Argument Check

            if (callee == null)
            {
                throw new ArgumentNullException("callee");
            }

            if (logError == null)
            {
                throw new ArgumentNullException("logError");
            }

            #endregion

            var task = CreateAndStartComputeTask(
                () => callee(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                options,
                callee.Method,
                logError,
                AttachErrorLoggingTask);

            return task;
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified options and exception
        ///     logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="options">
        ///     The options of asynchronous execution, or <c>null</c> to use the default options.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [CanBeNull] AsyncOptions options,
            [NotNull] LogErrorWithException logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            #region Argument Check

            if (callee == null)
            {
                throw new ArgumentNullException("callee");
            }

            if (logError == null)
            {
                throw new ArgumentNullException("logError");
            }

            #endregion

            var task = CreateAndStartComputeTask(
                () => callee(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                options,
                callee.Method,
                logError,
                AttachErrorLoggingTask);

            return task;
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified options and exception
        ///     logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="options">
        ///     The options of asynchronous execution, or <c>null</c> to use the default options.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [CanBeNull] AsyncOptions options,
            [NotNull] LogErrorWithMessage logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            #region Argument Check

            if (callee == null)
            {
                throw new ArgumentNullException("callee");
            }

            if (logError == null)
            {
                throw new ArgumentNullException("logError");
            }

            #endregion

            var task = CreateAndStartComputeTask(
                () => callee(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                options,
                callee.Method,
                logError,
                AttachErrorLoggingTask);

            return task;
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified exception logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [NotNull] LogErrorWithExceptionAndMessage logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            return ComputeAsync(callee, null, logError, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified exception logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [NotNull] LogErrorWithException logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            return ComputeAsync(callee, null, logError, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified exception logging method.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="logError">
        ///     A reference to a method that will log an exception occurred during execution of asynchronous
        ///     operation.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="callee"/> is <c>null</c>.</para>
        ///     <para>- or -</para>
        ///     <para><paramref name="logError"/> is <c>null</c>.</para>
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [NotNull] LogErrorWithMessage logError,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            return ComputeAsync(callee, null, logError, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        ///     Computes a result of the specified method asynchronously using the specified options.
        ///     This method uses <see cref="Trace"/> to log asynchronous operation errors.
        /// </summary>
        /// <typeparam name="T1">
        ///     The type of the 1st argument of the method.
        /// </typeparam>
        /// <typeparam name="T2">
        ///     The type of the 2nd argument of the method.
        /// </typeparam>
        /// <typeparam name="T3">
        ///     The type of the 3rd argument of the method.
        /// </typeparam>
        /// <typeparam name="T4">
        ///     The type of the 4th argument of the method.
        /// </typeparam>
        /// <typeparam name="T5">
        ///     The type of the 5th argument of the method.
        /// </typeparam>
        /// <typeparam name="T6">
        ///     The type of the 6th argument of the method.
        /// </typeparam>
        /// <typeparam name="T7">
        ///     The type of the 7th argument of the method.
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the method.
        /// </typeparam>
        /// <param name="callee">
        ///     The method that is run asynchronously.
        /// </param>
        /// <param name="options">
        ///     The options of asynchronous execution, or <c>null</c> to use the default options.
        /// </param>
        /// <param name="arg1">
        ///     The 1st argument of the method.
        /// </param>
        /// <param name="arg2">
        ///     The 2nd argument of the method.
        /// </param>
        /// <param name="arg3">
        ///     The 3rd argument of the method.
        /// </param>
        /// <param name="arg4">
        ///     The 4th argument of the method.
        /// </param>
        /// <param name="arg5">
        ///     The 5th argument of the method.
        /// </param>
        /// <param name="arg6">
        ///     The 6th argument of the method.
        /// </param>
        /// <param name="arg7">
        ///     The 7th argument of the method.
        /// </param>
        /// <returns>
        ///     A task that is executing or has executed the specified method asynchronously.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="callee"/> is <c>null</c>.
        /// </exception>
        public static Task<TResult> ComputeAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> callee,
            [CanBeNull] AsyncOptions options,
            T1 arg1,
            T2 arg2,
            T3 arg3,
            T4 arg4,
            T5 arg5,
            T6 arg6,
            T7 arg7)
        {
            #region Argument Check

            if (callee == null)
            {
                throw new ArgumentNullException("callee");
            }

            #endregion

            var task = CreateAndStartComputeTask(
                () => callee(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                options,
                callee.Method,
                (LogErrorWithMessage)TraceErrorInternal,
                AttachErrorLoggingTask);

            return task;
        }

        #endregion
    }
}