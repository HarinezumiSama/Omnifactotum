using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class EnumFixedSizeDictionaryTests
    {
        #region Tests

        [Test]
        [TestCaseSource(typeof(BasicScenarioCases))]
        public void TestBasicScenario(FileMode fileMode)
        {
            const string Value = "V.alue";

            var dictionary = new EnumFixedSizeDictionary<FileMode, string>();
            Assert.That(dictionary.Count, Is.EqualTo(0));

            dictionary.Add(fileMode, Value);
            Assert.That(dictionary.Count, Is.EqualTo(1));
            Assert.That(dictionary.ContainsKey(fileMode), Is.True);
            Assert.That(dictionary.Keys.ToArray(), Is.EquivalentTo(fileMode.AsArray()));
            Assert.That(dictionary.Values.ToArray(), Is.EquivalentTo(Value.AsArray()));
        }

        #endregion

        #region BasicScenarioCases Class

        public sealed class BasicScenarioCases : TestCasesBase
        {
            #region Protected Methods

            protected override IEnumerable<TestCaseData> GetCases()
            {
                return EnumFactotum.GetAllValues<FileMode>().Select(item => new TestCaseData(item));
            }

            #endregion
        }

        #endregion
    }
}