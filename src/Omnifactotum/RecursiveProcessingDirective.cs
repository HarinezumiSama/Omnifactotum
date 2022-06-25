#nullable enable

using System;
using System.Collections.Generic;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the recursive processing result used
    ///     in the <see cref="Factotum.ProcessRecursively{T}(T,Func{T,IEnumerable{T}},Func{T,RecursiveProcessingDirective},RecursiveProcessingContext{T})"/>
    ///     method.
    /// </summary>
    public enum RecursiveProcessingDirective
    {
        /// <summary>
        ///     The processing should continue.
        /// </summary>
        Continue,

        /// <summary>
        ///     The processing should not go recursively for the item being processed but should continue for other items.
        /// </summary>
        NoRecursionForItem,

        /// <summary>
        ///     The processing should stop immediately.
        /// </summary>
        Terminate
    }
}