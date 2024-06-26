﻿using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(WebUrlConstraint))]
internal sealed class WebUrlConstraintTests : TypedConstraintTestsBase<WebUrlConstraint, string?>
#pragma warning restore CS0618 // Type or member is obsolete
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "http://example.com";
        yield return "https://example.com";
        yield return "http://127.0.0.1";
        yield return "https://127.0.0.1";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
        yield return string.Empty;
        yield return "\x0020";
        yield return "A";
        yield return "127.0.0.1";
        yield return """C:\path\to\file.txt""";
        yield return "file:///C:/path/to/file";
        yield return "ftp://ftp.example.com/";
        yield return "http://";
        yield return "https://";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => $"The value {(invalidValue is null ? "null" : $"\"{invalidValue}\"")} is not a valid Web URL.";
}