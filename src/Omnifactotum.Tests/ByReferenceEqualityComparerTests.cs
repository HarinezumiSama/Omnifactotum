using System;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ByReferenceEqualityComparer<>))]
    internal sealed class ByReferenceEqualityComparerTests
    {
        [Test]
        public void TestReferenceType()
        {
            Assert.That(typeof(SomeReferenceType).IsClass, Is.True);

            var testee = ByReferenceEqualityComparer<SomeReferenceType>.Instance;

            Assert.That(testee.Equals(null, null), Is.True);

            const string ValueA = "A";
            const string ValueB = "B";

            var objA1 = new SomeReferenceType(ValueA);
            var objA2 = new SomeReferenceType(ValueA);
            var objB = new SomeReferenceType(ValueB);

            Assert.That(testee.Equals(objA1, objA1), Is.True);
            Assert.That(testee.GetHashCode(objA1), Is.EqualTo(testee.GetHashCode(objA1)));

            Assert.That(testee.Equals(objA1, objA2), Is.False);
            Assert.That(testee.GetHashCode(objA1), Is.Not.EqualTo(testee.GetHashCode(objA2)));

            Assert.That(testee.Equals(objA1, objB), Is.False);
            Assert.That(testee.GetHashCode(objA1), Is.Not.EqualTo(testee.GetHashCode(objB)));

            Assert.That(testee.Equals(objA1, null), Is.False);
            Assert.That(testee.Equals(objA2, null), Is.False);
            Assert.That(testee.Equals(objB, null), Is.False);
        }

        private sealed class SomeReferenceType : IEquatable<SomeReferenceType>
        {
            private static readonly StringComparer ValueComparer = StringComparer.Ordinal;

            private readonly string? _value;

            public SomeReferenceType(string? value) => _value = value;

            public override bool Equals(object? obj) => Equals(obj as SomeReferenceType);

            public override int GetHashCode() => _value is null ? 0 : ValueComparer.GetHashCode(_value);

            public bool Equals(SomeReferenceType? other) => other is not null && ValueComparer.Equals(_value, other._value);
        }
    }
}