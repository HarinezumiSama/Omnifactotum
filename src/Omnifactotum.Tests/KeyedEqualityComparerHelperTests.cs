using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    //// ReSharper disable AssignNullToNotNullAttribute - for negative test cases

    [TestFixture(TestOf = typeof(KeyedEqualityComparer))]
    internal sealed class KeyedEqualityComparerHelperTests
    {
        [Test]
        public void TestConstruction()
        {
            //// ReSharper disable once ConvertToLocalFunction
            Func<string, int> keySelector = s => s.Length;

            var instance = KeyedEqualityComparer.For<string>.Create(keySelector);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
        }

        [Test]
        public void TestConstructionNegative()
        {
            Assert.That(
                () => KeyedEqualityComparer.For<string>.Create<int>(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestConstructionWithKeyComparer()
        {
            //// ReSharper disable once ConvertToLocalFunction
            Func<int, string> keySelector = i => i.ToString(CultureInfo.InvariantCulture);
            var keyComparer = StringComparer.OrdinalIgnoreCase;

            var instance = KeyedEqualityComparer.For<int>.Create(keySelector, keyComparer);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
            Assert.That(instance.KeyComparer, Is.SameAs(keyComparer));
        }

        [Test]
        public void TestConstructionWithNullKeyComparer()
        {
            //// ReSharper disable once ConvertToLocalFunction
            Func<int, string> keySelector = i => i.ToString(CultureInfo.InvariantCulture);

            var instance = KeyedEqualityComparer.For<int>.Create(keySelector, null);
            Assert.That(instance.KeySelector, Is.SameAs(keySelector));
            Assert.That(instance.KeyComparer, Is.EqualTo(EqualityComparer<string>.Default));
        }

        [Test]
        public void TestConstructionWithKeyComparerNegative()
        {
            Assert.That(
                () => KeyedEqualityComparer.For<int>.Create(null, StringComparer.Ordinal),
                Throws.TypeOf<ArgumentNullException>());
        }
    }
}