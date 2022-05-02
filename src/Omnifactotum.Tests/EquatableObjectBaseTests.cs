#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(EquatableObjectBase))]
    internal sealed class EquatableObjectBaseTests
    {
#pragma warning disable CS1718 // (Comparison made to same variable) - Testing the overloaded operators
        [Test]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        public void TestObjectMethods()
        {
            const int Value1 = 17;
            const int Value2 = 19;

            Assert.That(Value1, Is.Not.EqualTo(Value2));

            var obj1A = new TestDescendant(Value1);
            var obj1B = new TestDescendant(Value1);
            var obj2 = new TestDescendant(Value2);
            var objOther = new OtherTestDescendant();

            NUnitFactotum.AssertEquality(obj1A, null, AssertEqualityExpectation.NotEqual);

            NUnitFactotum.AssertEquality(obj1A, obj1A, AssertEqualityExpectation.EqualAndMayBeSame);
            Assert.That(obj1A == obj1A, Is.True);
            Assert.That(obj1A != obj1A, Is.False);

            NUnitFactotum.AssertEquality(obj1B, obj1B, AssertEqualityExpectation.EqualAndMayBeSame);
            Assert.That(obj1B == obj1B, Is.True);
            Assert.That(obj1B != obj1B, Is.False);

            NUnitFactotum.AssertEquality(obj2, obj2, AssertEqualityExpectation.EqualAndMayBeSame);
            Assert.That(obj2 == obj2, Is.True);
            Assert.That(obj2 != obj2, Is.False);

            NUnitFactotum.AssertEquality(objOther, objOther, AssertEqualityExpectation.EqualAndMayBeSame);
            Assert.That(objOther == objOther, Is.True);
            Assert.That(objOther != objOther, Is.False);

            NUnitFactotum.AssertEquality(obj1A, obj1B, AssertEqualityExpectation.EqualAndCannotBeSame);
            Assert.That(obj1A == obj1B, Is.True);
            Assert.That(obj1A != obj1B, Is.False);

            NUnitFactotum.AssertEquality(obj1A, obj2, AssertEqualityExpectation.NotEqual);
            Assert.That(obj1A == obj2, Is.False);
            Assert.That(obj1A != obj2, Is.True);

            NUnitFactotum.AssertEquality(obj1B, obj2, AssertEqualityExpectation.NotEqual);
            Assert.That(obj1B == obj2, Is.False);
            Assert.That(obj1B != obj2, Is.True);

            Assert.That(() => objOther.Equals(obj1A), Is.False);
            Assert.That(() => obj1A.Equals(objOther), Is.False);

            //// ReSharper disable SuspiciousTypeConversion.Global
            Assert.That(() => ((object)objOther).Equals(obj1A), Is.False);
            Assert.That(() => ((object)obj1A).Equals(objOther), Is.False);
            //// ReSharper restore SuspiciousTypeConversion.Global

            Assert.That(() => objOther == obj1A, Is.False);
            Assert.That(() => obj1A == objOther, Is.False);
            Assert.That(() => objOther != obj1A, Is.True);
            Assert.That(() => obj1A != objOther, Is.True);
        }
#pragma warning restore CS1718

        private sealed class TestDescendant : EquatableObjectBase
        {
            internal TestDescendant(int value) => Value = value;

            private int Value { get; }

            protected override int GetHashCodeInternal() => Value.GetHashCode();

            protected override bool EqualsInternal(EquatableObjectBase other) => other is TestDescendant castOther && Value == castOther.Value;
        }

        private sealed class OtherTestDescendant : EquatableObjectBase
        {
            protected override int GetHashCodeInternal() => 0;

            protected override bool EqualsInternal(EquatableObjectBase other) => throw new InvalidOperationException();
        }
    }
}