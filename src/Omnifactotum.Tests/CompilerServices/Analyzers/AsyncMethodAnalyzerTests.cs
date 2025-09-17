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
                            using System.Collections.Generic;
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

                                internal static async IAsyncEnumerable<int> CorrectAsyncEnumerableAsync(char c) { await Task.CompletedTask; yield break; }

                                private Task CorrectPrivateVoidTaskAsync(char value) => Task.CompletedTask;
                                private Task<string> CorrectPrivateResultTaskAsync(char value) => Task.FromResult(string.Empty);

                                private async ValueTask CorrectPrivateVoidValueTaskAsync(char value) => await Task.CompletedTask;
                                private async ValueTask<string> CorrectPrivateResultValueTaskAsync(char value) => await Task.FromResult(string.Empty);

                                private void CorrectPrivateVoid(char value) {}
                                private string CorrectPrivateResult(char value) => string.Empty;

                                Task ITestInterface.CorrectInterfaceVoidTaskAsync(string value) => Task.CompletedTask;
                                public Task<string> CorrectInterfaceResultTaskAsync(string value) => Task.FromResult(string.Empty);

                                async ValueTask ITestInterface.CorrectInterfaceVoidValueTaskAsync(string value) => await Task.CompletedTask;
                                public async ValueTask<string> CorrectInterfaceResultValueTaskAsync(string value) => await Task.FromResult(string.Empty);

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
                            using System.Collections.Generic;
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass : TestClassBase, ITestInterface
                            {
                                public Task IncorrectPublicVoidTaskAsyn(string value) => Task.CompletedTask;
                                public Task<string> IncorrectPublicResultTaskAsynch(string value) => Task.FromResult(string.Empty);

                                public async ValueTask IncorrectPublicVoidValueTaskasync(string value) => await Task.CompletedTask;
                                public async ValueTask<string> IncorrectPublicResultValueTaskASYNC(string value) => await Task.FromResult(string.Empty);

                                public void IncorrectPublicVoidAsync(string value) {}
                                public string IncorrectPublicResultAsync(string value) => string.Empty;

                                internal static async IAsyncEnumerable<int> IncorrectAsyncEnumerable(char c) { await Task.CompletedTask; yield break; }

                                private Task IncorrectPrivateVoidTaskAsyn(char value) => Task.CompletedTask;
                                private Task<string> IncorrectPrivateResultTaskAsynch(char value) => Task.FromResult(string.Empty);

                                private async ValueTask IncorrectPrivateVoidValueTaskaSync(char value) => await Task.CompletedTask;
                                private async ValueTask<string> IncorrectPrivateResultValueTaskAsynC(char value) => await Task.FromResult(string.Empty);

                                private void IncorrectPrivateVoidAsync<T>(T value) {}
                                private string IncorrectPrivateResultAsync<T>(T value) => string.Empty;

                                Task ITestInterface.IncorrectInterfaceVoidTask(string value) => Task.CompletedTask;
                                public Task<string> IncorrectInterfaceResultTask(string value) => Task.FromResult(string.Empty);

                                async ValueTask ITestInterface.IncorrectInterfaceVoidValueTask(string value) => await Task.CompletedTask;
                                public async ValueTask<string> IncorrectInterfaceResultValueTask(string value) => await Task.FromResult(string.Empty);

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

                                public override Task IncorrectBaseClassVoidTask() => Task.CompletedTask;
                                protected override int IncorrectBaseClassResultAsync() => 0;
                            }

                            public interface ITestInterface
                            {
                                Task IncorrectInterfaceVoidTask(string value);
                                Task<string> IncorrectInterfaceResultTask(string value);

                                ValueTask IncorrectInterfaceVoidValueTask(string value);
                                ValueTask<string> IncorrectInterfaceResultValueTask(string value);
                            }

                            public abstract class TestClassBase
                            {
                                public virtual Task IncorrectBaseClassVoidTask() => Task.CompletedTask;
                                protected abstract int IncorrectBaseClassResultAsync();
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            //// Methods
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidTaskAsyn(string)")
                                .WithSpan(7, 17, 7, 44),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultTaskAsynch(string)")
                                .WithSpan(8, 25, 8, 56),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidValueTaskasync(string)")
                                .WithSpan(10, 28, 10, 61),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultValueTaskASYNC(string)")
                                .WithSpan(11, 36, 11, 71),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidAsync(string)")
                                .WithSpan(13, 17, 13, 41),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicResultAsync(string)")
                                .WithSpan(14, 19, 14, 45),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectAsyncEnumerable(char)")
                                .WithSpan(16, 49, 16, 73),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidTaskAsyn(char)")
                                .WithSpan(18, 18, 18, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultTaskAsynch(char)")
                                .WithSpan(19, 26, 19, 58),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidValueTaskaSync(char)")
                                .WithSpan(21, 29, 21, 63),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultValueTaskAsynC(char)")
                                .WithSpan(22, 37, 22, 73),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateVoidAsync<T>(T)")
                                .WithSpan(24, 18, 24, 43),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPrivateResultAsync<T>(T)")
                                .WithSpan(25, 20, 25, 47),

                            //// Local functions
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalVoidTask(string)")
                                .WithSpan(35, 14, 35, 36),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultTask(string)")
                                .WithSpan(36, 22, 36, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalVoidValueTask(string)")
                                .WithSpan(38, 25, 38, 52),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultValueTask(string)")
                                .WithSpan(39, 33, 39, 62),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalVoidTask(string)")
                                .WithSpan(44, 14, 44, 41),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalResultTask(string)")
                                .WithSpan(45, 22, 45, 51),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalVoidValueTask(string)")
                                .WithSpan(47, 25, 47, 57),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "AsyncIncorrectLocalResultValueTask(string)")
                                .WithSpan(48, 33, 48, 67),

                            //// Interface methods
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidTask(string)")
                                .WithSpan(57, 10, 57, 36),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceResultTask(string)")
                                .WithSpan(58, 18, 58, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidValueTask(string)")
                                .WithSpan(60, 15, 60, 46),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceResultValueTask(string)")
                                .WithSpan(61, 23, 61, 56),

                            //// Base class methods
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "TestClassBase.IncorrectBaseClassVoidTask()")
                                .WithSpan(66, 25, 66, 51),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClassBase.IncorrectBaseClassResultAsync()")
                                .WithSpan(67, 28, 67, 57))
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