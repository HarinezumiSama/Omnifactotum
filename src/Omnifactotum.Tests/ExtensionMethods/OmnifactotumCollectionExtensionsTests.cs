using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    internal sealed class OmnifactotumCollectionExtensionsTests
    {
        [Test]
        [TestCaseSource(typeof(ToUIStringForStringCollectionTestCases))]
        public void TestToUIStringForStringCollection(string[] values, string expectedResult)
        {
            var actualResult = values.ToUIString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCaseSource(typeof(ToUIStringForNullableCollectionTestCases))]
        public void TestToUIStringForNullableCollection(int?[] values, string expectedResult)
        {
            var actualResult = values.ToUIString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        private sealed class ToUIStringForStringCollectionTestCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(null, "<null>").SetName("Null collection of strings");

                yield return new TestCaseData(
                    new[] { null, "", "Hello", "Class \"MyClass\"" },
                    @"null, """", ""Hello"", ""Class """"MyClass""""""")
                    .SetName("Collection containing various string values");
            }
        }

        private sealed class ToUIStringForNullableCollectionTestCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(null, "<null>").SetName("Null collection of nullable integers");

                yield return
                    new TestCaseData(new int?[] { null, 42 }, @"null, 42")
                        .SetName("Collection of nullable integers containing various values");
            }
        }
    }
}