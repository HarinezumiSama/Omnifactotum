using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.Extensions
{
    //// ReSharper disable AssignNullToNotNullAttribute - Intentionally for tests
    [TestFixture]
    public sealed class OmnifactotumAssemblyExtensionsTests
    {
        #region Tests

        [Test]
        public void TestGetLocalPathUsingNullAssemblyNegative()
        {
            Assert.That(() => ((Assembly)null).GetLocalPath(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestGetLocalPathOfInMemoryAssemblyNegative()
        {
            var codeProvider = new CSharpCodeProvider();
            var compilerResults = codeProvider.CompileAssemblyFromSource(
                new CompilerParameters { GenerateInMemory = true });
            var assembly = compilerResults.CompiledAssembly.AssertNotNull();

            Assert.That(() => assembly.GetLocalPath(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void TestGetLocalPathOfDynamicAssemblyNegative()
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Test_" + Guid.NewGuid().ToString("N")), 
                AssemblyBuilderAccess.RunAndSave);

            Assert.That(() => assemblyBuilder.GetLocalPath(), Throws.TypeOf<ArgumentException>());
        }

        #endregion
    }
}