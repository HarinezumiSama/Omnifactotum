using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using Omnifactotum.CompilerServices;
using Omnifactotum.CompilerServices.Analyzers;
using Omnifactotum.CompilerServices.CodeFixProviders;
using Omnifactotum.NUnit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.NUnit.CodeFixVerifier<
        Omnifactotum.CompilerServices.Analyzers.AsyncMethodAnalyzer,
        Omnifactotum.CompilerServices.CodeFixProviders.AsyncMethodCodeFixProvider>;

namespace Omnifactotum.Tests.CompilerServices.CodeFixProviders;

[TestFixture(TestOf = typeof(AsyncMethodCodeFixProvider))]
internal sealed class AsyncMethodCodeFixProviderTests : TestsBase
{
    public static IEnumerable<TestCaseData> CodeFixProviderTestCases
    {
        get
        {
            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                public Task IncorrectPublicVoidTask<T>(T value) => Task.CompletedTask;
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidTask<T>(T)")
                                .WithSpan(6, 17, 6, 40)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                public Task IncorrectPublicVoidTaskAsync<T>(T value) => Task.CompletedTask;
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}: Regular method");

            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            internal interface ITestInterface
                            {
                                ValueTask IncorrectInterfaceVoidValueTask(string value);
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidValueTask(string)")
                                .WithSpan(6, 15, 6, 46)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            internal interface ITestInterface
                            {
                                ValueTask IncorrectInterfaceVoidValueTaskAsync(string value);
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}: Interface method");

            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                internal void PublicContainingMethod()
                                {
                                    Task<string> IncorrectLocalResultTask<T>(T value) => Task.FromResult(string.Empty);
                                }
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultTask<T>(T)")
                                .WithSpan(8, 22, 8, 46)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                internal void PublicContainingMethod()
                                {
                                    Task<string> IncorrectLocalResultTaskAsync<T>(T value) => Task.FromResult(string.Empty);
                                }
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}: Local function");
        }
    }

    [Test]
    [TestCaseSource(nameof(CodeFixProviderTestCases))]
    public async Task TestCodeFixProviderAsync(CodeFixTestData testData)
    {
        testData.AssertNotNull();

        var codeFixTest = new CSharpCodeFixTest<AsyncMethodAnalyzer, AsyncMethodCodeFixProvider, NUnitVerifier>
        {
            ReferenceAssemblies = CreateReferenceAssemblies(),
            TestCode = testData.InitialSource,
            FixedCode = testData.FixedSource,
            CodeActionIndex = testData.CodeActionIndex
        };

        codeFixTest.ExpectedDiagnostics.ReplaceItems(testData.ExpectedDiagnostics.AssertNotNull());
        codeFixTest.DisabledDiagnostics.ReplaceItems(testData.DisabledDiagnosticIds.EmptyIfNull());

        await codeFixTest.RunAsync();
    }
}