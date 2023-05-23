using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullOrEmptyCollectionConstraint))]
internal sealed class TypedNotNullOrEmptyCollectionConstraintTests : TypedConstraintTestsBase<NotNullOrEmptyCollectionConstraint<int>, ICollection<int>?>
{
    protected override IEnumerable<ICollection<int>?> GetTypedValidValues()
    {
        yield return new[] { 42 };
    }

    protected override IEnumerable<ICollection<int>?> GetTypedInvalidValues()
    {
        yield return null;
        yield return Array.Empty<int>();
    }

    protected override string GetTypedInvalidValueErrorMessage(ICollection<int>? invalidValue)
        => invalidValue is null ? "The value cannot be null." : "The collection cannot be empty.";
}