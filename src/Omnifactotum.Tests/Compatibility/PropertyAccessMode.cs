//// ReSharper disable once CheckNamespace :: Compatibility (Omnifactotum.NUnit)

namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Represents the property access mode.
    /// </summary>
    internal enum PropertyAccessMode
    {
        /// <summary>
        ///     The property is read-only.
        /// </summary>
        ReadOnly,

        /// <summary>
        ///     The property is write-only.
        /// </summary>
        WriteOnly,

        /// <summary>
        ///     The property can be read and written.
        /// </summary>
        ReadWrite
    }
}