using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture(TestOf = typeof(NotNullOrEmptyCollectionConstraint))]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod", Justification = "Cannot be used due to multi-targeting.")]
    [SuppressMessage("Performance", "CA1825", Justification = "Cannot be used due to multi-targeting.")]
    internal sealed class TypedNotNullOrEmptyCollectionConstraintTests
        : TypedConstraintTestsBase<NotNullOrEmptyCollectionConstraint<int>, ICollection<int>>
    {
        protected override IEnumerable<ICollection<int>> GetTypedValidValues()
        {
            yield return new[] { 42 };
        }

        protected override IEnumerable<ICollection<int>> GetTypedInvalidValues()
        {
            yield return null;
            yield return new int[0];
        }
    }
}