using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Omnifactotum;
using ItemNotNullAttribute = Omnifactotum.Annotations.ItemNotNullAttribute;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;
using static Omnifactotum.ExtensionMethods.InternalExtensionFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Threading.Tasks;

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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
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
    ///     A task that represents the completion of all the supplied tasks.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static async Task AwaitAllAsync([NotNull] this IEnumerable<Task> tasks)
        => await Task.WhenAll(tasks).ConfigureAwaitNoCapturedContext();

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
    ///     A task that represents the completion of all the supplied tasks.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static async Task<TResult[]> AwaitAllAsync<TResult>([NotNull] this IEnumerable<Task<TResult>> tasks)
        => await Task.WhenAll(tasks).ConfigureAwaitNoCapturedContext();

#if NET5_0_OR_GREATER
    /// <summary>
    ///     Returns a <see cref="Task{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </summary>
    /// <param name="resultTask">
    ///     The <see cref="Task{TResult}"/> whose result to return while checking it for <see langword="null"/>.
    /// </param>
    /// <param name="resultTaskExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="resultTask"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <typeparam name="T">
    ///     The type of the result produced by the task.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </returns>
    /// <seealso cref="M:OmnifactotumGenericObjectExtensions.EnsureNotNull{T}(T?,string?)"/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    [NotNull]
    [ItemNotNull]
    public static async Task<T> EnsureNotNullAsync<T>(
        [NotNull] this Task<T?> resultTask,
        [CallerArgumentExpression(nameof(resultTask))] string? resultTaskExpression = null)
        where T : class
        => resultTask is null
            ? throw new ArgumentNullException(nameof(resultTask), GetEnsureNotNullAsyncArgumentNullMessage(resultTaskExpression))
            : (await resultTask).EnsureNotNull(GetEnsureNotNullAsyncExpressionForEnsureNotNull(resultTaskExpression));

    /// <summary>
    ///     Returns a <see cref="Task{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </summary>
    /// <param name="resultTask">
    ///     The <see cref="Task{TResult}"/> whose result to return while checking it for <see langword="null"/>.
    /// </param>
    /// <param name="resultTaskExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="resultTask"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <typeparam name="T">
    ///     The type of the result produced by the task.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </returns>
    /// <seealso cref="M:OmnifactotumGenericObjectExtensions.EnsureNotNull{T}(T?,string?)"/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    public static async Task<T> EnsureNotNullAsync<T>(
        [NotNull] this Task<T?> resultTask,
        [CallerArgumentExpression(nameof(resultTask))] string? resultTaskExpression = null)
        where T : struct
        => resultTask is null
            ? throw new ArgumentNullException(nameof(resultTask), GetEnsureNotNullAsyncArgumentNullMessage(resultTaskExpression))
            : (await resultTask).EnsureNotNull(GetEnsureNotNullAsyncExpressionForEnsureNotNull(resultTaskExpression));
#endif
}