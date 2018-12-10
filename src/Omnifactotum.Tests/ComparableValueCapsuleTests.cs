using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ComparableValueCapsule<>))]
    internal sealed class ComparableValueCapsuleTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("5472de5169534ddc84226f8dbeb6a998")]
        public void TestConstruction(string value)
        {
            var capsule = new CaseInsensitiveStringComparableValueCapsule(value);
            Assert.That(capsule.Value, Is.EqualTo(value));
        }

        [Test]
        public void TestValuePropertyIsReadOnly()
        {
            var valuePropertyInfo = typeof(EquatableValueCapsule<>).GetProperty(
                nameof(EquatableValueCapsule<object>.Value),
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(valuePropertyInfo, Is.Not.Null);

            Assert.That(valuePropertyInfo.GetGetMethod(false), Is.Not.Null);
            Assert.That(valuePropertyInfo.GetSetMethod(true), Is.Null);
        }

        [Test]
        [TestCase(null, null, 0, 0)]
        [TestCase(null, @"Value", -1, -1)]
        [TestCase(@"Value", null, 1, 1)]
        [TestCase(@"Value", @"Value", 0, 0)]
        //// ReSharper disable once StringLiteralTypo :: Test value
        [TestCase(@"Value", @"vALUE", 0, 1)]
        //// ReSharper disable once StringLiteralTypo :: Test value
        [TestCase(@"vALUE", @"Value", 0, -1)]
        [TestCase(@"Value", @"V@lue", 1, 1)]
        [TestCase(@"V@lue", @"Value", -1, -1)]
        public void TestEqualityAndComparison(
            string value1,
            string value2,
            int caseInsensitiveStringComparisonResult,
            int regularStringComparisonResult)
        {
            var caseInsensitiveCapsule1 = new CaseInsensitiveStringComparableValueCapsule(value1);
            var caseInsensitiveCapsule2 = new CaseInsensitiveStringComparableValueCapsule(value2);

            NUnitFactotum.AssertEquality(
                caseInsensitiveCapsule1,
                caseInsensitiveCapsule2,
                caseInsensitiveStringComparisonResult == 0
                    ? AssertEqualityExpectation.EqualAndCannotBeSame
                    : AssertEqualityExpectation.NotEqual);

            Assert.That(
                caseInsensitiveCapsule1.CompareTo(caseInsensitiveCapsule2),
                Is.EqualTo(caseInsensitiveStringComparisonResult));

            Assert.That(
                caseInsensitiveCapsule1 < caseInsensitiveCapsule2,
                Is.EqualTo(caseInsensitiveStringComparisonResult < 0));

            Assert.That(
                caseInsensitiveCapsule1 <= caseInsensitiveCapsule2,
                Is.EqualTo(caseInsensitiveStringComparisonResult <= 0));

            Assert.That(
                caseInsensitiveCapsule1 > caseInsensitiveCapsule2,
                Is.EqualTo(caseInsensitiveStringComparisonResult > 0));

            Assert.That(
                caseInsensitiveCapsule1 >= caseInsensitiveCapsule2,
                Is.EqualTo(caseInsensitiveStringComparisonResult >= 0));

            var regularCapsule1 = new RegularStringComparableValueCapsule(value1);
            var regularCapsule2 = new RegularStringComparableValueCapsule(value2);

            NUnitFactotum.AssertEquality(
                regularCapsule1,
                regularCapsule2,
                regularStringComparisonResult == 0
                    ? AssertEqualityExpectation.EqualAndCannotBeSame
                    : AssertEqualityExpectation.NotEqual);

            Assert.That(regularCapsule1.CompareTo(regularCapsule2), Is.EqualTo(regularStringComparisonResult));
            Assert.That(regularCapsule1 < regularCapsule2, Is.EqualTo(regularStringComparisonResult < 0));
            Assert.That(regularCapsule1 <= regularCapsule2, Is.EqualTo(regularStringComparisonResult <= 0));
            Assert.That(regularCapsule1 > regularCapsule2, Is.EqualTo(regularStringComparisonResult > 0));
            Assert.That(regularCapsule1 >= regularCapsule2, Is.EqualTo(regularStringComparisonResult >= 0));
        }

        [Test]
        public void TestComparisonForCornerCases()
        {
            const string Value = "d9ba7a0b21a449e6a8bb1c4baca778c3";

            Assert.That(new RegularStringComparableValueCapsule(Value).CompareTo(null), Is.GreaterThan(0));
            Assert.That(new CaseInsensitiveStringComparableValueCapsule(Value).CompareTo(null), Is.GreaterThan(0));

            Assert.That(
                () =>
                    new RegularStringComparableValueCapsule(Value).CompareTo(
                        new CaseInsensitiveStringComparableValueCapsule(Value)),
                Throws.ArgumentException);
        }

        private sealed class RegularStringComparableValueCapsule : ComparableValueCapsule<string>
        {
            public RegularStringComparableValueCapsule([CanBeNull] string value)
                : base(value)
            {
                // Nothing to do
            }
        }

        private sealed class CaseInsensitiveStringComparableValueCapsule : ComparableValueCapsule<string>
        {
            private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

            public CaseInsensitiveStringComparableValueCapsule([CanBeNull] string value)
                : base(value)
            {
                // Nothing to do
            }

            protected override IEqualityComparer<string> GetValueEqualityComparer() => Comparer;

            protected override IComparer<string> GetValueComparer() => Comparer;
        }
    }
}