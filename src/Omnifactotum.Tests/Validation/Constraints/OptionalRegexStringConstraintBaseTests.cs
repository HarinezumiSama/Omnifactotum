using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalRegexStringConstraintBase))]
internal sealed class OptionalRegexStringConstraintBaseTests : TypedConstraintTestsBase<OptionalRegexStringConstraintBaseTests.RegexStringConstraint, string?>
{
    protected override void OnTestConstruction(RegexStringConstraint testee)
    {
        base.OnTestConstruction(testee);

        Assert.That(() => OptionalRegexStringConstraintBase.DefaultRegexTimeout, Is.Not.EqualTo(Regex.InfiniteMatchTimeout));

        Assert.That(() => testee.RegexObject.ToString(), Is.EqualTo(RegexStringConstraint.PatternValue));
        Assert.That(() => testee.RegexObject.Options, Is.EqualTo(RegexStringConstraint.OptionsValue));
        Assert.That(() => testee.RegexObject.MatchTimeout, Is.EqualTo(RegexStringConstraint.TimeoutValue));
    }

    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return null;
        yield return "Foo";
        yield return "FooBar";
        yield return "Foo\n";
        yield return "Foo\x0020";
        yield return "Foo\t";
        yield return "Foo\x0020something";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
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
            $"The value \"{invalidValue}\" does not meet the validation pattern.",
            $"The value \"{invalidValue
            }\" does not match the regular expression pattern \"^Foo\" (options: ExplicitCapture, Compiled, Singleline).");

    internal sealed class RegexStringConstraint : OptionalRegexStringConstraintBase
    {
        public const string PatternValue = "^Foo";
        public const RegexOptions OptionsValue = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture;

        public static readonly TimeSpan TimeoutValue = TimeSpan.FromMilliseconds(79);

        public RegexStringConstraint()
            : base(PatternValue, OptionsValue, TimeoutValue)
        {
            // Nothing to do
        }
    }
}