﻿using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint<>))]
internal sealed class TypedNotNullConstraintTests : TypedConstraintTestsBase<NotNullConstraint<string>, string?>
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "A";
        yield return "ad414644a9324f8ebdde4befbc2d3c6a";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue) => "The 'string' value cannot be null.";
}