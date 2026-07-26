using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

internal abstract class NotNullConstraintRefObjectBaseTests<[MeansImplicitUse] TConstraint, T> : TypedConstraintTestsBase<TConstraint, T>
    where TConstraint : TypedMemberConstraintBase<T>, new()
{
    [Ignore("Not applicable.")]
    public sealed override void TestValidateWhenIncorrectValueTypeThenThrows() => throw new NotSupportedException();

    protected static IEnumerable<object> InternalGetTypedValidValues()
    {
        yield return string.Empty;
        yield return "A";
        yield return "f0f6806923e84EEFB3A76db6121ba0b5";

        yield return ImmutableArray<string>.Empty;
        yield return ImmutableArray.Create(string.Empty);

        yield return ImmutableArray<int>.Empty;
        yield return ImmutableArray.Create(0);
    }

    protected static IEnumerable<object> InternalGetTypedInvalidValues()
    {
        yield return null!;
        yield return default(ImmutableArray<string>);
        yield return default(ImmutableArray<int>);
    }

    protected sealed override ValidationErrorDetails GetTypedInvalidValueErrorDetails(T invalidValue) => "The 'object' value must not be null.";
}