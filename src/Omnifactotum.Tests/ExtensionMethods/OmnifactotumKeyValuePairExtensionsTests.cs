using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.ExtensionMethods;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumKeyValuePairExtensions))]
internal sealed class OmnifactotumKeyValuePairExtensionsTests
{
    [Test]
    [TestCase(19, "value1")]
    [TestCase("value2", -13)]
    public void TestToKeyValuePair<TKey, TValue>(TKey key, TValue value)
    {
        var pair = KeyValuePair.Create(key, value).ToValueTuple();
        Assert.That(pair.Item1, Is.EqualTo(key));
        Assert.That(pair.Item2, Is.EqualTo(value));
    }
}