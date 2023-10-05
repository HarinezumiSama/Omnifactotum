using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumReadOnlySpanExtensions))]
internal sealed class OmnifactotumReadOnlySpanExtensionsToHexStringTests : OmnifactotumCommonArrayAndSpanExtensionsTestsBase
{
    protected override string ExecuteTestee(byte[] bytes, string? separator, bool upperCase)
        => new ReadOnlySpan<byte>(bytes).ToHexString(separator, upperCase);

    protected override string ExecuteTesteeWithDefaultOptionalParameters(byte[] bytes) => new ReadOnlySpan<byte>(bytes).ToHexString();
}