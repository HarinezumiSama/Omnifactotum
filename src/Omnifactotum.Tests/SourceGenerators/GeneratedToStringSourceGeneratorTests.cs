using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Omnifactotum.SourceGenerators;

namespace Omnifactotum.Tests.SourceGenerators;

[TestFixture(TestOf = typeof(GeneratedToStringSourceGenerator))]
public sealed class GeneratedToStringSourceGeneratorTests
{
    private const string TestClassText = "";

    private const string ExpectedGeneratedClassText = "";

    [Test]
    public void Test()
    {
        //// SourceGeneratorVerifier<ToStringSourceGenerator,,>

        var generator = new GeneratedToStringSourceGenerator();

        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = CSharpCompilation.Create(
            nameof(GeneratedToStringSourceGeneratorTests),
            new[] { CSharpSyntaxTree.ParseText(TestClassText) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        var runResult = driver.RunGenerators(compilation).GetRunResult();

        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("TestClass.g.cs", StringComparison.Ordinal));
        var generatedFileSourceText = generatedFileSyntax.GetText();

        Assert.That(
            generatedFileSourceText.Lines.Select(line => generatedFileSourceText.ToString(line.Span)),
            Is.EqualTo(ExpectedGeneratedClassText.Split(Environment.NewLine)));
    }
}