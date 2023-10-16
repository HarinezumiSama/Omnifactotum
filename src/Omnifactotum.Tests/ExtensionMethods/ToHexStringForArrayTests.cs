using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumArrayExtensions))]
internal sealed class ToHexStringForArrayTests : ToHexStringTestsBase
{
    [Test]
    public void TestToHexStringWhenInvalidArgumentThenThrows([Values(null, "", ":", "\t|\t")] string? separator, [Values] bool upperCase)
        => Assert.That(() => ExecuteToHexString(null!, separator, upperCase), Throws.TypeOf<ArgumentNullException>());

    protected override string ExecuteToHexString(byte[] bytes, string? separator, bool upperCase)
        => bytes.ToHexString(separator, upperCase);

    protected override string ExecuteToHexStringWithDefaultOptionalParameters(byte[] bytes) => bytes.ToHexString();
}