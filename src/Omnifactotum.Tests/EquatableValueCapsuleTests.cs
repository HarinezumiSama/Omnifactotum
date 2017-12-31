using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class EquatableValueCapsuleTests
    {
        [Test]
        public void TestConstruction()
        {
            const string Value = "74eb9ddd3c6d496e9f84ceeb765412cd";
            var capsule = new CaseInsensitiveStringEquatableValueCapsule(Value);
            Assert.That(capsule.Value, Is.EqualTo(Value));
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
        [TestCase(
            null,
            null,
            AssertEqualityExpectation.EqualAndMayBeSame,
            AssertEqualityExpectation.EqualAndMayBeSame)]
        [TestCase(
            null,
            "Value",
            AssertEqualityExpectation.NotEqual,
            AssertEqualityExpectation.NotEqual)]
        [TestCase(
            "Value",
            "Value",
            AssertEqualityExpectation.EqualAndCannotBeSame,
            AssertEqualityExpectation.EqualAndCannotBeSame)]
        [TestCase(
            "Value",
            "vALUE",
            AssertEqualityExpectation.EqualAndCannotBeSame,
            AssertEqualityExpectation.NotEqual)]
        [TestCase(
            "Value",
            "V@lue",
            AssertEqualityExpectation.NotEqual,
            AssertEqualityExpectation.NotEqual)]
        public void TestEquality(
            string value1,
            string value2,
            AssertEqualityExpectation caseInsensitiveStringExpectation,
            AssertEqualityExpectation regularStringExpectation)
        {
            var caseInsensitiveCapsule1 = new CaseInsensitiveStringEquatableValueCapsule(value1);
            var caseInsensitiveCapsule2 = new CaseInsensitiveStringEquatableValueCapsule(value2);

            NUnitFactotum.AssertEquality(
                caseInsensitiveCapsule1,
                caseInsensitiveCapsule2,
                caseInsensitiveStringExpectation);

            var regularCapsule1 = new RegularStringEquatableValueCapsule(value1);
            var regularCapsule2 = new RegularStringEquatableValueCapsule(value2);

            NUnitFactotum.AssertEquality(regularCapsule1, regularCapsule2, regularStringExpectation);
        }

        private sealed class RegularStringEquatableValueCapsule : EquatableValueCapsule<string>
        {
            public RegularStringEquatableValueCapsule([CanBeNull] string value)
                : base(value)
            {
                // Nothing to do
            }
        }

        private sealed class CaseInsensitiveStringEquatableValueCapsule : EquatableValueCapsule<string>
        {
            public CaseInsensitiveStringEquatableValueCapsule([CanBeNull] string value)
                : base(value)
            {
                // Nothing to do
            }

            protected override IEqualityComparer<string> GetValueEqualityComparer()
                => StringComparer.OrdinalIgnoreCase;
        }
    }
}