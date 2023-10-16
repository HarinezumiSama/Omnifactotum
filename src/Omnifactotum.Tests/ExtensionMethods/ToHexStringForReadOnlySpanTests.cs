using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumReadOnlySpanExtensions))]
internal sealed class ToHexStringForReadOnlySpanTests : ToHexStringTestsBase
{
    protected override string ExecuteToHexString(byte[] bytes, string? separator, bool upperCase)
        => new ReadOnlySpan<byte>(bytes).ToHexString(separator, upperCase);

    protected override string ExecuteToHexStringWithDefaultOptionalParameters(byte[] bytes) => new ReadOnlySpan<byte>(bytes).ToHexString();
}