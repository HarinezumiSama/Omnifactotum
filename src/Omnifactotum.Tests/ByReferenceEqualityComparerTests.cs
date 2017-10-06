using System;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class ByReferenceEqualityComparerTests
    {
        [Test]
        public void TestReferenceType()
        {
            Assert.That(typeof(SomeReferenceType).IsClass, Is.True);

            var equalityComparer = ByReferenceEqualityComparer<SomeReferenceType>.Instance;

            var objA1 = new SomeReferenceType { Value = "A" };
            var objA2 = new SomeReferenceType { Value = "A" };
            var objB = new SomeReferenceType { Value = "B" };

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

            var objA1 = new SomeValueType { Value = "A" };
            var objA2 = new SomeValueType { Value = "A" };
            var objB = new SomeValueType { Value = "B" };

            Assert.That(equalityComparer.Equals(objA1, objA1), Is.True);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.EqualTo(equalityComparer.GetHashCode(objA1)));

            Assert.That(equalityComparer.Equals(objA1, objA2), Is.True);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.EqualTo(equalityComparer.GetHashCode(objA1)));

            Assert.That(equalityComparer.Equals(objA1, objB), Is.False);
            Assert.That(equalityComparer.GetHashCode(objA1), Is.Not.EqualTo(equalityComparer.GetHashCode(objB)));
        }

        private sealed class SomeReferenceType : IEquatable<SomeReferenceType>
        {
            public string Value
            {
                private get;
                set;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as SomeReferenceType);
            }

            public override int GetHashCode()
            {
                return this.Value == null ? 0 : StringComparer.Ordinal.GetHashCode(this.Value);
            }

            public bool Equals(SomeReferenceType other)
            {
                return other != null && StringComparer.Ordinal.Equals(this.Value, other.Value);
            }
        }

        private struct SomeValueType : IEquatable<SomeValueType>
        {
            public string Value
            {
                private get;
                set;
            }

            public override bool Equals(object obj)
            {
                return obj is SomeValueType && Equals((SomeValueType)obj);
            }

            public override int GetHashCode()
            {
                return this.Value == null ? 0 : StringComparer.Ordinal.GetHashCode(this.Value);
            }

            public bool Equals(SomeValueType other)
            {
                return StringComparer.Ordinal.Equals(this.Value, other.Value);
            }
        }
    }
}