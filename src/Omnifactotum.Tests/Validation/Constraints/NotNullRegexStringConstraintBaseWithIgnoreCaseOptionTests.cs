using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullRegexStringConstraintBase))]
internal sealed partial class NotNullRegexStringConstraintBaseWithIgnoreCaseOptionTests
    : TypedConstraintTestsBase<NotNullRegexStringConstraintBaseWithIgnoreCaseOptionTests.RegexStringConstraint, string?>
{
    protected override void OnTestConstruction(RegexStringConstraint testee)
    {
        base.OnTestConstruction(testee);

        Assert.That(() => testee.RegexObject.ToString(), Is.EqualTo(RegexStringConstraint.PatternValue));
        Assert.That(() => testee.RegexObject.Options, Is.EqualTo(RegexStringConstraint.OptionsValue));
        Assert.That(() => testee.RegexObject.MatchTimeout, Is.EqualTo(RegexStringConstraint.TimeoutValue));
    }

    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "Foo";
        yield return "foo";
        yield return "FOO";
        yield return "FooBar";
        yield return "Foo\n";
        yield return "fOo\n";
        yield return "Foo\x0020";
        yield return "Foo\t";
        yield return "Foo\x0020something";
        yield return "FoO\x0020something";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
        yield return string.Empty;
        yield return "-Foo";
        yield return "\x0020Foo";
        yield return "\tFoo";
        yield return "\rFoo";
        yield return "\nFoo";
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => new(
            $"[{invalidValue ?? "<NULL>"}] does not start with 'Foo' (ignoring case).",
            $"The value {(invalidValue is null ? "null" : $"\"{invalidValue}\"")
            } does not match the regular expression pattern \"^Foo\" (options: IgnoreCase, ExplicitCapture, Compiled, Singleline).");

    internal sealed partial class RegexStringConstraint : NotNullRegexStringConstraintBase
    {
        public const string PatternValue = "^Foo";
        public const RegexOptions OptionsValue = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;
        public const int TimeoutValueMilliseconds = 83;

        public static readonly TimeSpan TimeoutValue = TimeSpan.FromMilliseconds(TimeoutValueMilliseconds);

        public RegexStringConstraint()
            : base(CreateRegex())
        {
            // Nothing to do
        }

        protected override string GetErrorDetailsText(string? value) => $"[{value ?? "<NULL>"}] does not start with 'Foo' (ignoring case).";

#if NET7_0_OR_GREATER
        [GeneratedRegex(PatternValue, OptionsValue, TimeoutValueMilliseconds)]
        private static partial Regex CreateRegex();
#else
        private static partial Regex CreateRegex();

        private static partial Regex CreateRegex() => new(PatternValue, OptionsValue, TimeoutValue);
#endif
    }
}