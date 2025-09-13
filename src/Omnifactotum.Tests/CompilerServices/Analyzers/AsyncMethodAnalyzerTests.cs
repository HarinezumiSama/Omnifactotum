using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using Omnifactotum.CompilerServices;
using Omnifactotum.CompilerServices.Analyzers;
using Omnifactotum.NUnit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.NUnit.AnalyzerVerifier<Omnifactotum.CompilerServices.Analyzers.AsyncMethodAnalyzer>;

namespace Omnifactotum.Tests.CompilerServices.Analyzers;

[TestFixture(TestOf = typeof(AsyncMethodAnalyzer))]
internal sealed class AsyncMethodAnalyzerTests : TestsBase
{
    public static IEnumerable<TestCaseData> AnalyzerTestCases
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        get
        {
            yield return new TestCaseData(
                    new AnalyzerTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass : ITestInterface
                            {
                                public Task CorrectPublicVoidTaskAsync(string value) => Task.CompletedTask;
                                public Task<string> CorrectPublicResultTaskAsync(string value) => Task.FromResult(string.Empty);

                                public async ValueTask CorrectPublicVoidValueTaskAsync(string value) => await Task.CompletedTask;
                                public async ValueTask<string> CorrectPublicResultValueTaskAsync(string value) => await Task.FromResult(string.Empty);

                                public void CorrectPublicVoid(string value) {}
                                public string CorrectPublicResult(string value) => string.Empty;

                                private Task CorrectPrivateVoidTaskAsync(char value) => Task.CompletedTask;
                                private Task<string> CorrectPrivateResultTaskAsync(char value) => Task.FromResult(string.Empty);

                                private async ValueTask CorrectPrivateVoidValueTaskAsync(char value) => await Task.CompletedTask;
                                private async ValueTask<string> CorrectPrivateResultValueTaskAsync(char value) => await Task.FromResult(string.Empty);

                                private void CorrectPrivateVoid(char value) {}
                                private string CorrectPrivateResult(char value) => string.Empty;

                                Task ITestInterface.CorrectInterfaceVoidTaskAsync(string value) => Task.CompletedTask;
                                Task<string> ITestInterface.CorrectInterfaceResultTaskAsync(string value) => Task.FromResult(string.Empty);

                                async ValueTask ITestInterface.CorrectInterfaceVoidValueTaskAsync(string value) => await Task.CompletedTask;
                                async ValueTask<string> ITestInterface.CorrectInterfaceResultValueTaskAsync(string value) => await Task.FromResult(string.Empty);

                                public void PublicContainingMethod()
                                {
                                    Task CorrectLocalVoidTaskAsync(string value) => Task.CompletedTask;
                                    Task<string> CorrectLocalResultTaskAsync(string value) => Task.FromResult(string.Empty);

                                    async ValueTask CorrectLocalVoidValueTaskAsync(string value) => await Task.CompletedTask;
                                    async ValueTask<string> CorrectLocalResultValueTaskAsync(string value) => await Task.FromResult(string.Empty);
                                }

                                private void PrivateContainingMethod()
                                {
                                    Task CorrectLocalVoidTaskAsync(string value) => Task.CompletedTask;
                                    Task<string> CorrectLocalResultTaskAsync(string value) => Task.FromResult(string.Empty);

                                    async ValueTask CorrectLocalVoidValueTaskAsync(string value) => await Task.CompletedTask;
                                    async ValueTask<string> CorrectLocalResultValueTaskAsync(string value) => await Task.FromResult(string.Empty);
                                }
                            }

                            public interface ITestInterface
                            {
                                Task CorrectInterfaceVoidTaskAsync(string value);
                                Task<string> CorrectInterfaceResultTaskAsync(string value);

                                ValueTask CorrectInterfaceVoidValueTaskAsync(string value);
                                ValueTask<string> CorrectInterfaceResultValueTaskAsync(string value);
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList<DiagnosticResult>.Empty
                    })
                .SetName(
                    $"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}/{
                        nameof(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)}: Correct names.");

            yield return new TestCaseData(
                    new AnalyzerTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass : ITestInterface
                            {
                                public Task IncorrectPublicVoidTaskAsyn(string value) => Task.CompletedTask;
                                public Task<string> IncorrectPublicResultTaskAsynch(string value) => Task.FromResult(string.Empty);

                                public async ValueTask IncorrectPublicVoidValueTaskasync(string value) => await Task.CompletedTask;
                                public async ValueTask<string> IncorrectPublicResultValueTaskASYNC(string value) => await Task.FromResult(string.Empty);

                                public void IncorrectPublicVoidAsync(string value) {}
                                public string IncorrectPublicResultAsync(string value) => string.Empty;

                                private Task IncorrectPrivateVoidTaskAsyn(char value) => Task.CompletedTask;
                                private Task<string> IncorrectPrivateResultTaskAsynch(char value) => Task.FromResult(string.Empty);

                                private async ValueTask IncorrectPrivateVoidValueTaskaSync(char value) => await Task.CompletedTask;
                                private async ValueTask<string> IncorrectPrivateResultValueTaskAsynC(char value) => await Task.FromResult(string.Empty);

                                private void IncorrectPrivateVoidAsync<T>(T value) {}
                                private string IncorrectPrivateResultAsync<T>(T value) => string.Empty;

                                Task ITestInterface.IncorrectInterfaceVoidTask(string value) => Task.CompletedTask;
                                Task<string> ITestInterface.IncorrectInterfaceResultTask(string value) => Task.FromResult(string.Empty);

                                async ValueTask ITestInterface.IncorrectInterfaceVoidValueTask(string value) => await Task.CompletedTask;
                                async ValueTask<string> ITestInterface.IncorrectInterfaceResultValueTask(string value) => await Task.FromResult(string.Empty);

                                public void PublicContainingMethod()
                                {
                                    Task IncorrectLocalVoidTask(string value) => Task.CompletedTask;
                                    Task<string> IncorrectLocalResultTask(string value) => Task.FromResult(string.Empty);

                                    async ValueTask IncorrectLocalVoidValueTask(string value) => await Task.CompletedTask;
                                    async ValueTask<string> IncorrectLocalResultValueTask(string value) => await Task.FromResult(string.Empty);
                                }

                                private void PrivateContainingMethod()
                                {
                                    Task AsyncIncorrectLocalVoidTask(string value) => Task.CompletedTask;
                                    Task<string> AsyncIncorrectLocalResultTask(string value) => Task.FromResult(string.Empty);

                                    async ValueTask AsyncIncorrectLocalVoidValueTask(string value) => await Task.CompletedTask;
                                    async ValueTask<string> AsyncIncorrectLocalResultValueTask(string value) => await Task.FromResult(string.Empty);
                                }
                            }

                            public interface ITestInterface
                            {
                                Task IncorrectInterfaceVoidTask(string value);
                                Task<string> IncorrectInterfaceResultTask(string value);

                                ValueTask IncorrectInterfaceVoidValueTask(string value);
                                ValueTask<string> IncorrectInterfaceResultValueTask(string value);
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            //// Methods
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidTaskAsyn(string)")
                                .WithSpan(6, 17, 6, 44),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultTaskAsynch(string)")
                                .WithSpan(7, 25, 7, 56),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidValueTaskasync(string)")
                                .WithSpan(9, 28, 9, 61),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultValueTaskASYNC(string)")
                                .WithSpan(10, 36, 10, 71),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidAsync(string)")
                                .WithSpan(12, 17, 12, 41),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultAsync(string)")
                                .WithSpan(13, 19, 13, 45),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidTaskAsyn(char)")
                                .WithSpan(15, 18, 15, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultTaskAsynch(char)")
                                .WithSpan(16, 26, 16, 58),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidValueTaskaSync(char)")
                                .WithSpan(18, 29, 18, 63),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultValueTaskAsynC(char)")
                                .WithSpan(19, 37, 19, 73),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidAsync<T>(T)")
                                .WithSpan(21, 18, 21, 43),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultAsync<T>(T)")
                                .WithSpan(22, 20, 22, 47),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.ITestInterface.IncorrectInterfaceVoidTask(string)")
                                .WithSpan(24, 25, 24, 51),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.ITestInterface.IncorrectInterfaceResultTask(string)")
                                .WithSpan(25, 33, 25, 61),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.ITestInterface.IncorrectInterfaceVoidValueTask(string)")
                                .WithSpan(27, 36, 27, 67),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.ITestInterface.IncorrectInterfaceResultValueTask(string)")
                                .WithSpan(28, 44, 28, 77),

                            //// Local functions
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalVoidTask(string)")
                                .WithSpan(32, 14, 32, 36),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultTask(string)")
                                .WithSpan(33, 22, 33, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalVoidValueTask(string)")
                                .WithSpan(35, 25, 35, 52),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultValueTask(string)")
                                .WithSpan(36, 33, 36, 62),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalVoidTask(string)")
                                .WithSpan(41, 14, 41, 41),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalResultTask(string)")
                                .WithSpan(42, 22, 42, 51),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalVoidValueTask(string)")
                                .WithSpan(44, 25, 44, 57),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalResultValueTask(string)")
                                .WithSpan(45, 33, 45, 67),

                            //// Interface methods
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidTask(string)")
                                .WithSpan(51, 10, 51, 36),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceResultTask(string)")
                                .WithSpan(52, 18, 52, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidValueTask(string)")
                                .WithSpan(54, 15, 54, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceResultValueTask(string)")
                                .WithSpan(55, 23, 55, 56))
                    })
                .SetName(
                    $"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}/{
                        nameof(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)}: Incorrect names.");
        }
    }

    [Test]
    [TestCaseSource(nameof(AnalyzerTestCases))]
    public async Task TestAnalyzerAsync(AnalyzerTestData testData)
    {
        testData.AssertNotNull();

        var analyzerTest = new CSharpAnalyzerTest<AsyncMethodAnalyzer, NUnitVerifier>
        {
            ReferenceAssemblies = CreateReferenceAssemblies(),
            TestCode = testData.InitialSource
        };

        analyzerTest.ExpectedDiagnostics.ReplaceItems(testData.ExpectedDiagnostics.AssertNotNull());
        analyzerTest.DisabledDiagnostics.ReplaceItems(testData.DisabledDiagnosticIds.EmptyIfNull());

        await analyzerTest.RunAsync();
    }
}