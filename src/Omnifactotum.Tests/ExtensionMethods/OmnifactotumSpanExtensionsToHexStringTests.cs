using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumSpanExtensions))]
internal sealed class OmnifactotumSpanExtensionsToHexStringTests : OmnifactotumCommonArrayAndSpanExtensionsTestsBase
{
    protected override string ExecuteTestee(byte[] bytes, string? separator, bool upperCase)
        => new Span<byte>(bytes).ToHexString(separator, upperCase);

    protected override string ExecuteTesteeWithDefaultOptionalParameters(byte[] bytes) => new Span<byte>(bytes).ToHexString();
}