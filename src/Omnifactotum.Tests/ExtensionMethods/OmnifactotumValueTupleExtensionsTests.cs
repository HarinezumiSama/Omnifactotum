using System;
using NUnit.Framework;
using Omnifactotum.ExtensionMethods;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumValueTupleExtensions))]
internal sealed class OmnifactotumValueTupleExtensionsTests
{
    [Test]
    [TestCase(23, "value1")]
    [TestCase("value2", -17)]
    public void TestToKeyValuePair<TKey, TValue>(TKey key, TValue value)
    {
        var pair = ValueTuple.Create(key, value).ToKeyValuePair();
        Assert.That(pair.Key, Is.EqualTo(key));
        Assert.That(pair.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase(-23, "value3")]
    [TestCase("value4", 17)]
    public void TestToDictionaryEntry<TKey, TValue>(TKey key, TValue value)
        where TKey : notnull
    {
        var entry = ValueTuple.Create(key, value).ToDictionaryEntry();
        Assert.That(entry.Key, Is.EqualTo(key));
        Assert.That(entry.Value, Is.EqualTo(value));
    }

    [Test]
    [TestCase('a', 'z')]
    [TestCase(17, 29)]
    [TestCase("qaz", "wsx")]
    public void TestToValueRange<T>(T item1, T item2)
        where T : IComparable
    {
        var tuple = ValueTuple.Create(item1, item2);
        var range = tuple.ToValueRange();
        Assert.That(range.Lower, Is.EqualTo(item1));
        Assert.That(range.Upper, Is.EqualTo(item2));
    }
}