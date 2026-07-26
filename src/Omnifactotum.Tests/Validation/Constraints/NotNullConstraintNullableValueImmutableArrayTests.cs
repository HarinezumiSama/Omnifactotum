using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.NullableValue<>))]
[SuppressMessage("ReSharper", "UseCollectionExpression", Justification = "Multiple target frameworks.")]
internal sealed class NotNullConstraintNullableValueImmutableArrayTests
    : TypedConstraintTestsBase<NotNullConstraint.NullableValue<ImmutableArray<string>>, ImmutableArray<string>?>
{
    protected override IEnumerable<ImmutableArray<string>?> GetTypedValidValues()
    {
        yield return ImmutableArray<string>.Empty;
        yield return ImmutableArray.Create(string.Empty);
        yield return ImmutableArray.Create("str1", "str2");
    }

    protected override IEnumerable<ImmutableArray<string>?> GetTypedInvalidValues()
    {
        yield return null;
        yield return default(ImmutableArray<string>);
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(ImmutableArray<string>? invalidValue)
        => "The 'ImmutableArray<string>?' value must not be null.";
}