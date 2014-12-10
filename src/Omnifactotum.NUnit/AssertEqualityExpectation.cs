using System;
using System.Linq;

namespace Omnifactotum.NUnit
{
    //// TODO [vmcl] Think about IResolveConstraint for the expectations

    /// <summary>
    ///     Represents the equality expectation used in the <see cref="NUnitFactotum.AssertEquality{T}"/> method.
    /// </summary>
    public enum AssertEqualityExpectation
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