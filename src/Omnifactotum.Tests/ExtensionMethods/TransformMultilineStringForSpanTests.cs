using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
internal sealed class TransformMultilineStringForSpanTests : TransformMultilineStringTestsBase
{
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    protected override string ExecuteTransformMultilineString(string input, Func<string, int, string> transformLine, bool normalizeLineEndings)
    {
        Span<char> span = input.ToCharArray().AsSpan();
        return span.TransformMultilineString(transformLine, normalizeLineEndings, CancellationToken.None);
    }

    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    protected override string ExecuteTransformMultilineStringWithDefaultOptionalParameters(string input, Func<string, int, string> transformLine)
    {
        Span<char> span = input.ToCharArray().AsSpan();
        return span.TransformMultilineString(transformLine);
    }
}