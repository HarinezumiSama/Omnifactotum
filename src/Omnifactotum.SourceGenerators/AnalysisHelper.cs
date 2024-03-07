using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Omnifactotum.SourceGenerators;

internal static class AnalysisHelper
{
    [SuppressMessage("ReSharper", "InvertIf")]
    public static bool ReportGeneratedToStringAnalysisErrors(
        this TypeDeclarationSyntax syntaxNode,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        if (syntaxNode is null)
        {
            throw new ArgumentNullException(nameof(syntaxNode));
        }

        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        if (reportDiagnostic is null)
        {
            throw new ArgumentNullException(nameof(reportDiagnostic));
        }

        const LanguageVersion MinimumLanguageVersion = LanguageVersion.CSharp11;

        var effectiveLanguageVersion =
            compilation is CSharpCompilation { LanguageVersion: var languageVersion }
                ? languageVersion
                : LanguageVersion.Default.MapSpecifiedToEffectiveVersion();

        if (effectiveLanguageVersion < MinimumLanguageVersion)
        {
            reportDiagnostic(
                CreateDiagnostic(
                    syntaxNode,
                    DiagnosticDescriptors.GeneratedToString.IncompatibleCSharpVersion,
                    effectiveLanguageVersion.ToDisplayString(),
                    MinimumLanguageVersion.ToDisplayString()));

            return true;
        }

        if (!syntaxNode.Modifiers.Any(SyntaxKind.PartialKeyword)
            || syntaxNode.Modifiers.Any(SyntaxKind.StaticKeyword)
            || syntaxNode.Modifiers.Any(SyntaxKind.FileKeyword))
        {
            reportDiagnostic(CreateDiagnostic(syntaxNode, DiagnosticDescriptors.GeneratedToString.InvalidModifiers));
            return true;
        }

        var alreadyHasToStringMethod = syntaxNode.Members.Any(
            syntax => syntax is MethodDeclarationSyntax
            {
                Identifier.Text: nameof(ToString),
                ParameterList.Parameters.Count: 0,
                Arity: 0
            });

        if (alreadyHasToStringMethod)
        {
            reportDiagnostic(CreateDiagnostic(syntaxNode, DiagnosticDescriptors.GeneratedToString.ToStringMethodAlreadyDefined));
            return true;
        }

        return false;

        static Location GetTypeNodeLocation(TypeDeclarationSyntax typeDeclarationSyntax)
        {
            var token = typeDeclarationSyntax.Modifiers.Any()
                ? typeDeclarationSyntax.Modifiers.First()
                : typeDeclarationSyntax.Keyword;

            return Location.Create(
                typeDeclarationSyntax.SyntaxTree,
                TextSpan.FromBounds(token.Span.Start, typeDeclarationSyntax.Identifier.Span.End));

            //// return typeDeclarationSyntax.GetLocation();
        }

        static Diagnostic CreateDiagnostic(
            TypeDeclarationSyntax typeDeclarationSyntax,
            DiagnosticDescriptor diagnosticDescriptor,
            params object?[]? extraMessageArgs)
        {
            var messageArgs = (extraMessageArgs ?? Enumerable.Empty<object?>()).Prepend(typeDeclarationSyntax.Identifier.Text).ToArray();
            return Diagnostic.Create(diagnosticDescriptor, GetTypeNodeLocation(typeDeclarationSyntax), messageArgs);
        }
    }
}