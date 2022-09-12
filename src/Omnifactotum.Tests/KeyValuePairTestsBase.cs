using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture]
internal abstract class KeyValuePairTestsBase
{
    public static IEnumerable<TestCaseData> CreateTestCases
    {
        get
        {
            yield return new TestCaseData(17, @"Foo");
            yield return new TestCaseData("Bar", 42);
        }
    }

    [Test]
    [TestCaseSource(nameof(CreateTestCases))]
    public void TestCreate<TKey, TValue>(TKey key, TValue value)
    {
        //// ReSharper disable once UseDeconstruction
        var pair = CreateTestee(key, value);
        Assert.That(pair.Key, Is.EqualTo(key));
        Assert.That(pair.Value, Is.EqualTo(value));
    }

    protected abstract KeyValuePair<TKey, TValue> CreateTestee<TKey, TValue>(TKey key, TValue value);
}