using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalStringMinLengthConstraintBase))]
internal sealed class OptionalStringMinLengthConstraintBaseTests
    : TypedConstraintTestsBase<OptionalStringMinLengthConstraintBaseTests.MinLengthConstraint, string?>
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
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentOutOfRangeException.ParamName)).EqualTo("minLength"));

    protected override IEnumerable<string> GetTypedValidValues()
    {
        yield return null!;
        yield return "fox";
        yield return "wolf";
        yield return "snake";
        yield return "rabbit";
    }

    protected override IEnumerable<string> GetTypedInvalidValues()
    {
        yield return string.Empty;
        yield return "a";
        yield return "ox";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => $"The string value may be null, but otherwise its length must be at least 3. Actual value {
            (invalidValue is null ? "is null" : $"length is {invalidValue.Length}")}.";

    internal sealed class MinLengthConstraint() : OptionalStringMinLengthConstraintBase(3);

    private sealed class ConfigurableConstraint(int length) : OptionalStringMinLengthConstraintBase(length);
}