#nullable enable

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Threading.Tasks
{
    /// <summary>
    ///     Contains extension methods for the <see cref="Task"/> and <see cref="Task{TResult}"/> types as well as
    ///     the respective collections.
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantNullnessAttributeWithNullableReferenceTypes")]
    public static class OmnifactotumTaskExtensions
    {
        /// <summary>
        ///     Creates an awaiter used to await the specified <see cref="Task"/>, configured so that it does not attempt
        ///     to marshal the continuation back to the original context captured.
        /// </summary>
        /// <param name="task">
        ///     The <see cref="Task"/> to create an awaiter for.
        /// </param>
        /// <returns>
        ///     An awaiter used to await the specified <see cref="Task"/>, configured so that it does not attempt to marshal
        ///     the continuation back to the original context captured.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="task"/> is <see langword="null"/>.
        /// </exception>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static ConfiguredTaskAwaitable ConfigureAwaitNoCapturedContext([NotNull] this Task task)
            => (task ?? throw new ArgumentNullException(nameof(task))).ConfigureAwait(false);

        /// <summary>
        ///     Creates an awaiter used to await the specified <see cref="Task{TResult}"/>, configured so that it does not attempt
        ///     to marshal the continuation back to the original context captured.
        /// </summary>
        /// <param name="task">
        ///     The <see cref="Task"/> to create an awaiter for.
        /// </param>
        /// <returns>
        ///     An awaiter used to await the specified <see cref="Task{TResult}"/>, configured so that it does not attempt to marshal
        ///     the continuation back to the original context captured.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="task"/> is <see langword="null"/>.
        /// </exception>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwaitNoCapturedContext<TResult>(
            [NotNull] this Task<TResult> task)
            => (task ?? throw new ArgumentNullException(nameof(task))).ConfigureAwait(false);

        /// <summary>
        ///     Creates a task that will complete when all of the <see cref="Task"/> objects in an enumerable collection
        ///     have completed.
        /// </summary>
        /// <param name="tasks">
        ///     The tasks to wait on for completion.
        /// </param>
        /// <returns>
        ///     A task that represents the completion of all of the supplied tasks.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static Task AwaitAllAsync([NotNull] this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

        /// <summary>
        ///     Creates a task that will complete when all of the <see cref="Task{TResult}"/> objects in an enumerable collection
        ///     have completed.
        /// </summary>
        /// <param name="tasks">
        ///     The tasks to wait on for completion.
        /// </param>
        /// <typeparam name="TResult">
        ///     The type of the completed task.
        /// </typeparam>
        /// <returns>
        ///     A task that represents the completion of all of the supplied tasks.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static Task<TResult[]> AwaitAllAsync<TResult>([NotNull] this IEnumerable<Task<TResult>> tasks) => Task.WhenAll(tasks);
    }
}