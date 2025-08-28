using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum;
using ItemNotNullAttribute = Omnifactotum.Annotations.ItemNotNullAttribute;
using static Omnifactotum.ExtensionMethods.InternalExtensionFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Threading.Tasks;

/// <summary>
///     Contains extension methods for the <see cref="ValueTask"/> and <see cref="ValueTask{TResult}"/> types.
/// </summary>
public static class OmnifactotumValueTaskExtensions
{
    /// <summary>
    ///     Creates an awaiter used to await the specified <see cref="ValueTask"/>, configured so that it does not attempt
    ///     to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task">
    ///     The <see cref="ValueTask"/> to create an awaiter for.
    /// </param>
    /// <returns>
    ///     An awaiter used to await the specified <see cref="ValueTask"/>, configured so that it does not attempt to marshal
    ///     the continuation back to the original context captured.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="task"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ConfiguredValueTaskAwaitable ConfigureAwaitNoCapturedContext(this in ValueTask task)
        => task.ConfigureAwait(false);

    /// <summary>
    ///     Creates an awaiter used to await the specified <see cref="ValueTask{TResult}"/>, configured so that it does not attempt
    ///     to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="task">
    ///     The <see cref="ValueTask{TResult}"/> to create an awaiter for.
    /// </param>
    /// <returns>
    ///     An awaiter used to await the specified <see cref="ValueTask{TResult}"/>, configured so that it does not attempt to marshal
    ///     the continuation back to the original context captured.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="task"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ConfiguredValueTaskAwaitable<TResult> ConfigureAwaitNoCapturedContext<TResult>(this in ValueTask<TResult> task)
        => task.ConfigureAwait(false);

#if NET5_0_OR_GREATER
    /// <summary>
    ///     Returns a <see cref="ValueTask{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </summary>
    /// <param name="resultTask">
    ///     The <see cref="ValueTask{TResult}"/> whose result to return while checking it for <see langword="null"/>.
    /// </param>
    /// <param name="resultTaskExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="resultTask"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <typeparam name="T">
    ///     The type of the result produced by the task.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </returns>
    /// <seealso cref="M:OmnifactotumGenericObjectExtensions.EnsureNotNull{T}(T?,string?)"/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    [ItemNotNull]
    public static async ValueTask<T> EnsureNotNullAsync<T>(
        this ValueTask<T?> resultTask,
        [CallerArgumentExpression(nameof(resultTask))] string? resultTaskExpression = null)
        where T : class
        => (await resultTask).EnsureNotNull(GetEnsureNotNullAsyncExpressionForEnsureNotNull(resultTaskExpression));

    /// <summary>
    ///     Returns a <see cref="ValueTask{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </summary>
    /// <param name="resultTask">
    ///     The <see cref="ValueTask{TResult}"/> whose result to return while checking it for <see langword="null"/>.
    /// </param>
    /// <param name="resultTaskExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="resultTask"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET 5+ and C# 10+).</para>
    /// </param>
    /// <typeparam name="T">
    ///     The type of the result produced by the task.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}"/> that produces the result of the specified task if this result is not <see langword="null"/>;
    ///     otherwise, throws an exception.
    /// </returns>
    /// <seealso cref="M:OmnifactotumGenericObjectExtensions.EnsureNotNull{T}(T?,string?)"/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [DebuggerStepThrough]
    public static async ValueTask<T> EnsureNotNullAsync<T>(
        this ValueTask<T?> resultTask,
        [CallerArgumentExpression(nameof(resultTask))] string? resultTaskExpression = null)
        where T : struct
        => (await resultTask).EnsureNotNull(GetEnsureNotNullAsyncExpressionForEnsureNotNull(resultTaskExpression));
#endif
}