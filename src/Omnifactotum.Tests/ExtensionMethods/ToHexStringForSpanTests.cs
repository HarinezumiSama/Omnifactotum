using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumSpanExtensions))]
internal sealed class ToHexStringForSpanTests : ToHexStringTestsBase
{
    protected override string ExecuteToHexString(byte[] bytes, string? separator, bool upperCase)
        => new Span<byte>(bytes).ToHexString(separator, upperCase);

    protected override string ExecuteToHexStringWithDefaultOptionalParameters(byte[] bytes) => new Span<byte>(bytes).ToHexString();
}