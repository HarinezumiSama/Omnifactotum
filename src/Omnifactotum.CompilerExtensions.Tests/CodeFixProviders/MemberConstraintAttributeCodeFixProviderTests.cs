using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Omnifactotum.CompilerExtensions.Analyzers;
using Omnifactotum.CompilerExtensions.CodeFixProviders;
using Omnifactotum.NUnit;
using Verifier =
    Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
        Omnifactotum.CompilerExtensions.Analyzers.MemberConstraintAttributeAnalyzer,
        Omnifactotum.CompilerExtensions.CodeFixProviders.MemberConstraintAttributeCodeFixProvider,
        Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

#if NET7_0_OR_GREATER
using System.Collections.Immutable;
#endif

namespace Omnifactotum.CompilerExtensions.Tests.CodeFixProviders;

[TestFixture(TestOf = typeof(MemberConstraintAttributeCodeFixProvider))]
internal sealed class MemberConstraintAttributeCodeFixProviderTests : TestsBase
{
    public static IEnumerable<TestCaseData> CodeFixProviderTestCases
    {
        get
        {
#if NET7_0_OR_GREATER
            yield return new TestCaseData(
                    new CodeFixTestData
                    {
                        InitialSource =
                            // language=c#
                            """
                            using System;
                            using Omnifactotum.Validation.Annotations;
                            using Omnifactotum.Validation.Constraints;

                            public class Configuration
                            {
                                public string Property1 { get; set; }

                                [System.ComponentModel.BrowsableAttribute(false)]
                                public string Property2 { get; set; }

                                [Omnifactotum.Validation.Annotations.MemberConstraint(typeof(NotNullConstraint))]
                                [MemberItemConstraint(typeof(Omnifactotum.Validation.Constraints.NotNullWebUrlConstraint))]
                                public string[] Property3 { get; set; }

                                [MemberConstraint(typeof(GoodConstraint)), MemberItemConstraint(typeof(GoodItemConstraint))]
                                public char[] Property4 { get; set; }
                            }

                            internal class GoodConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class GoodItemConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.GenericValidationAttributeCanBeUsed)
                                .WithArguments("MemberConstraintAttribute", "MemberConstraintAttribute<TMemberConstraint>")
                                .WithLocation(12, 6),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.GenericValidationAttributeCanBeUsed)
                                .WithArguments("MemberItemConstraintAttribute", "MemberItemConstraintAttribute<TMemberConstraint>")
                                .WithLocation(13, 6),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.GenericValidationAttributeCanBeUsed)
                                .WithArguments("MemberConstraintAttribute", "MemberConstraintAttribute<TMemberConstraint>")
                                .WithLocation(16, 6),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.GenericValidationAttributeCanBeUsed)
                                .WithArguments("MemberItemConstraintAttribute", "MemberItemConstraintAttribute<TMemberConstraint>")
                                .WithLocation(16, 48)),
                        CodeActionIndex = 0,
                        FixedSource =
                            // language=c#
                            """
                            using System;
                            using Omnifactotum.Validation.Annotations;
                            using Omnifactotum.Validation.Constraints;

                            public class Configuration
                            {
                                public string Property1 { get; set; }

                                [System.ComponentModel.BrowsableAttribute(false)]
                                public string Property2 { get; set; }

                                [MemberConstraint<NotNullConstraint>]
                                [MemberItemConstraint<NotNullWebUrlConstraint>]
                                public string[] Property3 { get; set; }

                                [MemberConstraint<GoodConstraint>, MemberItemConstraint<GoodItemConstraint>]
                                public char[] Property4 { get; set; }
                            }

                            internal class GoodConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class GoodItemConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }
                            """
                    })
                .SetName(DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed);
#else
            yield break;
#endif
        }
    }

    [Test]
    [TestCaseSource(nameof(CodeFixProviderTestCases))]
    public async Task TestCodeFixProviderAsync(CodeFixTestData testData)
    {
        testData.AssertNotNull();

        var codeFixTest = new CSharpCodeFixTest<MemberConstraintAttributeAnalyzer, MemberConstraintAttributeCodeFixProvider, DefaultVerifier>
        {
            ReferenceAssemblies = CreateReferenceAssemblies().AddRuntimeAssemblies(typeof(Factotum).Assembly),
            TestCode = testData.InitialSource,
            FixedCode = testData.FixedSource,
            CodeActionIndex = testData.CodeActionIndex
        };

        codeFixTest.ExpectedDiagnostics.ReplaceItems(testData.ExpectedDiagnostics.AssertNotNull());
        codeFixTest.DisabledDiagnostics.ReplaceItems(testData.DisabledDiagnosticIds.EmptyIfNull());

        await codeFixTest.RunAsync();
    }
}