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
    public static bool IsAnnotatedWithGeneratedToStringAttribute(this TypeDeclarationSyntax typeSyntax, SemanticModel semanticModel)
    {
        if (typeSyntax is null)
        {
            throw new ArgumentNullException(nameof(typeSyntax));
        }

        if (semanticModel is null)
        {
            throw new ArgumentNullException(nameof(semanticModel));
        }

        var typeSymbol = semanticModel.GetDeclaredSymbol(typeSyntax);

        return typeSymbol is not null
            && typeSymbol.GetAttributes().Any(attributeData => attributeData.AttributeClass?.ToString() == Constants.GeneratedToString.Attribute.FullName);
    }

    public static bool IsAnnotatedWithGeneratedToStringAttribute(this TypeDeclarationSyntax typeSyntax, Compilation compilation)
    {
        if (typeSyntax is null)
        {
            throw new ArgumentNullException(nameof(typeSyntax));
        }

        if (compilation is null)
        {
            throw new ArgumentNullException(nameof(compilation));
        }

        var semanticModel = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
        return typeSyntax.IsAnnotatedWithGeneratedToStringAttribute(semanticModel);
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    public static bool ReportGeneratedToStringAnalysisErrors(
        this TypeDeclarationSyntax typeSyntax,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        if (typeSyntax is null)
        {
            throw new ArgumentNullException(nameof(typeSyntax));
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
                    typeSyntax,
                    DiagnosticDescriptors.GeneratedToString.IncompatibleCSharpVersion,
                    effectiveLanguageVersion.ToDisplayString(),
                    MinimumLanguageVersion.ToDisplayString()));

            return true;
        }

        if (!typeSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
            || typeSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
            || typeSyntax.Modifiers.Any(SyntaxKind.FileKeyword))
        {
            reportDiagnostic(CreateDiagnostic(typeSyntax, DiagnosticDescriptors.GeneratedToString.InvalidModifiers));
            return true;
        }

        var alreadyHasToStringMethod = typeSyntax.Members.Any(
            syntax => syntax is MethodDeclarationSyntax
            {
                Identifier.Text: nameof(ToString),
                ParameterList.Parameters.Count: 0,
                Arity: 0
            });

        if (alreadyHasToStringMethod)
        {
            reportDiagnostic(CreateDiagnostic(typeSyntax, DiagnosticDescriptors.GeneratedToString.ToStringMethodAlreadyDefined));
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