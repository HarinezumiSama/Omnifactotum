using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of
    ///     the <see cref="VirtualTreeNode{T}"/> type using type inference in a friendly way.
    /// </summary>
    public static class VirtualTreeNode
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="VirtualTreeNode{T}"/> class using the specified value.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="value">
        ///     The value to initialize <see cref="VirtualTreeNode{T}"/> with.
        /// </param>
        /// <returns>
        ///     A new instance of the <see cref="VirtualTreeNode{T}"/> class.
        /// </returns>
        public static VirtualTreeNode<T> Create<T>([CanBeNull] T value)
        {
            return new VirtualTreeNode<T>(value);
        }
    }
}