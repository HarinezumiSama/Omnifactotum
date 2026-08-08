using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullStringLengthRangeConstraintBase))]
internal sealed class NotNullStringLengthRangeConstraintBaseTests
    : TypedConstraintTestsBase<NotNullStringLengthRangeConstraintBaseTests.LengthRangeConstraint, string>
{
    [Test]
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(0, int.MaxValue)]
    [TestCase(1, 1)]
    [TestCase(1, int.MaxValue)]
    [TestCase(int.MaxValue, int.MaxValue)]
    public void TestWhenValidLengthThenSucceeds(int minLength, int maxLength)
        => Assert.That(() => new ConfigurableConstraint(minLength, maxLength), Throws.Nothing);

    [Test]
    [TestCase(int.MinValue, 0, "minLength")]
    [TestCase(int.MinValue, 1, "minLength")]
    [TestCase(int.MinValue, int.MaxValue, "minLength")]
    [TestCase(-1, 0, "minLength")]
    [TestCase(-1, 1, "minLength")]
    [TestCase(-1, int.MaxValue, "minLength")]
    [TestCase(0, -1, "maxLength")]
    [TestCase(0, int.MinValue, "maxLength")]
    public void TestWhenInvalidLengthThenThrows(int minLength, int maxLength, string parameterName)
        => Assert.That(
            () => new ConfigurableConstraint(minLength, maxLength),
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentOutOfRangeException.ParamName)).EqualTo(parameterName));

    [Test]
    [TestCase(1, 0, "'maxLength' must be greater than or equal to 'minLength' (minLength = 1, maxLength = 0).")]
    [TestCase(2, 1, "'maxLength' must be greater than or equal to 'minLength' (minLength = 2, maxLength = 1).")]
    [TestCase(17, 13, "'maxLength' must be greater than or equal to 'minLength' (minLength = 17, maxLength = 13).")]
    public void TestWhenInvalidMinMaxLengthCombinationThenThrows(int minLength, int maxLength, string expectedMessageStartsWith)
        => Assert.That(
            () => new ConfigurableConstraint(minLength, maxLength),
            Throws.TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property(nameof(ArgumentOutOfRangeException.ParamName))
                .EqualTo("maxLength")
                .And
                .Message
                .StartsWith(expectedMessageStartsWith));

    protected override IEnumerable<string> GetTypedValidValues()
    {
        yield return "ox";
        yield return "fox";
        yield return "wolf";
        yield return "snake";
    }

    protected override IEnumerable<string> GetTypedInvalidValues()
    {
        yield return null!;
        yield return string.Empty;
        yield return "a";
        yield return "rabbit";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue)
        => $"The string value must not be null, and its length must be at least 2 and at most 5. Actual value {
            (invalidValue is null ? "is null" : $"length is {invalidValue.Length}")}.";

    internal sealed class LengthRangeConstraint() : NotNullStringLengthRangeConstraintBase(2, 5);

    private sealed class ConfigurableConstraint(int minLength, int maxLength) : NotNullStringLengthRangeConstraintBase(minLength, maxLength);
}