using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(RegexStringConstraintBase))]
#pragma warning restore CS0618 // Type or member is obsolete
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
    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => new(
            $"The value {(invalidValue is null ? "null" : $"\"{invalidValue}\"")} does not meet the validation pattern.",
            $"The value {(invalidValue is null ? "null" : $"\"{invalidValue}\"")
            } does not match the regular expression pattern \"^Foo\" (options: ExplicitCapture, Compiled, Singleline).");

#pragma warning disable CS0618 // Type or member is obsolete
    internal sealed class RegexStringConstraint : RegexStringConstraintBase
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public const string Pattern = "^Foo";
        public const RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        public static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(79);

        public RegexStringConstraint()
            : base(Pattern, Options, Timeout)
        {
            // Nothing to do
        }
    }
}