using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using Omnifactotum.CompilerExtensions.Analyzers;
using Omnifactotum.NUnit;
using Verifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    Omnifactotum.CompilerExtensions.Analyzers.MemberConstraintAttributeAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace Omnifactotum.CompilerExtensions.Tests.Analyzers;

[TestFixture(TestOf = typeof(MemberConstraintAttributeAnalyzer))]
internal sealed class MemberConstraintAttributeAnalyzerTests : TestsBase
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
                                [MemberConstraint(typeof(GoodConstraint))]
                                [MemberItemConstraint(typeof(GoodItemConstraint))]
                                public string[] Property3 { get; set; }

                            """
#if NET7_0_OR_GREATER
                            +
                            """

                                [MemberConstraint<NotNullConstraint>]
                                [MemberConstraint<GoodConstraint>]
                                [MemberItemConstraint<NotNullConstraint>]
                                [MemberItemConstraint<GoodItemConstraint>]
                            """
#else
                            +
                            """

                                [MemberConstraint(typeof(NotNullConstraint))]
                                [MemberConstraint(typeof(GoodConstraint))]
                                [MemberItemConstraint(typeof(NotNullConstraint))]
                                [MemberItemConstraint(typeof(GoodItemConstraint))]
                            """
#endif
                            +
                            """

                                public string[] Property4 { get; set; }

                                [MemberConstraint(typeof(int))]
                                [MemberConstraint(typeof(NoProperInterfaceConstraint))]
                                [MemberItemConstraint(typeof(int))]
                                [MemberItemConstraint(typeof(NoProperInterfaceItemConstraint))]
                                public int[] Property5 { get; set; }

                                [MemberConstraint(typeof(NoProperConstructorConstraint))]
                                [MemberItemConstraint(typeof(NoProperConstructorItemConstraint))]
                                public int[] Property6 { get; set; }
                            }

                            internal class GoodConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class GoodItemConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class NoProperInterfaceConstraint : IDisposable
                            {
                                public void Dispose() {}
                            }

                            internal class NoProperInterfaceItemConstraint : IDisposable
                            {
                                public void Dispose() {}
                            }

                            internal class NoProperConstructorConstraint : IMemberConstraint
                            {
                                public NoProperConstructorConstraint(int parameter) {}

                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class NoProperConstructorItemConstraint : IMemberConstraint
                            {
                                public NoProperConstructorItemConstraint(int parameter) {}

                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }
                            """,
                        DisabledDiagnosticIds = ImmutableList.Create(DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed),
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface)
                                .WithArguments("int")
                                .WithLocation(24, 30),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface)
                                .WithArguments("NoProperInterfaceConstraint")
                                .WithLocation(25, 30),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface)
                                .WithArguments("int")
                                .WithLocation(26, 34),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface)
                                .WithArguments("NoProperInterfaceItemConstraint")
                                .WithLocation(27, 34),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNoParameterlessConstructor)
                                .WithArguments("NoProperConstructorConstraint")
                                .WithLocation(30, 30),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeNoParameterlessConstructor)
                                .WithArguments("NoProperConstructorItemConstraint")
                                .WithLocation(31, 34))
                    })
                .SetName(
                    $"{DiagnosticDescriptorIds.ValidationConstraintTypeNotImplementsInterface}|{
                        DiagnosticDescriptorIds.ValidationConstraintTypeNoParameterlessConstructor}");

#if NET7_0_OR_GREATER
            yield return new TestCaseData(
                    new AnalyzerTestData
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

                                [MemberConstraint(typeof(int))]
                                [MemberConstraint(typeof(NoProperInterfaceConstraint))]
                                [MemberItemConstraint(typeof(int))]
                                [MemberItemConstraint(typeof(NoProperInterfaceItemConstraint))]
                                public int[] Property5 { get; set; }

                                [MemberConstraint(typeof(NoProperConstructorConstraint))]
                                [MemberItemConstraint(typeof(NoProperConstructorItemConstraint))]
                                public int[] Property6 { get; set; }
                            }

                            internal class GoodConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class GoodItemConstraint : IMemberConstraint
                            {
                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class NoProperInterfaceConstraint : IDisposable
                            {
                                public void Dispose() {}
                            }

                            internal class NoProperInterfaceItemConstraint : IDisposable
                            {
                                public void Dispose() {}
                            }

                            internal class NoProperConstructorConstraint : IMemberConstraint
                            {
                                public NoProperConstructorConstraint(int parameter) {}

                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }

                            internal class NoProperConstructorItemConstraint : IMemberConstraint
                            {
                                public NoProperConstructorItemConstraint(int parameter) {}

                                public void Validate(MemberConstraintValidationContext memberContext, object? value) {}
                            }
                            """,
                        DisabledDiagnosticIds = ImmutableList.Create(
                            DiagnosticDescriptorIds.ValidationConstraintTypeNotImplementsInterface,
                            DiagnosticDescriptorIds.ValidationConstraintTypeNoParameterlessConstructor),
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
                                .WithLocation(16, 48))
                    })
                .SetName(DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed);
