using System;

namespace Omnifactotum;

/// <summary>
///     Represents an assertion failure in <see cref="Factotum.Assert"/>.
/// </summary>
public sealed class OmnifactotumAssertionException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OmnifactotumAssertionException"/> class.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the error.
    /// </param>
    internal OmnifactotumAssertionException(string message)
        : base(message)
    {
        // Nothing to do
    }
}