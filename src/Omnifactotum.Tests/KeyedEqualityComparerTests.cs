using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    //// ReSharper disable AssignNullToNotNullAttribute - for negative test cases

    [TestFixture]
    public sealed class KeyedEqualityComparerTests
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            var absValueIntEqualityComparer = new AbsValueIntEqualityComparer();
            Assert.That(absValueIntEqualityComparer.Equals(1, 2), Is.False);
            Assert.That(absValueIntEqualityComparer.Equals(1, -1), Is.True);

            Assert.That(
                absValueIntEqualityComparer.GetHashCode(-1),
                Is.EqualTo(absValueIntEqualityComparer.GetHashCode(1)));
        }

        [Test]
        public void TestConstruction()
        {
            Func<string, int> keySelector = s => s.Length;

            var testee = new KeyedEqualityComparer<string, int>(keySelector);
            Assert.That(testee.KeySelector, Is.SameAs(keySelector));
        }

        [Test]
        public void TestConstructionNegative()
        {
            Assert.That(() => new KeyedEqualityComparer<string, int>(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestConstructionWithKeyComparer()
        {
            Func<int, string> keySelector = i => i.ToString(CultureInfo.InvariantCulture);
            var keyComparer = StringComparer.OrdinalIgnoreCase;

            var testee = new KeyedEqualityComparer<int, string>(keySelector, keyComparer);
            Assert.That(testee.KeySelector, Is.SameAs(keySelector));
            Assert.That(testee.KeyComparer, Is.SameAs(keyComparer));
        }

        [Test]
        public void TestConstructionWithNullKeyComparer()
        {
            Func<int, string> keySelector = i => i.ToString(CultureInfo.InvariantCulture);

            var testee = new KeyedEqualityComparer<int, string>(keySelector, null);
            Assert.That(testee.KeySelector, Is.SameAs(keySelector));
            Assert.That(testee.KeyComparer, Is.EqualTo(EqualityComparer<string>.Default));
        }

        [Test]
        public void TestConstructionWithKeyComparerNegative()
        {
            Assert.That(
                () => new KeyedEqualityComparer<int, string>(null, StringComparer.Ordinal),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestTypedEquals()
        {
            var testee = new KeyedEqualityComparer<string, int>(int.Parse);
            Assert.That(() => testee.Equals(null, null), Is.True);
            Assert.That(() => testee.Equals("1", "1"), Is.True);
            Assert.That(() => testee.Equals("1", "-1"), Is.False);
            Assert.That(() => testee.Equals("1", "2"), Is.False);
            Assert.That(() => testee.Equals("1", null), Is.False);
            Assert.That(() => testee.Equals(null, "1"), Is.False);
        }

        [Test]
        public void TestTypedEqualsWithKeyComparer()
        {
            var testee = new KeyedEqualityComparer<string, int>(int.Parse, new AbsValueIntEqualityComparer());
            Assert.That(() => testee.Equals(null, null), Is.True);
            Assert.That(() => testee.Equals("1", "1"), Is.True);
            Assert.That(() => testee.Equals("1", "-1"), Is.True);
            Assert.That(() => testee.Equals("1", "2"), Is.False);
            Assert.That(() => testee.Equals("1", null), Is.False);
            Assert.That(() => testee.Equals(null, "1"), Is.False);
        }

        [Test]
        public void TestGenericEquals()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(int.Parse);

            Assert.That(() => testee.Equals(null, null), Is.True);
            Assert.That(() => testee.Equals("1", "1"), Is.True);
            Assert.That(() => testee.Equals("1", "-1"), Is.False);
            Assert.That(() => testee.Equals("1", "2"), Is.False);
            Assert.That(() => testee.Equals("1", null), Is.False);
            Assert.That(() => testee.Equals(null, "1"), Is.False);
        }

        [Test]
        public void TestGenericEqualsNegative()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(int.Parse);

            Assert.That(() => testee.Equals(1, "1"), Throws.ArgumentException);
            Assert.That(() => testee.Equals("1", -1), Throws.ArgumentException);
        }

        [Test]
        public void TestGenericEqualsWithKeyComparer()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(
                int.Parse,
                new AbsValueIntEqualityComparer());

            Assert.That(() => testee.Equals(null, null), Is.True);
            Assert.That(() => testee.Equals("1", "1"), Is.True);
            Assert.That(() => testee.Equals("1", "-1"), Is.True);
            Assert.That(() => testee.Equals("1", "2"), Is.False);
            Assert.That(() => testee.Equals("1", null), Is.False);
            Assert.That(() => testee.Equals(null, "1"), Is.False);
        }

        [Test]
        public void TestGenericEqualsWithKeyComparerNegative()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(
                int.Parse,
                new AbsValueIntEqualityComparer());

            Assert.That(() => testee.Equals(1, "1"), Throws.ArgumentException);
            Assert.That(() => testee.Equals("1", -1), Throws.ArgumentException);
        }

        [Test]
        public void TestTypedGetHashCode()
        {
            var testee = new KeyedEqualityComparer<string, int>(int.Parse);
            Assert.That(() => testee.GetHashCode(null), Is.EqualTo(0));
        }

        [Test]
        public void TestTypedGetHashCodeWithKeyComparer()
        {
            var testee = new KeyedEqualityComparer<string, int>(int.Parse, new AbsValueIntEqualityComparer());
            Assert.That(() => testee.GetHashCode(null), Is.EqualTo(0));
            Assert.That(() => testee.GetHashCode("-1"), Is.EqualTo(testee.GetHashCode("1")));
        }

        [Test]
        public void TestGenericGetHashCode()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(int.Parse);
            Assert.That(() => testee.GetHashCode(null), Is.EqualTo(0));
        }

        [Test]
        public void TestGenericGetHashCodeWithKeyComparer()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(
                int.Parse,
                new AbsValueIntEqualityComparer());

            Assert.That(() => testee.GetHashCode(null), Is.EqualTo(0));
            Assert.That(() => testee.GetHashCode("-1"), Is.EqualTo(testee.GetHashCode("1")));
        }

        [Test]
        public void TestGenericGetHashCodeNegative()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(int.Parse);
            Assert.That(() => testee.GetHashCode(1), Throws.ArgumentException);
        }

        [Test]
        public void TestGenericGetHashCodeWithKeyComparerNegative()
        {
            IEqualityComparer testee = new KeyedEqualityComparer<string, int>(
                int.Parse,
                new AbsValueIntEqualityComparer());

            Assert.That(() => testee.GetHashCode(1), Throws.ArgumentException);
        }

        private sealed class AbsValueIntEqualityComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return Math.Abs(x) == Math.Abs(y);
            }

            public int GetHashCode(int obj)
            {
                return Math.Abs(obj).GetHashCode();
            }
        }
    }
}