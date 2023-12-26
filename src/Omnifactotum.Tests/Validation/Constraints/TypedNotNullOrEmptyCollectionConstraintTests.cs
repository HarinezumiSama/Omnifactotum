using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullOrEmptyCollectionConstraint<>))]
internal sealed class TypedNotNullOrEmptyCollectionConstraintTests : TypedConstraintTestsBase<NotNullOrEmptyCollectionConstraint<int>, ICollection<int>?>
{
    protected override IEnumerable<ICollection<int>?> GetTypedValidValues()
    {
        yield return new[] { int.MinValue };
        yield return new[] { 17, 42 };
        yield return new[] { int.MaxValue };

        yield return ImmutableArray.Create(23, 29);
    }

    protected override IEnumerable<ICollection<int>?> GetTypedInvalidValues()
    {
        yield return null;
        yield return Array.Empty<int>();

        yield return default(ImmutableArray<int>);
        yield return ImmutableArray<int>.Empty;
    }

    protected override string GetTypedInvalidValueErrorMessage(ICollection<int>? invalidValue)
        => invalidValue is null or ImmutableArray<int> { IsDefault: true } ? "The value cannot be null." : "The collection cannot be empty.";
}