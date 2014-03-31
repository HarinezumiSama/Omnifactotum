using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    ///     Indicates that a method does not make any observable state changes.
    ///     The same as <see cref="System.Diagnostics.Contracts.PureAttribute"/>.
    /// </summary>
    /// <example>
    ///     <code>
    ///     [Pure]
    ///     private int Multiply(int x, int y) { return x * y; }
    ///     
    ///     public void Foo()
    ///     {
    ///         const int a = 2, b = 2;
    ///         Multiply(a, b); // Warning: Return value of pure method is not used
    ///     }
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class PureAttribute : Attribute
    {
        // No members
    }
}