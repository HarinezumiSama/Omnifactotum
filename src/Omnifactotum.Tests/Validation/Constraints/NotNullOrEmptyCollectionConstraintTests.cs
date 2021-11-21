using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture(TestOf = typeof(NotNullOrEmptyCollectionConstraint))]
    internal sealed class NotNullOrEmptyCollectionConstraintTests
        : TypedConstraintTestsBase<NotNullOrEmptyCollectionConstraint, ICollection>
    {
        protected override IEnumerable<ICollection> GetTypedValidValues()
        {
            yield return new[] { "A" };
            yield return new[] { 42 };
        }

        protected override IEnumerable<ICollection> GetTypedInvalidValues()
        {
            yield return null;
            yield return Array.Empty<string>();
            yield return Array.Empty<int>();
        }
    }
}