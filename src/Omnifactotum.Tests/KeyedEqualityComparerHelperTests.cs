using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(KeyedEqualityComparer))]
    internal sealed class KeyedEqualityComparerHelperTests
    {
        [Test]
        public void TestConstruction()
        {
            int KeySelector(string s) => s.Length;

            Func<string, int> keySelector = KeySelector;

            var instance = KeyedEqualityComparer.For<string>.Create(keySelector);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
        }

        [Test]
        public void TestConstructionNegative()
            => Assert.That(() => KeyedEqualityComparer.For<string>.Create<int>(null!), Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void TestConstructionWithKeyComparer()
        {
            static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

            Func<int, string> keySelector = KeySelector;
            var keyComparer = StringComparer.OrdinalIgnoreCase;

            var instance = KeyedEqualityComparer.For<int>.Create(keySelector, keyComparer);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
            Assert.That(instance.KeyComparer, Is.SameAs(keyComparer));
        }

        [Test]
        public void TestConstructionWithDefaultKeyComparer()
        {
            static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

            Func<int, string> keySelector = KeySelector;

            var instance = KeyedEqualityComparer.For<int>.Create(keySelector);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
            Assert.That(instance.KeyComparer, Is.EqualTo(EqualityComparer<string>.Default));
        }

        [Test]
        public void TestConstructionWithNullKeyComparer()
        {
            static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

            Func<int, string> keySelector = KeySelector;

            // ReSharper disable once RedundantArgumentDefaultValue :: Test case
            var instance = KeyedEqualityComparer.For<int>.Create(keySelector, null);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
            Assert.That(instance.KeyComparer, Is.EqualTo(EqualityComparer<string>.Default));
        }

        [Test]
        public void TestConstructionWithKeyComparerNegative()
            => Assert.That(
                () => KeyedEqualityComparer.For<int>.Create(null!, StringComparer.Ordinal),
                Throws.TypeOf<ArgumentNullException>());
    }
}