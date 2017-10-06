using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests.ExtensionMethods
{
    //// ReSharper disable AssignNullToNotNullAttribute - Intentionally for tests

    [TestFixture]
    public sealed class OmnifactotumStringExtensionsTests
    {
        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase(" ", false)]
        [TestCase(" A ", false)]
        public void TestIsNullOrEmpty(string value, bool expectedResult)
        {
            var actualResult = value.IsNullOrEmpty();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase(" A ", false)]
        public void TestIsNullOrWhiteSpace(string value, bool expectedResult)
        {
            var actualResult = value.IsNullOrWhiteSpace();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase(" ", null)]
        [TestCase("\n", null)]
        [TestCase("T", null)]
        [TestCase("1", true)]
        [TestCase("42", true)]
        [TestCase(" 1 \n ", true)]
        [TestCase("true", true)]
        [TestCase("  tRue ", true)]
        [TestCase("0", false)]
        [TestCase(" \n 0 \r ", false)]
        [TestCase("false", false)]
        [TestCase(" FALse ", false)]
        public void TestToNullableBooleanAndToBoolean(string value, bool? expectedResult)
        {
            var actualResult = value.ToNullableBoolean();
            Assert.That(actualResult, Is.EqualTo(expectedResult));

            var constraint = expectedResult.HasValue
                ? (IResolveConstraint)Is.EqualTo(expectedResult.Value)
                : Throws.ArgumentException;

            Assert.That(value.ToBoolean, constraint);
        }

        [Test]
        public void TestJoinNegative()
        {
            const IEnumerable<string> NullCollection = null;
            Assert.That(() => NullCollection.Join(","), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestJoinOfMultiItemCollection()
        {
            var strings = new[] { "foo", "bar" };
            var joined = strings.Join(", ");
            Assert.That(joined, Is.EqualTo("foo, bar"));
        }

        [Test]
        public void TestJoinOfSingleItemCollection()
        {
            var strings = new[] { "bar" };
            var joined = strings.Join(", ");
            Assert.That(joined, Is.EqualTo(strings.Single()));
        }

        [Test]
        public void TestJoinOfEmptyCollection()
        {
            var strings = new string[0];
            var joined = strings.Join(", ");
            Assert.That(joined, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestJoinUsingNullSeparator()
        {
            var strings = new[] { "foo", "bar" };
            var joined = strings.Join(null);
            Assert.That(joined, Is.EqualTo("foobar"));
        }

        [Test]
        public void TestJoinUsingEmptySeparator()
        {
            var strings = new[] { "foo", "bar" };
            var joined = strings.Join(string.Empty);
            Assert.That(joined, Is.EqualTo("foobar"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("A")]
        public void TestAvoidNull(string value)
        {
            var actualResult = value.AvoidNull();

            if (value == null)
            {
                Assert.That(actualResult, Is.EqualTo(string.Empty));
            }
            else
            {
                Assert.That(actualResult, Is.SameAs(value));
            }
        }

        [Test]
        [TestCase(null, "null")]
        [TestCase("", "\"\"")]
        [TestCase(" A \"B\" C", "\" A \"\"B\"\" C\"")]
        public void TestToUIString(string value, string expectedResult)
        {
            var actualResult = value.ToUIString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("#A B C#", "A B C")]
        public void TestTrimSafely(string value, string expectedResult)
        {
            var actualResult = value.TrimSafely('#');
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("#A B C#", "A B C#")]
        public void TestTrimStartSafely(string value, string expectedResult)
        {
            var actualResult = value.TrimStartSafely('#');
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase("#A B C#", "#A B C")]
        public void TestTrimEndSafely(string value, string expectedResult)
        {
            var actualResult = value.TrimEndSafely('#');
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void TestShortenNegative(int maximumLength)
        {
            Assert.That(() => "ABC".Shorten(maximumLength), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(null, 0, "")]
        [TestCase(null, int.MaxValue, "")]
        [TestCase("", 0, "")]
        [TestCase("", int.MaxValue, "")]
        [TestCase(" A B C ", 0, "")]
        [TestCase(" A B C ", 3, " A ")]
        public void TestShorten(string value, int maximumLength, string expectedResult)
        {
            var actualResult = value.Shorten(maximumLength);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void TestReplicateNegative(int count)
        {
            Assert.That(() => "ABC".Replicate(count), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(null, 0, "")]
        [TestCase(null, int.MaxValue, "")]
        [TestCase("", 0, "")]
        [TestCase("", int.MaxValue, "")]
        [TestCase(" A B C ", 0, "")]
        [TestCase(" A B C ", 3, " A B C  A B C  A B C ")]
        public void TestReplicate(string value, int count, string expectedResult)
        {
            var actualResult = value.Replicate(count);
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}