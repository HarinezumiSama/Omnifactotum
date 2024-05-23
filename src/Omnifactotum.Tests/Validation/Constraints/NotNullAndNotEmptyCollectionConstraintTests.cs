using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullAndNotEmptyCollectionConstraint))]
internal sealed class NotNullAndNotEmptyCollectionConstraintTests : TypedConstraintTestsBase<NotNullAndNotEmptyCollectionConstraint, ICollection?>
{
    protected override IEnumerable<ICollection?> GetTypedValidValues()
    {
        yield return new[] { "A" };
        yield return new[] { 42 };
        yield return new object[] { "A", 42 };

        yield return ImmutableArray.Create<object>("q", 31);
    }

    protected override IEnumerable<ICollection?> GetTypedInvalidValues()
    {
        yield return null;

        yield return Array.Empty<object?>();
        yield return Array.Empty<string>();
        yield return Array.Empty<int>();
        yield return Array.Empty<int?>();

        yield return default(ImmutableArray<object?>);
        yield return default(ImmutableArray<string>);
        yield return default(ImmutableArray<int>);
        yield return default(ImmutableArray<int?>);

        yield return ImmutableArray<object?>.Empty;
        yield return ImmutableArray<string>.Empty;
        yield return ImmutableArray<int>.Empty;
        yield return ImmutableArray<int?>.Empty;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(ICollection? invalidValue)
        => invalidValue is null or ImmutableArray<object?> { IsDefault: true } or ImmutableArray<string> { IsDefault: true }
            or ImmutableArray<int> { IsDefault: true } or ImmutableArray<int?> { IsDefault: true }
            ? "The value cannot be null."
            : "The collection cannot be empty.";
}