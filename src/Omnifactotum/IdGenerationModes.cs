using System;

namespace Omnifactotum
{
    /// <summary>
    ///     Specifies the modes of identifier generation.
    /// </summary>
    [Flags]
    public enum IdGenerationModes
    {
        /// <summary>
        ///     A generated identifier should be unique (as <see cref="Guid"/>).
        /// </summary>
        Unique = 0x1,

        /// <summary>
        ///     A generated identifier should be cryptographically random.
        /// </summary>
        Random = 0x2,

        /// <summary>
        ///     A generated identifier should be both unique and cryptographically random.
        /// </summary>
        UniqueAndRandom = Unique | Random
    }
}