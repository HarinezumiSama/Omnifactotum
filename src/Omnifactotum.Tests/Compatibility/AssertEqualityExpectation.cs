#nullable enable

//// ReSharper disable once CheckNamespace :: Compatibility (Omnifactotum.NUnit)
namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Represents the equality expectation for the compared values.
    /// </summary>
    /// <seealso cref="NUnitFactotum.AssertEquality{T}"/>
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
        ///     The values must be equal, but cannot be the same reference unless they are <see langword="null"/>.
        /// </summary>
        EqualAndCannotBeSame
    }
}