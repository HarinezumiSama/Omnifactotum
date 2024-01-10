using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(EnumFixedSizeDictionary<,>))]
internal sealed class EnumFixedSizeDictionaryTests
{
    [Test]
    public void TestBasicScenario([Values] FileMode fileMode)
    {
        //// ReSharper disable once StringLiteralTypo :: Test value
        const string Value = @"V.alue";

        var dictionary = new EnumFixedSizeDictionary<FileMode, string>();
        Assert.That(() => dictionary.Count, Is.EqualTo(0));

        dictionary.Add(fileMode, Value);
        Assert.That(() => dictionary.Count, Is.EqualTo(1));
        Assert.That(() => dictionary.ContainsKey(fileMode), Is.True);
        Assert.That(() => dictionary.Keys.ToArray(), Is.EquivalentTo(fileMode.AsArray()));
        Assert.That(() => dictionary.Values.ToArray(), Is.EquivalentTo(Value.AsArray()));
    }
}