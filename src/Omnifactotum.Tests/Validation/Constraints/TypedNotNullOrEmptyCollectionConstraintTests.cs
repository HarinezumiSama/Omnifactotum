using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(NotNullOrEmptyCollectionConstraint<>))]
internal sealed class TypedNotNullOrEmptyCollectionConstraintTests : TypedConstraintTestsBase<NotNullOrEmptyCollectionConstraint<int>, IEnumerable<int>?>
#pragma warning restore CS0618 // Type or member is obsolete
{
    protected override IEnumerable<IEnumerable<int>?> GetTypedValidValues()
    {
        yield return new[] { int.MinValue };
        yield return new[] { 17, 42 };
        yield return new[] { int.MaxValue };

        yield return ImmutableArray.Create(23, 29);
        yield return ImmutableList.Create(31, -37, 11);
        yield return PureReadOnlyCollection.Create(-31, 0, 11);
    }

    protected override IEnumerable<IEnumerable<int>?> GetTypedInvalidValues()
    {
        yield return null;
        yield return default(ImmutableArray<int>);

        yield return Array.Empty<int>();
        yield return ImmutableArray<int>.Empty;
        yield return ImmutableList<int>.Empty;
        yield return PureReadOnlyCollection<int>.Empty;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(IEnumerable<int>? invalidValue)
        => invalidValue is null or ImmutableArray<int> { IsDefault: true } ? "The value cannot be null." : "The collection cannot be empty.";
}