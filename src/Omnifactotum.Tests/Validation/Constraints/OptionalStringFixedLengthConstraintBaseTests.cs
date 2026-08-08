using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalStringFixedLengthConstraintBase))]
internal sealed class OptionalStringFixedLengthConstraintBaseTests
    : TypedConstraintTestsBase<OptionalStringFixedLengthConstraintBaseTests.FixedLengthConstraint, string?>
{
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(int.MaxValue)]
    public void TestWhenValidLengthThenSucceeds(int length) => Assert.That(() => new ConfigurableConstraint(length), Throws.Nothing);

    [Test]
    [TestCase(int.MinValue)]
    [TestCase(-1)]
    public void TestWhenInvalidLengthThenThrows(int length)
        => Assert.That(
            () => new ConfigurableConstraint(length),
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentOutOfRangeException.ParamName)).EqualTo("length"));

    protected override IEnumerable<string> GetTypedValidValues()
    {
        yield return null!;
        yield return "fox";
        yield return "fOx";
        yield return "FoX";
        yield return "FOX";
    }

    protected override IEnumerable<string> GetTypedInvalidValues()
    {
        yield return string.Empty;
        yield return "a";
        yield return "ox";
        yield return "wolf";
        yield return "snake";
        yield return "rabbit";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => $"The string value may be null, but otherwise its length must be exactly 3. Actual value {
            (invalidValue is null ? "is null" : $"length is {invalidValue.Length}")}.";

    internal sealed class FixedLengthConstraint() : OptionalStringFixedLengthConstraintBase(3);

    private sealed class ConfigurableConstraint(int length) : OptionalStringFixedLengthConstraintBase(length);
}