using System.Net;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumHttpStatusCodeExtensions))]
internal sealed class OmnifactotumHttpStatusCodeExtensionsTests
{
    [Test]
    [TestCase((HttpStatusCode)(-12345), "-12345")]
    [TestCase((HttpStatusCode)(-1), "-1")]
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
    [TestCase((HttpStatusCode)int.MaxValue, "2147483647")]
    public void TestToUIString(HttpStatusCode input, string expectedResult)
        => Assert.That(() => input.ToUIString(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase((HttpStatusCode)int.MinValue, false)]
    [TestCase((HttpStatusCode)0, false)]
    [TestCase((HttpStatusCode)199, false)]
    [TestCase(HttpStatusCode.OK, true)]
    [TestCase(HttpStatusCode.Created, true)]
    [TestCase(HttpStatusCode.Accepted, true)]
    [TestCase(HttpStatusCode.NoContent, true)]
    [TestCase(HttpStatusCode.ResetContent, true)]
    [TestCase(HttpStatusCode.PartialContent, true)]
    [TestCase((HttpStatusCode)299, true)]
    [TestCase(HttpStatusCode.Ambiguous, false)]
    [TestCase(HttpStatusCode.MovedPermanently, false)]
    [TestCase(HttpStatusCode.Found, false)]
    [TestCase(HttpStatusCode.Redirect, false)]
    [TestCase(HttpStatusCode.BadRequest, false)]
    [TestCase(HttpStatusCode.Unauthorized, false)]
    [TestCase(HttpStatusCode.Forbidden, false)]
    [TestCase(HttpStatusCode.NotFound, false)]
    [TestCase(HttpStatusCode.MethodNotAllowed, false)]
    [TestCase(HttpStatusCode.Conflict, false)]
    [TestCase(HttpStatusCode.Gone, false)]
    [TestCase(HttpStatusCode.UnprocessableEntity, false)]
    [TestCase(HttpStatusCode.TooManyRequests, false)]
    [TestCase(HttpStatusCode.InternalServerError, false)]
    [TestCase(HttpStatusCode.NotImplemented, false)]
    [TestCase(HttpStatusCode.BadGateway, false)]
    [TestCase(HttpStatusCode.ServiceUnavailable, false)]
    [TestCase(HttpStatusCode.GatewayTimeout, false)]
    [TestCase((HttpStatusCode)999, false)]
    [TestCase((HttpStatusCode)int.MaxValue, false)]
    public void TestIsSuccessful(HttpStatusCode input, bool expectedResult)
        => Assert.That(() => input.IsSuccessful(), Is.EqualTo(expectedResult));
}