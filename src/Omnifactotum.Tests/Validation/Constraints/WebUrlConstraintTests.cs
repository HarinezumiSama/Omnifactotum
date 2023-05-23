using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(WebUrlConstraint))]
internal sealed class WebUrlConstraintTests : TypedConstraintTestsBase<WebUrlConstraint, string?>
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
        yield return @"C:\path\to\file.txt";
        yield return @"file:///C:/path/to/file";
        yield return @"ftp://ftp.example.com/";
        yield return @"http://";
        yield return @"https://";
    }

    protected override string GetTypedInvalidValueErrorMessage(string? invalidValue) => $@"The value {invalidValue.ToUIString()} is not a valid Web URL.";
}