using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class ValueRangeTests
    {
        [Test]
        public void TestInvalidConstruction()
        {
            Assert.That(() => new ValueRange<int>(2, 1), Throws.ArgumentException);
            Assert.That(() => ValueRange.Create(2, 1), Throws.ArgumentException);
        }

        [Test]
        [TestCaseSource(typeof(ConstructionCases))]
        public void TestConstruction(int lower, int upper)
        {
            var objExplicit = new ValueRange<int>(lower, upper);
            Assert.That(objExplicit.Lower, Is.EqualTo(lower));
            Assert.That(objExplicit.Upper, Is.EqualTo(upper));

            var objImplicit = ValueRange.Create(lower, upper);
            Assert.That(objImplicit.Lower, Is.EqualTo(lower));
            Assert.That(objImplicit.Upper, Is.EqualTo(upper));

            Assert.That(lower, Is.LessThanOrEqualTo(upper));
        }

        [Test]
        [TestCaseSource(typeof(ContainsValueCases))]
        public void TestContainsValue(ValueRange<int> range, int value, bool expectedResult)
        {
            var actualResult = range.Contains(value);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(typeof(ContainsRangeCases))]
        public void TestContainsRange(ValueRange<int> range, ValueRange<int> otherRange, bool expectedResult)
        {
            var actualResult = range.Contains(otherRange);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(typeof(IntersectsWithCases))]
        public void TestIntersectsWith(ValueRange<int> range, ValueRange<int> otherRange, bool expectedResult)
        {
            Assert.That(range.IntersectsWith(otherRange), Is.EqualTo(expectedResult));
            Assert.That(otherRange.IntersectsWith(range), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(typeof(EqualityCases))]
        public void TestEquality(ValueRange<int> range1, ValueRange<int> range2, bool equal)
        {
            NUnitFactotum.AssertEquality(
                range1,
                range2,
                equal ? AssertEqualityExpectation.EqualAndMayBeSame : AssertEqualityExpectation.NotEqual);

            Assert.That(range1 == range2, Is.EqualTo(equal));
            Assert.That(range1 != range2, Is.Not.EqualTo(equal));
        }

        [Test]
        [TestCaseSource(typeof(ToStringCases))]
        public void TestToString(ValueRange<int> range, string expectedString)
        {
            var actualString = range.ToString();
            Assert.That(actualString, Is.EqualTo(expectedString));
        }

        private sealed class ConstructionCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(1, 2);
                yield return new TestCaseData(int.MinValue, int.MaxValue);
            }
        }

        private sealed class ContainsValueCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(ValueRange.Create(1, 3), 0, false);
                yield return new TestCaseData(ValueRange.Create(1, 3), 1, true);
                yield return new TestCaseData(ValueRange.Create(1, 3), 2, true);
                yield return new TestCaseData(ValueRange.Create(1, 3), 3, true);
                yield return new TestCaseData(ValueRange.Create(1, 3), 4, false);
            }
        }

        private sealed class ContainsRangeCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(2, 4), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(0, 2), false);

                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 1), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(2, 2), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(3, 3), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 2), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 3), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(2, 3), true);
            }
        }

        private sealed class IntersectsWithCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(-1, 0), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(0, 0), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(4, 4), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(4, 5), false);

                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(-1, 1), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(-1, 2), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(-1, 3), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(-1, 4), true);

                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 1), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(2, 2), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(3, 3), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 3), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(3, 4), true);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(3, 5), true);
            }
        }

        private sealed class EqualityCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(new ValueRange<int>(), new ValueRange<int>(), true);

                yield return new TestCaseData(ValueRange.Create(1, 1), ValueRange.Create(1, 1), true);
                yield return new TestCaseData(ValueRange.Create(1, 2), ValueRange.Create(1, 2), true);
                yield return new TestCaseData(ValueRange.Create(-10, 3), ValueRange.Create(-10, 3), true);

                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(1, 1), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(2, 2), false);
                yield return new TestCaseData(ValueRange.Create(1, 3), ValueRange.Create(3, 3), false);
            }
        }

        private sealed class ToStringCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(new ValueRange<int>(), "[0; 0]");
                yield return new TestCaseData(new ValueRange<int>(1, 2), "[1; 2]");
                yield return new TestCaseData(new ValueRange<int>(-5, 4), "[-5; 4]");
            }
        }
    }
}