using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalNotEmptyCollectionConstraint<>))]
internal sealed class TypedOptionalNotEmptyCollectionConstraintTests
    : TypedConstraintTestsBase<OptionalNotEmptyCollectionConstraint<int>, IEnumerable<int>?>
{
    protected override IEnumerable<IEnumerable<int>?> GetTypedValidValues()
    {
        yield return null;
        yield return default(ImmutableArray<int>);

        yield return new[] { int.MinValue };
        yield return new[] { 17, 42 };
        yield return new[] { int.MaxValue };

        yield return ImmutableArray.Create(23, 29);
        yield return ImmutableList.Create(31, -37, 11);
        yield return PureReadOnlyCollection.Create(-31, 0, 11);
    }

    protected override IEnumerable<IEnumerable<int>?> GetTypedInvalidValues()
    {
        yield return Array.Empty<int>();
        yield return ImmutableArray<int>.Empty;
        yield return ImmutableList<int>.Empty;
        yield return PureReadOnlyCollection<int>.Empty;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(IEnumerable<int>? invalidValue) => "The collection cannot be empty.";
}