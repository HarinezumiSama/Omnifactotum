using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(KeyedComparer))]
internal sealed class KeyedComparerHelperTests
{
    [Test]
    public void TestConstructionNegative()
        => Assert.That(() => KeyedComparer.For<string>.Create<int>(null!), Throws.TypeOf<ArgumentNullException>());

    [Test]
    public void TestConstructionWithKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;
        var keyComparer = StringComparer.OrdinalIgnoreCase;

        var instance = KeyedComparer.For<int>.Create(keySelector, keyComparer);
        Assert.That(instance.KeySelector, Is.SameAs(keySelector));
        Assert.That(instance.KeyComparer, Is.SameAs(keyComparer));
    }

    [Test]
    public void TestConstructionWithDefaultKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;

        var instance = KeyedComparer.For<int>.Create(keySelector);
        Assert.That(instance.KeySelector, Is.SameAs(keySelector));
        Assert.That(instance.KeyComparer, Is.EqualTo(Comparer<string>.Default));
    }

    [Test]
    public void TestConstructionWithNullKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;

        // ReSharper disable once RedundantArgumentDefaultValue :: Test case
        var instance = KeyedComparer.For<int>.Create(keySelector, null);
        Assert.That(instance.KeySelector, Is.SameAs(keySelector));
        Assert.That(instance.KeyComparer, Is.EqualTo(Comparer<string>.Default));
    }

    [Test]
    public void TestConstructionWithKeyComparerNegative()
        => Assert.That(
            () => KeyedComparer.For<int>.Create(null!, StringComparer.Ordinal),
            Throws.TypeOf<ArgumentNullException>());
}