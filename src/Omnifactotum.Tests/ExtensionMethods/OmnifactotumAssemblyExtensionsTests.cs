using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if NETFRAMEWORK
using System.Reflection.Emit;
#endif
using Microsoft.CSharp;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    internal sealed class OmnifactotumAssemblyExtensionsTests
    {
        [Test]
        public void TestGetLocalPathWhenLocalAssemblyIsPassedThenSucceeds()
            => Assert.That(GetType().Assembly.GetLocalPath(), Is.Not.Null.And.Not.Empty);

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestGetLocalPathWhenNullAssemblyIsPassedThenThrows()
            => Assert.That(() => ((Assembly)null).GetLocalPath(), Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void TestGetLocalPathWhenInMemoryAssemblyIsPassedThenThrows()
        {
            var codeProvider = new CSharpCodeProvider();

            var compilerResults = codeProvider.CompileAssemblyFromSource(
                new CompilerParameters { GenerateInMemory = true });

            var assembly = compilerResults.CompiledAssembly.AssertNotNull();

            Assert.That(() => assembly.GetLocalPath(), Throws.TypeOf<ArgumentException>());
        }

#if NETFRAMEWORK
        [Test]
        public void TestGetLocalPathWhenDynamicAssemblyIsPassedThenThrows()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Test_" + Guid.NewGuid().ToString("N")),
                AssemblyBuilderAccess.RunAndSave);

            Assert.That(() => assemblyBuilder.GetLocalPath(), Throws.TypeOf<ArgumentException>());
        }
#endif
    }
}