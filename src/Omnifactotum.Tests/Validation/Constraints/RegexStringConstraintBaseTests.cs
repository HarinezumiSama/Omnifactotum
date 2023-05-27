using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(RegexStringConstraintBase))]
internal sealed class RegexStringConstraintBaseTests : TypedConstraintTestsBase<RegexStringConstraintBaseTests.RegexStringConstraint, string?>
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "Foo";
        yield return "FooBar";
        yield return "Foo\n";
        yield return "Foo\x0020";
        yield return "Foo\t";
        yield return "Foo\x0020something";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
        yield return string.Empty;
        yield return "foo";
        yield return "FOO";
        yield return "-Foo";
        yield return "\x0020Foo";
        yield return "\tFoo";
        yield return "\rFoo";
        yield return "\nFoo";
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected override string GetTypedInvalidValueErrorMessage(string? invalidValue)
        => $"The value {invalidValue.ToUIString()} does not match the regular expression pattern \"^Foo\" (options: ExplicitCapture, Compiled, Singleline).";

    internal sealed class RegexStringConstraint : RegexStringConstraintBase
    {
        public RegexStringConstraint()
            : base("^Foo", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture, TimeSpan.FromMilliseconds(79))
        {
            // Nothing to do
        }
    }
}