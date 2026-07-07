using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;

namespace Omnifactotum.CompilerExtensions.CodeFixProviders;

/// <summary>
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MemberConstraintAttributeCodeFixProvider))]
[Shared]
public sealed class MemberConstraintAttributeCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed);

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await FixerContext.TryCreateAsync(context) is not { } fixerContext)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var diagnosticNode = fixerContext.RootNode.FindNode(diagnosticSpan, getInnermostNodeForTie: true);

            switch (diagnostic.Id)
            {
                case DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed:
                    RegisterGenericValidationAttributeCanBeUsedCodeFixes(fixerContext, diagnostic, diagnosticNode);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected diagnostic ID '{diagnostic.Id}'.");
            }
        }
    }

    [SuppressMessage("ReSharper", "ArgumentsStyleAnonymousFunction")]
    private static void RegisterGenericValidationAttributeCanBeUsedCodeFixes(FixerContext fixerContext, Diagnostic diagnostic, SyntaxNode diagnosticNode)
    {
        if (diagnosticNode is not AttributeSyntax attributeSyntax)
        {
            return;
        }

        var parseOptions = attributeSyntax.SyntaxTree.Options as CSharpParseOptions;
        var languageVersion = parseOptions?.LanguageVersion;
        if (languageVersion is not >= LanguageVersion.CSharp11)
        {
            return;
        }

        if (attributeSyntax.Name.DescendantNodesAndSelf().OfType<SimpleNameSyntax>().LastOrDefault() is not IdentifierNameSyntax identifierNameSyntax)
        {
            return;
        }

        if (attributeSyntax.ArgumentList?.Arguments is not { Count: 1 } arguments
            || arguments[0].Expression is not TypeOfExpressionSyntax { Type: { } originalTypeSyntax })
        {
            return;
        }

        var codeAction = CodeAction.Create(
            title: $"Fix {diagnostic.Id}: Replace with a generic attribute equivalent",
            createChangedDocument: async token =>
            {
                var editor = await DocumentEditor.CreateAsync(fixerContext.Document, token);

                var typeSyntax = originalTypeSyntax.WithAdditionalAnnotations(Simplifier.Annotation).EnsureNotNull();
                var typeArgumentList = SyntaxFactory.TypeArgumentList(SyntaxFactory.SingletonSeparatedList(typeSyntax));
                var genericNameSyntax = SyntaxFactory.GenericName(identifierNameSyntax.Identifier, typeArgumentList).WithTriviaFrom(identifierNameSyntax);
                var newNameSyntax = attributeSyntax.Name.ReplaceNode(identifierNameSyntax, genericNameSyntax);

                var newAttributeSyntax = attributeSyntax
                    .WithName(newNameSyntax)
                    .WithArgumentList(null)
                    .WithAdditionalAnnotations(Simplifier.Annotation, Formatter.Annotation);

                editor.ReplaceNode(attributeSyntax, newAttributeSyntax);
                return editor.GetChangedDocument();
            },
            equivalenceKey: CreateEquivalenceKey(diagnostic));

        fixerContext.CodeFixContext.RegisterCodeFix(codeAction, diagnostic);
    }

    private static string CreateEquivalenceKey(Diagnostic diagnostic) => $"{typeof(MemberConstraintAttributeCodeFixProvider).FullName}:{diagnostic.Id}";

    private readonly struct FixerContext
    {
        private FixerContext(CodeFixContext codeFixContext, SyntaxNode rootNode)
        {
            CodeFixContext = codeFixContext;
            RootNode = rootNode;
        }

        public CodeFixContext CodeFixContext { get; }

        public Document Document => CodeFixContext.Document;

        public SyntaxNode RootNode { get; }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        [SuppressMessage("ReSharper", "UseNullPropagation")]
        public static async Task<FixerContext?> TryCreateAsync(CodeFixContext context)
        {
            var rootNode = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (rootNode is null)
            {
                return null;
            }

            return new FixerContext(codeFixContext: context, rootNode: rootNode);
        }
    }
}