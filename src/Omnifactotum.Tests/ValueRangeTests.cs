#nullable enable

using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueRange<>))]
    [TestFixture(TestOf = typeof(ValueRange))]
    internal sealed class ValueRangeTests
    {
        [Test]
        [TestCase(2, 1, @"The lower boundary (2) cannot be greater than the upper boundary (1).")]
        [TestCase(0, -1, @"The lower boundary (0) cannot be greater than the upper boundary (-1).")]
        [TestCase(int.MaxValue, int.MaxValue - 1, @"The lower boundary (2147483647) cannot be greater than the upper boundary (2147483646).")]
        public void TestInvalidConstruction(int lower, int upper, string expectedExceptionMessage)
        {
            Assert.That(lower, Is.GreaterThan(upper));

            Assert.That(() => new ValueRange<int>(lower, upper), Throws.ArgumentException.With.Message.EqualTo(expectedExceptionMessage));
            Assert.That(() => ValueRange.Create(lower, upper), Throws.ArgumentException.With.Message.EqualTo(expectedExceptionMessage));
        }

        [Test]
        [TestCase(1, 2)]
        [TestCase(-17, 42)]
        [TestCase(int.MinValue, int.MinValue)]
        [TestCase(int.MinValue, int.MaxValue)]
        [TestCase(int.MaxValue, int.MaxValue)]
        public void TestConstruction(int lower, int upper)
        {
            Assert.That(lower, Is.LessThanOrEqualTo(upper));

            var objExplicit = new ValueRange<int>(lower, upper);
            Assert.That(objExplicit.Lower, Is.EqualTo(lower));
            Assert.That(objExplicit.Upper, Is.EqualTo(upper));

            var objImplicit = ValueRange.Create(lower, upper);
            Assert.That(objImplicit.Lower, Is.EqualTo(lower));
            Assert.That(objImplicit.Upper, Is.EqualTo(upper));
        }

        [Test]
        [TestCase(1, 3, 0, false)]
        [TestCase(1, 3, 1, true)]
        [TestCase(1, 3, 2, true)]
        [TestCase(1, 3, 3, true)]
        [TestCase(1, 3, 4, false)]
        public void TestContainsValue(int rangeLower, int rangeUpper, int value, bool expectedResult)
        {
            var range = new ValueRange<int>(rangeLower, rangeUpper);
            Assert.That(() => range.Contains(value), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(1, 3, 2, 4, false)]
        [TestCase(1, 3, 0, 2, false)]
        [TestCase(1, 3, 1, 1, true)]
        [TestCase(1, 3, 2, 2, true)]
        [TestCase(1, 3, 3, 3, true)]
        [TestCase(1, 3, 1, 2, true)]
        [TestCase(1, 3, 1, 3, true)]
        [TestCase(1, 3, 2, 3, true)]
        public void TestContainsRange(int rangeLower, int rangeUpper, int otherRangeLower, int otherRangeUpper, bool expectedResult)
        {
            var range = new ValueRange<int>(rangeLower, rangeUpper);
            var otherRange = new ValueRange<int>(otherRangeLower, otherRangeUpper);

            Assert.That(() => range.Contains(otherRange), Is.EqualTo(expectedResult));
            Assert.That(() => range.Contains(in otherRange), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(1, 3, -1, 0, false)]
        [TestCase(1, 3, 0, 0, false)]
        [TestCase(1, 3, 4, 4, false)]
        [TestCase(1, 3, 4, 5, false)]
        [TestCase(1, 3, -1, 1, true)]
        [TestCase(1, 3, -1, 2, true)]
        [TestCase(1, 3, -1, 3, true)]
        [TestCase(1, 3, -1, 4, true)]
        [TestCase(1, 3, 1, 1, true)]
        [TestCase(1, 3, 2, 2, true)]
        [TestCase(1, 3, 3, 3, true)]
        [TestCase(1, 3, 1, 3, true)]
        [TestCase(1, 3, 3, 4, true)]
        [TestCase(1, 3, 3, 5, true)]
        public void TestIntersectsWith(int rangeLower, int rangeUpper, int otherRangeLower, int otherRangeUpper, bool expectedResult)
        {
            var range = new ValueRange<int>(rangeLower, rangeUpper);
            var otherRange = new ValueRange<int>(otherRangeLower, otherRangeUpper);

            Assert.That(() => range.IntersectsWith(otherRange), Is.EqualTo(expectedResult));
            Assert.That(() => range.IntersectsWith(in otherRange), Is.EqualTo(expectedResult));
            Assert.That(() => otherRange.IntersectsWith(range), Is.EqualTo(expectedResult));
            Assert.That(() => otherRange.IntersectsWith(in range), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(default(int), default(int), default(int), default(int), true)]
        [TestCase(1, 1, 1, 1, true)]
        [TestCase(1, 2, 1, 2, true)]
        [TestCase(-10, 3, -10, 3, true)]
        [TestCase(1, 3, 1, 1, false)]
        [TestCase(1, 3, 2, 2, false)]
        [TestCase(1, 3, 3, 3, false)]
        public void TestEquality(int rangeLeftLower, int rangeLeftUpper, int rangeRightLower, int rangeRightUpper, bool expectedEqual)
        {
            var rangeLeft = new ValueRange<int>(rangeLeftLower, rangeLeftUpper);
            var rangeRight = new ValueRange<int>(rangeRightLower, rangeRightUpper);

            NUnitFactotum.AssertEquality(
                rangeLeft,
                rangeRight,
                expectedEqual ? AssertEqualityExpectation.EqualAndMayBeSame : AssertEqualityExpectation.NotEqual);

            Assert.That(rangeLeft == rangeRight, Is.EqualTo(expectedEqual));
            Assert.That(rangeLeft != rangeRight, Is.EqualTo(!expectedEqual));
        }

        [Test]
        [TestCase(default(int), default(int), "[0; 0]")]
        [TestCase(1, 2, "[1; 2]")]
        [TestCase(-5, 4, "[-5; 4]")]
        public void TestToString(int rangeLower, int rangeUpper, string expectedString)
        {
            var range = new ValueRange<int>(rangeLower, rangeUpper);
            Assert.That(range.ToString, Is.EqualTo(expectedString));
        }
    }
}