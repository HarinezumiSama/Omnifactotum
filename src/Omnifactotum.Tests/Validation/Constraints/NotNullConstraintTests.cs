using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint))]
internal sealed class NotNullConstraintTests : ConstraintTestsBase<NotNullConstraint>
{
    [Ignore("Not applicable.")]
    public override void TestValidateWhenIncorrectValueTypeThenThrows() => throw new NotSupportedException();

    protected override IEnumerable<object?> GetValidValues()
    {
        yield return new object();
        yield return "904b350d50cc40d3a91c5d7652e76f1f";
        yield return 42;
        yield return Math.PI;

        yield return Array.Empty<object>();
        yield return Array.Empty<string>();
        yield return Array.Empty<int>();
        yield return Array.Empty<int?>();

        yield return ImmutableArray<object>.Empty;
        yield return ImmutableArray<string>.Empty;
        yield return ImmutableArray<int>.Empty;
        yield return ImmutableArray<int?>.Empty;
    }

    protected override IEnumerable<object?> GetInvalidValues()
    {
        yield return null;
        yield return default(ImmutableArray<object?>);
        yield return default(ImmutableArray<string>);
        yield return default(ImmutableArray<int>);
        yield return default(ImmutableArray<int?>);
    }

    protected override string GetInvalidValueErrorMessage(object? invalidValue) => "The value cannot be null.";
}