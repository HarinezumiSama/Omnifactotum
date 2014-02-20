using System;
using System.Collections.Generic;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the recursive processing result used
    ///     in the <see cref="Factotum.ProcessRecursively{T}"/> method(s).
    /// </summary>
    public enum RecursiveProcessingDirective
    {
        /// <summary>
        ///     The processing should continue.
        /// </summary>
        Continue,

        /// <summary>
        ///     The processing should not go recursively for the item being process but should continue processing
        ///     other items.
        /// </summary>
        NoRecursionForItem,

        /// <summary>
        ///     The processing should immediately stop processing all items.
        /// </summary>
        Terminate
    }
}