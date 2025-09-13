using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Omnifactotum.CompilerServices.CodeFixProviders;

/// <summary>
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AsyncMethodCodeFixProvider))]
[Shared]
public sealed class AsyncMethodCodeFixProvider : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix,
        DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix);

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
            var diagnosticNode = fixerContext.RootNode.FindNode(diagnosticSpan);

            switch (diagnostic.Id)
            {
                case DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix:
                    RegisterAsyncMethodMissingAsyncSuffixCodeFixes(fixerContext, diagnostic, diagnosticNode);
                    break;

                case DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix:
                    RegisterSyncMethodHasAsyncSuffixCodeFixes(fixerContext, diagnostic, diagnosticNode);
                    break;

                default:
                    throw new InvalidOperationException($"Unexpected diagnostic ID '{diagnostic.Id}'.");
            }
        }
    }

    [SuppressMessage("ReSharper", "ArgumentsStyleAnonymousFunction")]
    private static void RegisterAsyncMethodMissingAsyncSuffixCodeFixes(FixerContext fixerContext, Diagnostic diagnostic, SyntaxNode diagnosticNode)
    {
        const string Prefix = $"Fix {DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix}:\x0020";

        if (diagnosticNode is not { } syntaxNode)
        {
            return;
        }

        var fullMethodName = diagnostic.Properties.GetValueOrDefault(DiagnosticPropertyNames.MethodName).EnsureNotNull();

        var codeAction = CodeAction.Create(
            title: $"{Prefix}Append '{Metadata.AsyncMethodSuffix}' suffix to method '{fullMethodName}'",
            createChangedDocument: token => CreateChangedDocument(fixerContext, syntaxNode, token),
            equivalenceKey: CreateEquivalenceKey());

        fixerContext.CodeFixContext.RegisterCodeFix(codeAction, diagnostic);

        static string CreateEquivalenceKey() => $"{typeof(AsyncMethodCodeFixProvider).FullName}:{DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix}";

        static async Task<Document> CreateChangedDocument(FixerContext fixerContext, SyntaxNode targetNode, CancellationToken cancellationToken)
        {
            await Task.Delay(0, cancellationToken);

            var newRoot = fixerContext.RootNode;

            SyntaxNode newSyntaxNode = targetNode switch
            {
                MethodDeclarationSyntax syntax => syntax.WithIdentifier(SyntaxFactory.Identifier(syntax.Identifier.Text + Metadata.AsyncMethodSuffix)),
                LocalFunctionStatementSyntax syntax => syntax.WithIdentifier(SyntaxFactory.Identifier(syntax.Identifier.Text + Metadata.AsyncMethodSuffix)),
                _ => throw new NotImplementedException($"Unexpected syntax node type '{targetNode.GetType().FullName}'.")
            };

            newRoot = newRoot.ReplaceNode(targetNode, newSyntaxNode);

            return fixerContext.Document.WithSyntaxRoot(newRoot);
        }
    }

    private static void RegisterSyncMethodHasAsyncSuffixCodeFixes(FixerContext fixerContext, Diagnostic diagnostic, SyntaxNode diagnosticNode)
    {
        // throw new NotImplementedException();
    }

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

            // var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            // if (semanticModel is null)
            // {
            //     return null;
            // }

            return new FixerContext(codeFixContext: context, rootNode: rootNode);
        }
    }
}