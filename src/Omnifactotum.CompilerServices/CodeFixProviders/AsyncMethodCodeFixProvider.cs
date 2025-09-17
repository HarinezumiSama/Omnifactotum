using System;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Rename;

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
        const string DiagnosticId = DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix;

        if (diagnosticNode is not { } syntaxNode)
        {
            return;
        }

        var name = diagnostic.Properties.GetValueOrDefault(DiagnosticPropertyNames.Name).EnsureNotNull();
        var newName = name + Metadata.AsyncMethodSuffix;

        var codeAction = CodeAction.Create(
            title: $"Fix {DiagnosticId}: Rename '{name}' to '{newName}'",
            createChangedSolution: token => RenameSymbolAsync(fixerContext, syntaxNode, newName, token),
            equivalenceKey: CreateEquivalenceKey());

        fixerContext.CodeFixContext.RegisterCodeFix(codeAction, diagnostic);

        static string CreateEquivalenceKey() => $"{typeof(AsyncMethodCodeFixProvider).FullName}:{DiagnosticId}";
    }

    [SuppressMessage("ReSharper", "ArgumentsStyleAnonymousFunction")]
    private static void RegisterSyncMethodHasAsyncSuffixCodeFixes(FixerContext fixerContext, Diagnostic diagnostic, SyntaxNode diagnosticNode)
    {
        const string DiagnosticId = DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix;

        if (diagnosticNode is not { } syntaxNode)
        {
            return;
        }

        var name = diagnostic.Properties.GetValueOrDefault(DiagnosticPropertyNames.Name).EnsureNotNull();
        var newName = RemoveSuffix(name);

        var codeAction = CodeAction.Create(
            title: $"Fix {DiagnosticId}: Rename '{name}' to '{newName}'",
            createChangedSolution: token => RenameSymbolAsync(fixerContext, syntaxNode, newName, token),
            equivalenceKey: CreateEquivalenceKey());

        fixerContext.CodeFixContext.RegisterCodeFix(codeAction, diagnostic);

        static string CreateEquivalenceKey() => $"{typeof(AsyncMethodCodeFixProvider).FullName}:{DiagnosticId}";

        static string RemoveSuffix(string value)
        {
            if (!value.EndsWith(Metadata.AsyncMethodSuffix, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    $"Cannot fix {DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix}: '{value}' does not end with '{Metadata.AsyncMethodSuffix}'.");
            }

            return value.Substring(0, value.Length - Metadata.AsyncMethodSuffix.Length);
        }
    }

    private static async Task<Solution> RenameSymbolAsync(FixerContext context, SyntaxNode targetNode, string newName, CancellationToken cancellationToken)
    {
        var document = context.Document;
        var semanticModel = (await document.GetSemanticModelAsync(cancellationToken)).EnsureNotNull();
        var symbol = semanticModel.GetDeclaredSymbol(targetNode, cancellationToken).EnsureNotNull();

        return await Renamer.RenameSymbolAsync(document.Project.Solution, symbol, new SymbolRenameOptions(), newName, cancellationToken);
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