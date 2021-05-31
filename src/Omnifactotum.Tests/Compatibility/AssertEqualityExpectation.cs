//// ReSharper disable once CheckNamespace :: Compatibility (Omnifactotum.NUnit)

namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Represents the equality expectation used in the <see cref="NUnitFactotum.AssertEquality{T}"/> method.
    /// </summary>
    internal enum AssertEqualityExpectation
    {
        /// <summary>
        ///     The values must be not equal.
        /// </summary>
        NotEqual,

        /// <summary>
        ///     The values must be equal and may be the same reference.
        /// </summary>
        EqualAndMayBeSame,

        /// <summary>
        ///     The values must be equal and cannot be the same reference.
        /// </summary>
        EqualAndCannotBeSame
    }
}