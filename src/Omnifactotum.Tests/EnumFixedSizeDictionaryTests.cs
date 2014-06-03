using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class EnumFixedSizeDictionaryTests
    {
        #region Tests

        [Test]
        public void TestBasicScenario()
        {
            const string OpenValue = "O.p.e.n.";

            var dictionary = new EnumFixedSizeDictionary<FileMode, string>();
            Assert.That(dictionary.Count, Is.EqualTo(0));

            dictionary.Add(FileMode.Open, OpenValue);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.ContainsKey(FileMode.Open), Is.True);
            Assert.That(dictionary.Keys.ToArray(), Is.EquivalentTo(FileMode.Open.AsArray()));
            Assert.That(dictionary.Values.ToArray(), Is.EquivalentTo(OpenValue.AsArray()));
        }

        #endregion
    }
}