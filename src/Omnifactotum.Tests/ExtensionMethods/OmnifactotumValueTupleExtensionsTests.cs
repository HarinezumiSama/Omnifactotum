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
}