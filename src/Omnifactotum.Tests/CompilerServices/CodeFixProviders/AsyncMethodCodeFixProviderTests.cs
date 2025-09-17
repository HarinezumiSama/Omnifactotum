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
            //// AsyncMethodMissingAsyncSuffix

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
                                ValueTask<T> IncorrectInterfaceVoidValueTask<T>(string value);
                            }

                            public abstract class TestClassBase : ITestInterface
                            {
                                public abstract ValueTask<T> IncorrectInterfaceVoidValueTask<T>(string value);
                            }

                            public sealed class TestClass : TestClassBase
                            {
                                public override async ValueTask<T> IncorrectInterfaceVoidValueTask<T>(string value)
                                    => await Task.FromResult<T>(default(T));
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidValueTask<T>(string)")
                                .WithSpan(6, 18, 6, 49)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            internal interface ITestInterface
                            {
                                ValueTask<T> IncorrectInterfaceVoidValueTaskAsync<T>(string value);
                            }

                            public abstract class TestClassBase : ITestInterface
                            {
                                public abstract ValueTask<T> IncorrectInterfaceVoidValueTaskAsync<T>(string value);
                            }

                            public sealed class TestClass : TestClassBase
                            {
                                public override async ValueTask<T> IncorrectInterfaceVoidValueTaskAsync<T>(string value)
                                    => await Task.FromResult<T>(default(T));
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}: Interface method");

            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Collections.Generic;
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                internal void PublicContainingMethod()
                                {
                                    async IAsyncEnumerable<string> IncorrectLocalAsyncEnumerable(int value) { await Task.CompletedTask; yield break; }
                                }
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalAsyncEnumerable(int)")
                                .WithSpan(9, 40, 9, 69)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Collections.Generic;
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                internal void PublicContainingMethod()
                                {
                                    async IAsyncEnumerable<string> IncorrectLocalAsyncEnumerableAsync(int value) { await Task.CompletedTask; yield break; }
                                }
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix)}: Local function");

            //// SyncMethodHasAsyncSuffix

            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                public void IncorrectPublicVoidAsync(string value) {}
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "TestClass.IncorrectPublicVoidAsync(string)")
                                .WithSpan(6, 17, 6, 41)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                public void IncorrectPublicVoid(string value) {}
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)}: Regular method");

            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            internal interface ITestInterface
                            {
                                void IncorrectInterfaceVoidAsync(string value);
                            }

                            public abstract class TestClassBase : ITestInterface
                            {
                                public abstract void IncorrectInterfaceVoidAsync(string value);
                            }

                            public sealed class TestClass : TestClassBase
                            {
                                public override void IncorrectInterfaceVoidAsync(string value) {}
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("method", "ITestInterface.IncorrectInterfaceVoidAsync(string)")
                                .WithSpan(6, 10, 6, 37)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            internal interface ITestInterface
                            {
                                void IncorrectInterfaceVoid(string value);
                            }

                            public abstract class TestClassBase : ITestInterface
                            {
                                public abstract void IncorrectInterfaceVoid(string value);
                            }

                            public sealed class TestClass : TestClassBase
                            {
                                public override void IncorrectInterfaceVoid(string value) {}
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)}: Interface method");

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
                                    string IncorrectLocalResultAsync<T>(T value) => string.Empty;
                                }
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)
                                .WithArguments("local function", "IncorrectLocalResultAsync<T>(T)")
                                .WithSpan(8, 16, 8, 41)),
                        CodeActionIndex = 0,
                        FixedSource =
                            """
                            using System.Threading;
                            using System.Threading.Tasks;

                            public class TestClass
                            {
                                internal void PublicContainingMethod()
                                {
                                    string IncorrectLocalResult<T>(T value) => string.Empty;
                                }
                            }
                            """
                    })
                .SetName($"{nameof(DiagnosticDescriptors.SyncMethodHasAsyncSuffix)}: Local function");
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