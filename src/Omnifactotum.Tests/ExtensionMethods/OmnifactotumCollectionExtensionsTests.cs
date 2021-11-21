#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumCollectionExtensions))]
    internal sealed class OmnifactotumCollectionExtensionsTests
    {
        [Test]
        public void TestDoForEachWhenInvalidArgumentsThenThrows()
        {
            const int[]? NullArray = null;

            Assert.That(() => NullArray!.DoForEach(_ => { }), Throws.ArgumentNullException);
            Assert.That(() => new[] { "a" }.DoForEach(default(Action<string>)!), Throws.ArgumentNullException);
        }

        [Test]
        public void TestDoForEachWhenValidArgumentsThenSucceeds()
        {
            var stringBuilder = new StringBuilder();

            new[] { 'A', '/', 'z' }.DoForEach(c => stringBuilder.Append(c).Append('.'));

            Assert.That(stringBuilder.ToString(), Is.EqualTo("A./.z."));
        }

        [Test]
        public void TestDoForEachWithIndexWhenInvalidArgumentsThenThrows()
        {
            const int[]? NullArray = null;

            Assert.That(() => NullArray!.DoForEach((_, _) => { }), Throws.ArgumentNullException);
            Assert.That(() => new[] { "a" }.DoForEach(default(Action<string, int>)!), Throws.ArgumentNullException);
        }

        [Test]
        public void TestDoForEachWithIndexWhenValidArgumentsThenSucceeds()
        {
            var stringBuilder = new StringBuilder();

            new[] { 'A', '/', 'z' }.DoForEach((c, i) => stringBuilder.Append(c).Append(':').Append(i).Append('.'));

            Assert.That(stringBuilder.ToString(), Is.EqualTo("A:0./:1.z:2."));
        }

        [Test]
        public void TestDoForEachAsyncWhenInvalidArgumentsThenThrows()
        {
            const int[]? NullArray = null;

            Assert.That(() => NullArray!.DoForEachAsync(_ => Task.CompletedTask), Throws.ArgumentNullException);
            Assert.That(() => new[] { "a" }.DoForEachAsync(default(Func<string, Task>)!), Throws.ArgumentNullException);
        }

        [Test]
        public async Task TestDoForEachAsyncWhenValidArgumentsThenSucceedsAsync()
        {
            var stringBuilder = new StringBuilder();

            await new[] { 'a', '/', 'Z' }.DoForEachAsync(
                async c =>
                {
                    await Task.Delay(0);
                    stringBuilder.Append(c).Append('.');
                });

            Assert.That(stringBuilder.ToString(), Is.EqualTo("a./.Z."));
        }

        [Test]
        public void TestDoForEachAsyncWithIndexWhenInvalidArgumentsThenThrows()
        {
            const int[]? NullArray = null;

            Assert.That(() => NullArray!.DoForEachAsync((_, _) => Task.CompletedTask), Throws.ArgumentNullException);
            Assert.That(() => new[] { "a" }.DoForEachAsync(default(Func<string, int, Task>)!), Throws.ArgumentNullException);
        }

        [Test]
        public async Task TestDoForEachAsyncWithIndexWhenValidArgumentsThenSucceedsAsync()
        {
            var stringBuilder = new StringBuilder();

            await new[] { 'a', '/', 'Z' }.DoForEachAsync(
                async (c, i) =>
                {
                    await Task.Delay(0);
                    stringBuilder.Append(c).Append(':').Append(i).Append('.');
                });

            Assert.That(stringBuilder.ToString(), Is.EqualTo("a:0./:1.Z:2."));
        }

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

        [Test]
        public void TestAsReadOnlyNegative()
            => Assert.That(() => ((IList<string>?)null)!.AsReadOnly(), Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void TestAsReadOnly()
        {
            var initialValues = new[] { "foo", "bar" };

            var list = new List<string>(initialValues);
            var readOnly = list.AsReadOnly();

            Assert.That(readOnly, Is.Not.Null);
            Assert.That(readOnly, Is.TypeOf<ReadOnlyCollection<string>>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            var readOnlyAsCollection = (ICollection<string>)readOnly;
            var readOnlyAsList = (IList<string>)readOnly;

            Assert.That(() => readOnlyAsCollection.Clear(), Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            Assert.That(() => readOnlyAsCollection.Add("something"), Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            Assert.That(() => readOnlyAsCollection.Remove("foo"), Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            Assert.That(() => readOnlyAsList[0] = "Foo 2", Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            Assert.That(() => readOnlyAsList.RemoveAt(0), Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(initialValues));

            list[0] = "not foo";
            Assert.That(readOnly, Is.EqualTo(list));

            list.Add("double bar");
            Assert.That(readOnly, Is.EqualTo(list));
        }

        private sealed class ToUIStringForStringCollectionTestCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(null, "<null>").SetDescription("Null collection of strings");

                yield return new TestCaseData(
                        new[] { null, "", "Hello", "Class \"MyClass\"" },
                        @"null, """", ""Hello"", ""Class """"MyClass""""""")
                    .SetDescription("Collection containing various string values");
            }
        }

        private sealed class ToUIStringForNullableCollectionTestCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(null, "<null>").SetDescription("Null collection of nullable integers");

                yield return
                    new TestCaseData(new int?[] { null, 42 }, @"null, 42")
                        .SetDescription("Collection of nullable integers containing various values");
            }
        }
    }
}