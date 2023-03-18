using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Omnifactotum.NUnit;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumAssemblyExtensions))]
internal sealed class OmnifactotumAssemblyExtensionsTests
{
    [Test]
    public void TestGetLocalPathWhenLocalAssemblyIsPassedThenSucceeds()
    {
        var assembly = GetType().Assembly;
        var expectedPath = Path.GetFullPath(assembly.Location);
        Assert.That(() => assembly.GetLocalPath(), Is.EqualTo(expectedPath));
    }

    [Test]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void TestGetLocalPathWhenNullAssemblyIsPassedThenThrows()
        => Assert.That(() => ((Assembly?)null)!.GetLocalPath(), Throws.TypeOf<ArgumentNullException>());

    [Test]
    [TestCase(OutputKind.DynamicallyLinkedLibrary)]
    [TestCase(OutputKind.ConsoleApplication)]
    public void TestGetLocalPathWhenInMemoryAssemblyIsPassedThenThrows(
        OutputKind outputKind
    )
    {
        const string SourceCode = @"static class Program { static void Main() { } }";

        var syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);

        var compilation = CSharpCompilation
            .Create(
                AsInvariant($@"{nameof(TestGetLocalPathWhenInMemoryAssemblyIsPassedThenThrows)}_{Guid.NewGuid():N}"),
                options: new CSharpCompilationOptions(outputKind, platform: Platform.AnyCpu))
            .AssertNotNull()
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AssertNotNull()
            .AddSyntaxTrees(syntaxTree)
            .AssertNotNull();

        using var stream = new MemoryStream();

        var emitResult = compilation.Emit(stream);
        Assert.That(emitResult, Is.Not.Null);

        Assert.That(
            emitResult.Success,
            Is.True,
            () => AsInvariant(
                $@"Failed to compile an in-memory assembly:{Environment.NewLine}{
                    emitResult.Diagnostics.Select(d => AsInvariant($@"* {d.GetMessage()}")).Join(Environment.NewLine)}"));

        stream.Position = 0;
        var assembly = AssemblyLoadContext.Default.LoadFromStream(stream).AssertNotNull();

        Assert.That(() => assembly.GetLocalPath(), Throws.TypeOf<ArgumentException>());
    }

    [Test]
    [TestCase(AssemblyBuilderAccess.Run)]
    [TestCase(AssemblyBuilderAccess.RunAndCollect)]
    public void TestGetLocalPathWhenDynamicAssemblyIsPassedThenThrows(AssemblyBuilderAccess assemblyBuilderAccess)
    {
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            new AssemblyName(AsInvariant($@"{nameof(TestGetLocalPathWhenDynamicAssemblyIsPassedThenThrows)}_{Guid.NewGuid():N}")),
            assemblyBuilderAccess);

        Assert.That(() => assemblyBuilder.GetLocalPath(), Throws.TypeOf<ArgumentException>());
    }
}