#endif

            yield return new TestCaseData(
                    new AnalyzerTestData
                    {
                        InitialSource =
                            // language=c#
                            """
                            using System;
                            using System.Collections.Generic;
                            using Omnifactotum.Validation.Annotations;
                            using Omnifactotum.Validation.Constraints;

                            public class Configuration
                            {
                                [MemberConstraint(typeof(NotNullConstraint<string>))]
                                public bool MemberError { get; set; }

                                [MemberConstraint(typeof(NotNullConstraint<string>))]
                                public string MemberOk { get; set; }

                                [MemberConstraint(typeof(NotNullConstraint<string>))]
                                public string? MemberOkNullable { get; set; }

                                [MemberConstraint(typeof(NotNullConstraint))]
                                public bool MemberOkUntypedConstraint { get; set; }

                                [MemberItemConstraint(typeof(NotNullConstraint<string>))]
                                public object[] ItemError { get; set; }

                                [MemberItemConstraint(typeof(NotNullConstraint<object>))]
                                public bool?[] ItemOk { get; set; }

                                [MemberItemConstraint(typeof(NotNullConstraint<string>))]
                                public string ItemOnStringSkipped { get; set; }

                                [MemberConstraint(typeof(NotNullAndNotEmptyCollectionConstraint<int>))]
                                public List<string> CollectionMemberError { get; set; }

                                [MemberConstraint(typeof(NotNullAndNotEmptyCollectionConstraint<string>))]
                                public List<string> CollectionMemberOk { get; set; }
                            }

                            public class GenericConfiguration<T>
                            {
                                [MemberConstraint(typeof(NotNullConstraint<Action>))]
                                public Func<T, bool>? DelegateMemberError { get; set; }

                                [MemberConstraint(typeof(NotNullAndNotEmptyCollectionConstraint<string>))]
                                public List<T> ConditionalCollectionOk { get; set; }
                            }
                            """,
                        DisabledDiagnosticIds = ImmutableList.Create(DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed),
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<string>", "string?", "member", "bool")
                                .WithLocation(8, 30),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<string>", "string?", "collection item", "object")
                                .WithLocation(20, 34),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullAndNotEmptyCollectionConstraint<int>", "IEnumerable<int>?", "member", "List<string>")
                                .WithLocation(29, 30),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<Action>", "Action?", "member", "Func<T, bool>?")
                                .WithLocation(38, 30))
                    })
                .SetName($"{DiagnosticDescriptorIds.ValidationConstraintTypeIncompatibleWithMemberType} (non-generic attribute form)");

#if NET7_0_OR_GREATER
            yield return new TestCaseData(
                    new AnalyzerTestData
                    {
                        InitialSource =
                            // language=c#
                            """
                            using System;
                            using Omnifactotum.Validation.Annotations;
                            using Omnifactotum.Validation.Constraints;

                            public class Configuration
                            {
                                [MemberConstraint<NotNullConstraint<string>>]
                                public bool MemberError { get; set; }

                                [MemberConstraint<NotNullConstraint<string>>]
                                public string MemberOk { get; set; }

                                [MemberItemConstraint<NotNullConstraint<string>>]
                                public object[] ItemError { get; set; }

                                [MemberItemConstraint<NotNullConstraint<object>>]
                                public object[] ItemOk { get; set; }

                                [MemberConstraint<EnumValueDefinedConstraint<DayOfWeek>>]
                                public DayOfWeek EnumMemberOk { get; set; }

                                [MemberConstraint<EnumValueDefinedConstraint<DayOfWeek>>]
                                public DayOfWeek? EnumMemberError { get; set; }

                                [MemberConstraint<NullableEnumValueDefinedConstraint<DayOfWeek>>]
                                public DayOfWeek NullableEnumMemberOk { get; set; }

                                [MemberConstraint<NullableEnumValueDefinedConstraint<DayOfWeek>>]
                                public DayOfWeek? NullableEnumMemberOkNullable { get; set; }
                            }

                            public class GenericConfiguration<T>
                            {
                                [MemberConstraint<NotNullConstraint<Action>>]
                                public Func<T, bool>? DelegateMemberError { get; set; }

                                [MemberConstraint<NotNullConstraint<Delegate>>]
                                public Func<T, bool>? DelegateMemberOk { get; set; }
                            }
                            """,
                        ExpectedDiagnostics = ImmutableList.Create(
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<string>", "string?", "member", "bool")
                                .WithLocation(7, 23),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<string>", "string?", "collection item", "object")
                                .WithLocation(13, 27),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("EnumValueDefinedConstraint<DayOfWeek>", "DayOfWeek", "member", "DayOfWeek?")
                                .WithLocation(22, 23),
                            Verifier
                                .Diagnostic(DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType)
                                .WithArguments("NotNullConstraint<Action>", "Action?", "member", "Func<T, bool>?")
                                .WithLocation(34, 23))
                    })
                .SetName($"{DiagnosticDescriptorIds.ValidationConstraintTypeIncompatibleWithMemberType} (generic attribute form)");
#endif
        }
    }

    [Test]
    [TestCaseSource(nameof(AnalyzerTestCases))]
    public async Task TestAnalyzerAsync(AnalyzerTestData testData)
    {
        testData.AssertNotNull();

        var analyzerTest = new CSharpAnalyzerTest<MemberConstraintAttributeAnalyzer, DefaultVerifier>
        {
            ReferenceAssemblies = CreateReferenceAssemblies().AddRuntimeAssemblies(typeof(Factotum).Assembly),
            TestCode = testData.InitialSource
        };

        analyzerTest.ExpectedDiagnostics.ReplaceItems(testData.ExpectedDiagnostics.AssertNotNull());
        analyzerTest.DisabledDiagnostics.ReplaceItems(testData.DisabledDiagnosticIds.EmptyIfNull());

        await analyzerTest.RunAsync();
    }
}