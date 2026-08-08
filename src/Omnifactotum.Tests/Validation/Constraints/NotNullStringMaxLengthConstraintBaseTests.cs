using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullStringMaxLengthConstraintBase))]
internal sealed class NotNullStringMaxLengthConstraintBaseTests
    : TypedConstraintTestsBase<NotNullStringMaxLengthConstraintBaseTests.MaxLengthConstraint, string>
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
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentOutOfRangeException.ParamName)).EqualTo("maxLength"));

    protected override IEnumerable<string> GetTypedValidValues()
    {
        yield return string.Empty;
        yield return "a";
        yield return "ox";
        yield return "fox";
        yield return "wolf";
    }

    protected override IEnumerable<string> GetTypedInvalidValues()
    {
        yield return null!;
        yield return "snake";
        yield return "rabbit";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => $"The string value must not be null, and its length must be at most 4. Actual value {
            (invalidValue is null ? "is null" : $"length is {invalidValue.Length}")}.";

    internal sealed class MaxLengthConstraint() : NotNullStringMaxLengthConstraintBase(4);

    private sealed class ConfigurableConstraint(int length) : NotNullStringMaxLengthConstraintBase(length);
}