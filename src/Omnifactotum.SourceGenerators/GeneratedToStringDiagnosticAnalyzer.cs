using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.SourceGenerators;

#if DEBUG
/// <summary>
///     TODO
/// </summary>
#endif
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class GeneratedToStringDiagnosticAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(
            DiagnosticDescriptors.GeneratedToString.InvalidModifiers,
            DiagnosticDescriptors.GeneratedToString.IncompatibleCSharpVersion,
            DiagnosticDescriptors.GeneratedToString.ToStringMethodAlreadyDefined);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeSyntaxNodes,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration);

        static void AnalyzeSyntaxNodes(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not TypeDeclarationSyntax typeDeclarationSyntax
                || !typeDeclarationSyntax.IsAnnotatedWithGeneratedToStringAttribute(context.SemanticModel))
            {
                return;
            }

            typeDeclarationSyntax.ReportGeneratedToStringAnalysisErrors(context.Compilation, context.ReportDiagnostic);
        }
    }
}