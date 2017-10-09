using System;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class ByReferenceEqualityComparerTests
    {
        [Test]
        public void TestReferenceType()
        {
            Assert.That(typeof(SomeReferenceType).IsClass, Is.True);

            var equalityComparer = ByReferenceEqualityComparer<SomeReferenceType>.Instance;

            const string ValueA = "A";
            const string ValueB = "B";

            var objA1 = new SomeReferenceType(ValueA);
            var objA2 = new SomeReferenceType(ValueA);
            var objB = new SomeReferenceType(ValueB);

            Assert.That(equalityComparer.Equals(objA1, objA1), Is.True);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.EqualTo(equalityComparer.GetHashCode(objA1)));

            Assert.That(equalityComparer.Equals(objA1, objA2), Is.False);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.Not.EqualTo(equalityComparer.GetHashCode(objA2)));

            Assert.That(equalityComparer.Equals(objA1, objB), Is.False);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.Not.EqualTo(equalityComparer.GetHashCode(objB)));
        }

        [Test]
        public void TestValueType()
        {
            Assert.That(typeof(SomeValueType).IsValueType, Is.True);

            var equalityComparer = ByReferenceEqualityComparer<SomeValueType>.Instance;

            const string ValueA = "A";
            const string ValueB = "B";

            var objA1 = new SomeValueType(ValueA);
            var objA2 = new SomeValueType(ValueA);
            var objB = new SomeValueType(ValueB);

            Assert.That(equalityComparer.Equals(objA1, objA1), Is.True);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.EqualTo(equalityComparer.GetHashCode(objA1)));

            Assert.That(equalityComparer.Equals(objA1, objA2), Is.True);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.EqualTo(equalityComparer.GetHashCode(objA1)));

            Assert.That(equalityComparer.Equals(objA1, objB), Is.False);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.Not.EqualTo(equalityComparer.GetHashCode(objB)));
        }

        private sealed class SomeReferenceType : IEquatable<SomeReferenceType>
        {
            private readonly string _value;

            public SomeReferenceType(string value)
            {
                _value = value;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SomeReferenceType);
            }

            public override int GetHashCode()
            {
                return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
            }

            public bool Equals(SomeReferenceType other)
            {
                return other != null && StringComparer.Ordinal.Equals(_value, other._value);
            }
        }

        private struct SomeValueType : IEquatable<SomeValueType>
        {
            private readonly string _value;

            public SomeValueType(string value)
            {
                _value = value;
            }

            public override bool Equals(object obj)
            {
                return obj is SomeValueType type && Equals(type);
            }

            public override int GetHashCode()
            {
                return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
            }

            public bool Equals(SomeValueType other)
            {
                return StringComparer.Ordinal.Equals(_value, other._value);
            }
        }
    }
}