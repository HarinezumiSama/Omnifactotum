#if NETFRAMEWORK
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;
using NUnit.Framework;
using Omnifactotum.NUnit;

#else

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

#endif

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
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
            => Assert.That(() => ((Assembly)null).GetLocalPath(), Throws.TypeOf<ArgumentNullException>());

        [Test]
#if NETFRAMEWORK
        [TestCase(false)]
        [TestCase(true)]
#else
        [TestCase(OutputKind.DynamicallyLinkedLibrary)]
        [TestCase(OutputKind.ConsoleApplication)]
#endif
        public void TestGetLocalPathWhenInMemoryAssemblyIsPassedThenThrows(
#if NETFRAMEWORK
            bool generateExecutable
#else
            OutputKind outputKind
#endif
            )
        {
            const string SourceCode = @"static class Program { static void Main() { } }";

#if NETFRAMEWORK
            var codeProvider = new CSharpCodeProvider();

            var compilerResults = codeProvider.CompileAssemblyFromSource(
                new CompilerParameters { GenerateInMemory = true, GenerateExecutable = generateExecutable },
                SourceCode);

            Assert.That(compilerResults, Is.Not.Null);
            Assert.That(compilerResults.Errors, Is.Empty);

            var assembly = compilerResults.CompiledAssembly.AssertNotNull();

#else
            var syntaxTree = CSharpSyntaxTree.ParseText(SourceCode);

            var compilation = CSharpCompilation
                .Create(
                    $"{nameof(TestGetLocalPathWhenInMemoryAssemblyIsPassedThenThrows)}_{Guid.NewGuid():N}",
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
                () => $@"Failed to compile an in-memory assembly:{Environment.NewLine}{
                    emitResult.Diagnostics.Select(d => $"* {d.GetMessage()}").Join(Environment.NewLine)}");

            stream.Position = 0;
            var assembly = AssemblyLoadContext.Default.LoadFromStream(stream).AssertNotNull();
#endif

            Assert.That(() => assembly.GetLocalPath(), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        [TestCase(AssemblyBuilderAccess.Run)]
        [TestCase(AssemblyBuilderAccess.RunAndCollect)]
#if NETFRAMEWORK
        [TestCase(AssemblyBuilderAccess.RunAndSave)]
#endif
        public void TestGetLocalPathWhenDynamicAssemblyIsPassedThenThrows(AssemblyBuilderAccess assemblyBuilderAccess)
        {
#if NETFRAMEWORK
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
#else
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
#endif
                new AssemblyName($"{nameof(TestGetLocalPathWhenDynamicAssemblyIsPassedThenThrows)}_{Guid.NewGuid():N}"),
                assemblyBuilderAccess);

            Assert.That(() => assemblyBuilder.GetLocalPath(), Throws.TypeOf<ArgumentException>());
        }
    }
}