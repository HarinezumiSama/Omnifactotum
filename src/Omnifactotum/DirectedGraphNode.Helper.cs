using System.Runtime.CompilerServices;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of
///     the <see cref="DirectedGraphNode{T}"/> type using type inference in a friendly way.
/// </summary>
public static class DirectedGraphNode
{
    /// <summary>
    ///     Creates a new instance of the <see cref="DirectedGraphNode{T}"/> class using the specified value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value.
    /// </typeparam>
    /// <param name="value">
    ///     The value to initialize <see cref="DirectedGraphNode{T}"/> with.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="DirectedGraphNode{T}"/> class.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static DirectedGraphNode<T> Create<T>(T value) => new(value);
}