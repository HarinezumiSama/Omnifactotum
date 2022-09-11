using System.Net;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumHttpStatusCodeExtensions))]
    internal sealed class OmnifactotumHttpStatusCodeExtensionsTests
    {
        [Test]
        [TestCase((HttpStatusCode)0, "0")]
        [TestCase((HttpStatusCode)100, "100 Continue")]
        [TestCase((HttpStatusCode)101, "101 SwitchingProtocols")]
        [TestCase((HttpStatusCode)200, "200 OK")]
        [TestCase((HttpStatusCode)204, "204 NoContent")]
        [TestCase((HttpStatusCode)304, "304 NotModified")]
        [TestCase((HttpStatusCode)305, "305 UseProxy")]
        [TestCase((HttpStatusCode)401, "401 Unauthorized")]
        [TestCase((HttpStatusCode)404, "404 NotFound")]
        [TestCase((HttpStatusCode)418, "418 IAmATeapot")]
        [TestCase((HttpStatusCode)422, "422 UnprocessableEntity")]
        [TestCase((HttpStatusCode)425, "425 TooEarly")]
        [TestCase((HttpStatusCode)429, "429 TooManyRequests")]
        [TestCase((HttpStatusCode)451, "451 UnavailableForLegalReasons")]
        [TestCase((HttpStatusCode)500, "500 InternalServerError")]
        [TestCase((HttpStatusCode)503, "503 ServiceUnavailable")]
        public void TestToUIString(HttpStatusCode input, string expectedResult)
            => Assert.That(() => input.ToUIString(), Is.EqualTo(expectedResult));
    }
}