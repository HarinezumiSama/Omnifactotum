using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumStringExtensions))]
    internal sealed class OmnifactotumStringExtensionsTests
    {
        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("\u0000", false)]
        [TestCase("\u0020", false)]
        [TestCase("\t", false)]
        [TestCase("\r", false)]
        [TestCase("\n", false)]
        [TestCase("\u0020\t\r\n", false)]
        [TestCase("A", false)]
        [TestCase("\u0020A\u0020", false)]
        public void TestIsNullOrEmpty(string? value, bool expectedResult)
            => Assert.That(value.IsNullOrEmpty, Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("\u0000", false)]
        [TestCase("\u0020", true)]
        [TestCase("\t", true)]
        [TestCase("\r", true)]
        [TestCase("\n", true)]
        [TestCase("\u0020\t\r\n", true)]
        [TestCase("A", false)]
        [TestCase("\u0020A\u0020", false)]
        public void TestIsNullOrWhiteSpace(string? value, bool expectedResult)
            => Assert.That(value.IsNullOrWhiteSpace, Is.EqualTo(expectedResult));

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
        public void TestToNullableBooleanAndToBoolean(string? value, bool? expectedResult)
        {
            Assert.That(value.ToNullableBoolean, Is.EqualTo(expectedResult));

            var constraint = expectedResult.HasValue
                ? (IResolveConstraint)Is.EqualTo(expectedResult.Value)
                : Throws.ArgumentException;

            Assert.That(value!.ToBoolean, constraint);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020")]
        [TestCase(":")]
        [TestCase(",")]
        public void TestJoinWhenInvalidArgumentsThenThrows(string? separator)
        {
            const IEnumerable<string?> NullCollection = null!;

            Assert.That(
                () => NullCollection!.Join(separator),
                Throws.TypeOf<ArgumentNullException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo("values"));
        }

        [Test]
        [TestCase(new[] { "foo", "bar" }, ",", "foo,bar")]
        [TestCase(new[] { "foo", "bar" }, ",\u0020", "foo, bar")]
        [TestCase(new[] { "foo", "bar" }, "\u0020", "foo bar")]
        [TestCase(new[] { "\u0020foo\u0020", "\u0020bar\u0020" }, "\u0020", " foo   bar ")]
        [TestCase(new[] { "foo;", "bar;" }, ";", "foo;;bar;")]
        [TestCase(new[] { "bar" }, ",\u0020", "bar")]
        [TestCase(new string[0], ",\u0020", "")]
        [TestCase(new[] { "foo", "bar" }, null, "foobar")]
        [TestCase(new[] { "foo", "bar" }, "", "foobar")]
        public void TestJoinWhenValidArgumentsThenSucceeds(string?[] values, string? separator, string expectedResult)
            => Assert.That(() => values.Join(separator), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020")]
        [TestCase("foo bar")]
        [TestCase("\u0020foo bar\u0020")]
        public void TestAvoidNull(string? value)
            => Assert.That(value.AvoidNull, value is null ? Is.EqualTo(string.Empty) : Is.SameAs(value));

        [Test]
        [TestCase(null, "null")]
        [TestCase("", @"""""")]
        [TestCase(@" A ""B"" 'C'", @""" A """"B"""" 'C'""")]
        public void TestToUIString(string? value, string expectedResult) => Assert.That(value.ToUIString, Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null, null, "")]
        [TestCase(null, new[] { '#' }, "")]
        [TestCase("", null, "")]
        [TestCase("", new[] { '#' }, "")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "A \t\r\nB \t\r\nC")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "A \t\r\nB \t\r\nC")]
        [TestCase("#A#B#C#", new[] { '#' }, "A#B#C")]
        [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"A/\/B\/\C")]
        public void TestTrimSafely(string? value, char[]? trimChars, string expectedResult)
            => Assert.That(() => value.TrimSafely(trimChars), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null, null, "")]
        [TestCase(null, new[] { '#' }, "")]
        [TestCase("", null, "")]
        [TestCase("", new[] { '#' }, "")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "A \t\r\nB \t\r\nC\u0020\t\r\n")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "A \t\r\nB \t\r\nC\u0020\t\r\n")]
        [TestCase("#A#B#C#", new[] { '#' }, "A#B#C#")]
        [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"A/\/B\/\C/\/")]
        public void TestTrimStartSafely(string? value, char[]? trimChars, string expectedResult)
            => Assert.That(() => value.TrimStartSafely(trimChars), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null, null, "")]
        [TestCase(null, new[] { '#' }, "")]
        [TestCase("", null, "")]
        [TestCase("", new[] { '#' }, "")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "\u0020\t\r\nA \t\r\nB \t\r\nC")]
        [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "\u0020\t\r\nA \t\r\nB \t\r\nC")]
        [TestCase("#A#B#C#", new[] { '#' }, "#A#B#C")]
        [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"\/\A/\/B\/\C")]
        public void TestTrimEndSafely(string value, char[]? trimChars, string expectedResult)
            => Assert.That(() => value.TrimEndSafely(trimChars), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void TestShortenWhenInvalidArgumentsThenThrows(int maximumLength)
            => Assert.That(() => "foo".Shorten(maximumLength), Throws.TypeOf<ArgumentOutOfRangeException>());

        [Test]
        [TestCase(null, 0, "")]
        [TestCase(null, int.MaxValue, "")]
        [TestCase("", 0, "")]
        [TestCase("", int.MaxValue, "")]
        [TestCase("\u0020A B C\u0020", 0, "")]
        [TestCase("\u0020A B C\u0020", 3, "\u0020A\u0020")]
        public void TestShortenWhenValidArgumentsThenSucceeds(string value, int maximumLength, string expectedResult)
            => Assert.That(() => value.Shorten(maximumLength), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void TestReplicateWhenInvalidArgumentsThenThrows(int count)
            => Assert.That(() => "ABC".Replicate(count), Throws.TypeOf<ArgumentOutOfRangeException>());

        [Test]
        [TestCase(null, 0, "")]
        [TestCase(null, int.MaxValue, "")]
        [TestCase("", 0, "")]
        [TestCase("", int.MaxValue, "")]
        [TestCase("\u0020A\u0020B\u0020C\u0020", 0, "")]
        [TestCase("\u0020A\u0020B\u0020C\u0020", 3, "\u0020A\u0020B\u0020C\u0020\u0020A\u0020B\u0020C\u0020\u0020A\u0020B\u0020C\u0020")]
        public void TestReplicateWhenValidArgumentsThenSucceeds(string value, int count, string expectedResult)
            => Assert.That(() => value.Replicate(count), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase("\u0020\t\r\n", false)]
        [TestCase("/a/b/c", false)]
        [TestCase("ftp://a/b/c", false)]
        [TestCase("news://a/b/c", false)]
        [TestCase("nntp://a/b/c", false)]
        [TestCase("gopher://a/b/c", false)]
        [TestCase("mailto:john@example.com", false)]
        [TestCase("mailto://john@example.com", false)]
        [TestCase("file://a/b/c", false)]
        [TestCase("net.pipe://a/b/c", false)]
        [TestCase("net.tcp://a/b/c", false)]
        [TestCase("http://a/b/c", true)]
        [TestCase("https://a/b/c", true)]
        public void TestIsWebUriSucceeds(string value, bool expectedResult) => Assert.That(value.IsWebUri, Is.EqualTo(expectedResult));

        [Test]
        public void TestWithSingleTrailingSlashWhenInvalidArgumentThenThrows()
            => Assert.That(() => default(string)!.WithSingleTrailingSlash(), Throws.ArgumentNullException);

        [Test]
        [TestCase(@"", @"/", false)]
        [TestCase(@"foo", @"foo/", false)]
        [TestCase(@"foo/", @"foo/", true)]
        [TestCase(@"foo//////////", @"foo/", false)]
        [TestCase(@"foo/bar", @"foo/bar/", false)]
        [TestCase(@"foo/bar/", @"foo/bar/", true)]
        [TestCase(@"foo/bar//////////", @"foo/bar/", false)]
        [TestCase(@"mailto://example.com", @"mailto://example.com/", false)]
        [TestCase(@"http://example.com", @"http://example.com/", false)]
        [TestCase(@"http://example.com/", @"http://example.com/", true)]
        [TestCase(@"http://example.com//////////", @"http://example.com/", false)]
        [TestCase(@"http://example.com/api/v1", @"http://example.com/api/v1/", false)]
        [TestCase(@"http://example.com/api/v1/", @"http://example.com/api/v1/", true)]
        [TestCase(@"http://example.com/api/v1//////////", @"http://example.com/api/v1/", false)]
        public void TestWithSingleTrailingSlashWhenValidArgumentThenSucceeds(
            string value,
            string expectedResult,
            bool isSameObjectAsInput)
        {
            var actualResult = value.WithSingleTrailingSlash();
            Assert.That(actualResult, Is.EqualTo(expectedResult));

            Constraint referenceConstraint = Is.SameAs(value);
            if (!isSameObjectAsInput)
            {
                referenceConstraint = !referenceConstraint;
            }

            Assert.That(actualResult, referenceConstraint);
        }

        [Test]
        public void TestWithoutTrailingSlashWhenInvalidArgumentThenThrows()
            => Assert.That(() => default(string)!.WithoutTrailingSlash(), Throws.ArgumentNullException);

        [Test]
        [TestCase(@"", @"", true)]
        [TestCase(@"foo", @"foo", true)]
        [TestCase(@"foo/", @"foo", false)]
        [TestCase(@"foo//////////", @"foo", false)]
        [TestCase(@"foo/bar", @"foo/bar", true)]
        [TestCase(@"foo/bar/", @"foo/bar", false)]
        [TestCase(@"foo/bar//////////", @"foo/bar", false)]
        [TestCase(@"mailto://example.com", @"mailto://example.com", true)]
        [TestCase(@"http://example.com", @"http://example.com", true)]
        [TestCase(@"http://example.com/", @"http://example.com", false)]
        [TestCase(@"http://example.com//////////", @"http://example.com", false)]
        [TestCase(@"http://example.com/api/v1", @"http://example.com/api/v1", true)]
        [TestCase(@"http://example.com/api/v1/", @"http://example.com/api/v1", false)]
        [TestCase(@"http://example.com/api/v1//////////", @"http://example.com/api/v1", false)]
        public void TestWithoutTrailingSlashWhenValidArgumentThenSucceeds(
            string value,
            string expectedResult,
            bool isSameObjectAsInput)
        {
            var actualResult = value.WithoutTrailingSlash();
            Assert.That(actualResult, Is.EqualTo(expectedResult));

            Constraint referenceConstraint = Is.SameAs(value);
            if (!isSameObjectAsInput)
            {
                referenceConstraint = !referenceConstraint;
            }

            Assert.That(actualResult, referenceConstraint);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020")]
        [TestCase("\t")]
        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase(@"breaKFAst/завтрак/朝ごはん/petiT-DÉjeuner/早餐/")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestToSecureString(string? plainText)
        {
            using var secureString = plainText.ToSecureString();

            if (plainText is null)
            {
                Assert.That(secureString, Is.Null);
                return;
            }

            var actualResult = new NetworkCredential(string.Empty, secureString).Password;
            Assert.That(actualResult, Is.EqualTo(plainText));
        }
    }
}