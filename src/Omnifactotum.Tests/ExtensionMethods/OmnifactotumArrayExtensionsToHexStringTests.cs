using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumArrayExtensions))]
internal sealed class OmnifactotumArrayExtensionsToHexStringTests : OmnifactotumCommonArrayAndReadOnlySpanExtensionsTestsBase
{
    [Test]
    public void TestToHexStringWhenInvalidArgumentThenThrows([Values(null, "", ":", "\t|\t")] string? separator, [Values] bool upperCase)
        => Assert.That(() => ExecuteTestee(null!, separator, upperCase), Throws.TypeOf<ArgumentNullException>());

    protected override string ExecuteTestee(byte[] bytes, string? separator, bool upperCase)
        => bytes.ToHexString(separator, upperCase);

    protected override string ExecuteTesteeWithDefaultOptionalParameters(byte[] bytes) => bytes.ToHexString();
}