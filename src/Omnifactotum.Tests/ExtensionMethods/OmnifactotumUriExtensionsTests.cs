#nullable enable

using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumUriExtensions))]
    internal sealed class OmnifactotumUriExtensionsTests
    {
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
        public void TestIsWebUriSucceeds(string? uriString, bool expectedResult)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.IsWebUri(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("http://a/b/c")]
        [TestCase("https://a/b/c")]
        public void TestEnsureWebUriWhenWebUriThenSucceeds(string? uriString)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.EnsureWebUri(), Is.SameAs(uri));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020\t\r\n")]
        [TestCase("/a/b/c")]
        [TestCase("ftp://a/b/c")]
        [TestCase("news://a/b/c")]
        [TestCase("nntp://a/b/c")]
        [TestCase("gopher://a/b/c")]
        [TestCase("mailto:john@example.com")]
        [TestCase("mailto://john@example.com")]
        [TestCase("file://a/b/c")]
        [TestCase("net.pipe://a/b/c")]
        [TestCase("net.tcp://a/b/c")]
        public void TestEnsureWebUriWhenNotWebUriThenThrows(string? uriString)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.EnsureWebUri(), Throws.ArgumentException);
        }

        [Test]
        [TestCase("http://a/b/c")]
        [TestCase("https://a/b/c")]
        [TestCase("ftp://a/b/c")]
        [TestCase("news://a/b/c")]
        [TestCase("nntp://a/b/c")]
        [TestCase("gopher://a/b/c")]
        [TestCase("mailto:john@example.com")]
        [TestCase("mailto://john@example.com")]
        [TestCase("file://a/b/c")]
        [TestCase("net.pipe://a/b/c")]
        [TestCase("net.tcp://a/b/c")]
        public void TestEnsureAbsoluteUriWhenWebUriThenSucceeds(string? uriString)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.EnsureAbsoluteUri(), Is.SameAs(uri));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020\t\r\n")]
        [TestCase("/a/b/c")]
        public void TestEnsureAbsoluteUriWhenNotWebUriThenThrows(string? uriString)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.EnsureAbsoluteUri(), Throws.ArgumentException);
        }

        [Test]
        [TestCase(null, "null")]
        [TestCase("", "\"\"")]
        [TestCase("\u0020\t\r\n", "\"\u0020\t\r\n\"")]
        [TestCase("foo", "\"foo\"")]
        [TestCase("foo\"b\"ar", "\"foo\"\"b\"\"ar\"")]
        [TestCase("http://example.com/path", "\"http://example.com/path\"")]
        [TestCase("C:\\dir\\file.txt", "\"file:///C:/dir/file.txt\"")]
        [TestCase("mailto:user@example.com", "\"mailto:user@example.com\"")]
        public void TestToUIString(string? uriString, string expectedResult)
        {
            var uri = uriString is null ? null : new Uri(uriString, UriKind.RelativeOrAbsolute);
            Assert.That(() => uri.ToUIString(), Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestWithSingleTrailingSlashWhenInvalidArgumentThenThrows()
            => Assert.That(() => default(Uri)!.WithSingleTrailingSlash().ToString(), Throws.ArgumentNullException);

        [Test]
        [TestCase(@"", @"/", false)]
        [TestCase(@"foo", @"foo/", false)]
        [TestCase(@"foo/", @"foo/", true)]
        [TestCase(@"foo//////////", @"foo/", false)]
        [TestCase(@"foo/bar", @"foo/bar/", false)]
        [TestCase(@"foo/bar/", @"foo/bar/", true)]
        [TestCase(@"foo/bar//////////", @"foo/bar/", false)]
        [TestCase(@"mailto://example.com", @"mailto://example.com/", false)]
        [TestCase(@"http://example.com", @"http://example.com/", true)]
        [TestCase(@"http://example.com/", @"http://example.com/", true)]
        [TestCase(@"http://example.com//////////", @"http://example.com/", false)]
        [TestCase(@"http://example.com/api/v1", @"http://example.com/api/v1/", false)]
        [TestCase(@"http://example.com/api/v1/", @"http://example.com/api/v1/", true)]
        [TestCase(@"http://example.com/api/v1//////////", @"http://example.com/api/v1/", false)]
        public void TestWithSingleTrailingSlashWhenValidArgumentThenSucceeds(
            string uriString,
            string expectedResultUriString,
            bool isSameObjectAsInput)
        {
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            var resultUri = uri.WithSingleTrailingSlash();
            Assert.That(resultUri.ToString(), Is.EqualTo(expectedResultUriString));
            Assert.That(resultUri.IsAbsoluteUri, Is.EqualTo(uri.IsAbsoluteUri));

            Constraint referenceConstraint = Is.SameAs(uri);
            if (!isSameObjectAsInput)
            {
                referenceConstraint = !referenceConstraint;
            }

            Assert.That(resultUri, referenceConstraint);
        }

        [Test]
        public void TestWithoutTrailingSlashWhenInvalidArgumentThenThrows()
            => Assert.That(() => default(Uri)!.WithoutTrailingSlash().ToString(), Throws.ArgumentNullException);

        [Test]
        [TestCase(@"", @"", true)]
        [TestCase(@"foo", @"foo", true)]
        [TestCase(@"foo/", @"foo", false)]
        [TestCase(@"foo//////////", @"foo", false)]
        [TestCase(@"foo/bar", @"foo/bar", true)]
        [TestCase(@"foo/bar/", @"foo/bar", false)]
        [TestCase(@"foo/bar//////////", @"foo/bar", false)]
        [TestCase(@"mailto://example.com", @"mailto://example.com", true)]
        [TestCase(@"http://example.com", @"http://example.com/", false)]
        [TestCase(@"http://example.com/", @"http://example.com/", false)]
        [TestCase(@"http://example.com//////////", @"http://example.com/", false)]
        [TestCase(@"http://example.com/api/v1", @"http://example.com/api/v1", true)]
        [TestCase(@"http://example.com/api/v1/", @"http://example.com/api/v1", false)]
        [TestCase(@"http://example.com/api/v1//////////", @"http://example.com/api/v1", false)]
        public void TestWithoutTrailingSlashWhenValidArgumentThenSucceeds(
            string uriString,
            string expectedResultUriString,
            bool isSameObjectAsInput)
        {
            var uri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            var resultUri = uri.WithoutTrailingSlash();
            Assert.That(resultUri.ToString(), Is.EqualTo(expectedResultUriString));
            Assert.That(resultUri.IsAbsoluteUri, Is.EqualTo(uri.IsAbsoluteUri));

            Constraint referenceConstraint = Is.SameAs(uri);
            if (!isSameObjectAsInput)
            {
                referenceConstraint = !referenceConstraint;
            }

            Assert.That(resultUri, referenceConstraint);
        }
    }
}