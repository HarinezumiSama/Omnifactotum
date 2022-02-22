#nullable enable

using System;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents an error in a templated string occurred in <see cref="TemplatedStringResolver.Resolve"/>.
    /// </summary>
    public sealed class TemplatedStringResolverException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TemplatedStringResolverException"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        internal TemplatedStringResolverException(string message)
            : base(message)
        {
            // Nothing to do
        }
    }
}