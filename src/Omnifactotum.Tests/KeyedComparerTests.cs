using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(KeyedComparer<,>))]
internal sealed class KeyedComparerTests
{
    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        var absValueIntComparer = new AbsValueIntComparer();
        Assert.That(absValueIntComparer.Compare(1, 2), Is.Negative);
        Assert.That(absValueIntComparer.Compare(2, 1), Is.Positive);
        Assert.That(absValueIntComparer.Compare(1, 1), Is.Zero);
        Assert.That(absValueIntComparer.Compare(1, -1), Is.Zero);
        Assert.That(absValueIntComparer.Compare(-1, 1), Is.Zero);
        Assert.That(absValueIntComparer.Compare(-1, -1), Is.Zero);
    }

    [Test]
    public void TestConstruction()
    {
        int KeySelector(string s) => s.Length;

        var keySelector = KeySelector;

        var testee = new KeyedComparer<string, int>(keySelector);
        Assert.That(testee.KeySelector, Is.SameAs(keySelector));
    }

    [Test]
    public void TestConstructionNegative()
        => Assert.That(() => new KeyedComparer<string, int>(null!), Throws.TypeOf<ArgumentNullException>());

    [Test]
    public void TestConstructionWithKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;
        var keyComparer = StringComparer.OrdinalIgnoreCase;

        var testee = new KeyedComparer<int, string>(keySelector, keyComparer);
        Assert.That(testee.KeySelector, Is.SameAs(keySelector));
        Assert.That(testee.KeyComparer, Is.SameAs(keyComparer));
    }

    [Test]
    public void TestConstructionWithDefaultKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;

        var testee = new KeyedComparer<int, string>(keySelector);
        Assert.That(testee.KeySelector, Is.SameAs(keySelector));
        Assert.That(testee.KeyComparer, Is.EqualTo(Comparer<string>.Default));
    }

    [Test]
    public void TestConstructionWithNullKeyComparer()
    {
        static string KeySelector(int i) => i.ToString(CultureInfo.InvariantCulture);

        var keySelector = KeySelector;

        // ReSharper disable once RedundantArgumentDefaultValue :: Test case
        var testee = new KeyedComparer<int, string>(keySelector, null);
        Assert.That(testee.KeySelector, Is.SameAs(keySelector));
        Assert.That(testee.KeyComparer, Is.EqualTo(Comparer<string>.Default));
    }

    [Test]
    public void TestConstructionWithKeyComparerNegative()
        => Assert.That(() => new KeyedComparer<int, string>(null!, StringComparer.Ordinal), Throws.TypeOf<ArgumentNullException>());

    [Test]
    public void TestTypedEquals()
    {
        var testee = new KeyedComparer<string, int>(int.Parse);
        Assert.That(() => testee.Compare(null, null), Is.Zero);
        Assert.That(() => testee.Compare("1", "1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "-1"), Is.Positive);
        Assert.That(() => testee.Compare("1", "2"), Is.Negative);
        Assert.That(() => testee.Compare("1", null), Is.Positive);
        Assert.That(() => testee.Compare(null, "1"), Is.Negative);
    }

    [Test]
    public void TestTypedEqualsWithKeyComparer()
    {
        var testee = new KeyedComparer<string, int>(int.Parse, new AbsValueIntComparer());
        Assert.That(() => testee.Compare(null, null), Is.Zero);
        Assert.That(() => testee.Compare("1", "1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "-1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "2"), Is.Negative);
        Assert.That(() => testee.Compare("1", null), Is.Positive);
        Assert.That(() => testee.Compare(null, "1"), Is.Negative);
    }

    [Test]
    public void TestGenericEquals()
    {
        IComparer testee = new KeyedComparer<string, int>(int.Parse);
        Assert.That(() => testee.Compare(null, null), Is.Zero);
        Assert.That(() => testee.Compare("1", "1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "-1"), Is.Positive);
        Assert.That(() => testee.Compare("1", "2"), Is.Negative);
        Assert.That(() => testee.Compare("1", null), Is.Positive);
        Assert.That(() => testee.Compare(null, "1"), Is.Negative);
    }

    [Test]
    public void TestGenericEqualsNegative()
    {
        IComparer testee = new KeyedComparer<string, int>(int.Parse);

        Assert.That(
            () => testee.Compare(1L, "1"),
            Throws.ArgumentException
                .With
                .Message
                .EqualTo("Invalid value type \"System.Int64\". (Parameter 'left')")
                .And
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("left"));

        Assert.That(
            () => testee.Compare("1", -1m),
            Throws.ArgumentException
                .With
                .Message
                .EqualTo("Invalid value type \"System.Decimal\". (Parameter 'right')")
                .And
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("right"));
    }

    [Test]
    public void TestGenericEqualsWithKeyComparer()
    {
        IComparer testee = new KeyedComparer<string, int>(int.Parse, new AbsValueIntComparer());
        Assert.That(() => testee.Compare(null, null), Is.Zero);
        Assert.That(() => testee.Compare("1", "1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "-1"), Is.Zero);
        Assert.That(() => testee.Compare("1", "2"), Is.Negative);
        Assert.That(() => testee.Compare("1", null), Is.Positive);
        Assert.That(() => testee.Compare(null, "1"), Is.Negative);
    }

    [Test]
    public void TestGenericEqualsWithKeyComparerNegative()
    {
        IComparer testee = new KeyedComparer<string, int>(int.Parse, new AbsValueIntComparer());

        Assert.That(
            () => testee.Compare(1L, "1"),
            Throws.ArgumentException
                .With
                .Message
                .EqualTo("Invalid value type \"System.Int64\". (Parameter 'left')")
                .And
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("left"));

        Assert.That(
            () => testee.Compare("1", -1m),
            Throws.ArgumentException
                .With
                .Message
                .EqualTo("Invalid value type \"System.Decimal\". (Parameter 'right')")
                .And
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("right"));
    }

    private sealed class AbsValueIntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => Comparer<int>.Default.Compare(Math.Abs(x), Math.Abs(y));
    }
}