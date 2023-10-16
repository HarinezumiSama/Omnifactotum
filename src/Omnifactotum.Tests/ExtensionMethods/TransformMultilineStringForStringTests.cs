using System;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
internal sealed class TransformMultilineStringForStringTests : TransformMultilineStringTestsBase
{
    protected override void ExecuteTestTransformMultilineStringWhenInvalidArgumentThenThrows(bool normalizeLineEndings)
    {
        base.ExecuteTestTransformMultilineStringWhenInvalidArgumentThenThrows(normalizeLineEndings);

        Assert.That(
            () => ExecuteTransformMultilineString(default!, null!, normalizeLineEndings),
            Throws.ArgumentNullException);

        Assert.That(
            () => ExecuteTransformMultilineString(default!, (_, _) => throw new NotImplementedException(), normalizeLineEndings),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("value"));
    }

    protected override string ExecuteTransformMultilineString(string input, Func<string, int, string> transformLine, bool normalizeLineEndings)
        => input.TransformMultilineString(transformLine, normalizeLineEndings, CancellationToken.None);

    protected override string ExecuteTransformMultilineStringWithDefaultOptionalParameters(string input, Func<string, int, string> transformLine)
        => input.TransformMultilineString(transformLine);
}