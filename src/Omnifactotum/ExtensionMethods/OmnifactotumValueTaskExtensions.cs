using System.Runtime.CompilerServices;
using Omnifactotum;

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
}