using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
internal sealed class TransformMultilineStringForReadOnlySpanTests : TransformMultilineStringTestsBase
{
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    protected override string ExecuteTransformMultilineString(string input, Func<string, int, string> transformLine, bool normalizeLineEndings)
    {
        ReadOnlySpan<char> span = input.AsSpan();
        return span.TransformMultilineString(transformLine, normalizeLineEndings, CancellationToken.None);
    }

    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    protected override string ExecuteTransformMultilineStringWithDefaultOptionalParameters(string input, Func<string, int, string> transformLine)
    {
        ReadOnlySpan<char> span = input.AsSpan();
        return span.TransformMultilineString(transformLine);
    }
}