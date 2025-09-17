using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerServices.Analyzers;

/// <summary>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InvocationIsSkipped")]
public sealed class AsyncMethodAnalyzer : DiagnosticAnalyzer
{
    private static readonly InternalLogger<AsyncMethodAnalyzer> Logger = new();

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix,
        DiagnosticDescriptors.SyncMethodHasAsyncSuffix);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        Logger.AppendLog($"{nameof(Initialize)}");

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        if (!Debugger.IsAttached)
        {
            context.EnableConcurrentExecution();
            Logger.AppendLog($"{nameof(Initialize)}: {nameof(context.EnableConcurrentExecution)}()");
        }

        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationLogged, SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement);
    }

    private static void AnalyzeMethodDeclarationLogged(SyntaxNodeAnalysisContext context)
    {
        try
        {
            AnalyzeMethodDeclaration(context);
        }
        catch (Exception ex)
        {
            Logger.AppendLog($"{nameof(AnalyzeMethodDeclaration)} failed: {ex}");
            throw;
        }
    }

    [SuppressMessage("ReSharper", "InvertIf")]
    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, context.CancellationToken);

        Logger.AppendLog(
            $"{nameof(AnalyzeMethodDeclaration)}: Node type: '{context.Node.GetType().Name}'. Declared symbol type: '{declaredSymbol?.GetType().Name}'.");

        if (context.IsGeneratedCode
            || context.Node is not CSharpSyntaxNode syntaxNode
            || declaredSymbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        Logger.AppendLog($"{nameof(AnalyzeMethodDeclaration)}: method/function '{methodSymbol.GetDiagnosticDisplayString()}'");

        if (AnalyzerContext.TryCreate(context) is not { } analyzerContext)
        {
            return;
        }

        if (methodSymbol.OverriddenMethod is not null || methodSymbol.ImplementsInterface())
        {
            return;
        }

        var isAsyncMethod = methodSymbol.ReturnType.MatchesRequiredSymbol(analyzerContext.VoidTaskTypeSymbol)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.ResultTaskTypeSymbol)
            || methodSymbol.ReturnType.MatchesRequiredSymbol(analyzerContext.VoidValueTaskTypeSymbol)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.ResultValueTaskTypeSymbol)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.AsyncEnumerableTypeSymbol);

        var hasAsyncSuffix = methodSymbol.Name.EndsWith(Metadata.AsyncMethodSuffix, StringComparison.Ordinal);

        if (isAsyncMethod)
        {
            if (!hasAsyncSuffix)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix,
                    GetLocation(syntaxNode),
                    CreateProperties(methodSymbol),
                    GetDesignation(methodSymbol),
                    methodSymbol.GetDiagnosticDisplayString());

                context.ReportDiagnostic(diagnostic);
            }
        }
        else
        {
            if (hasAsyncSuffix)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.SyncMethodHasAsyncSuffix,
                    GetLocation(syntaxNode),
                    CreateProperties(methodSymbol),
                    GetDesignation(methodSymbol),
                    methodSymbol.GetDiagnosticDisplayString());

                context.ReportDiagnostic(diagnostic);
            }
        }

        static ImmutableDictionary<string, string?> CreateProperties(ISymbol symbol)
        {
            var methodName = symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);

            return ImmutableDictionary.CreateRange<string, string?>(
            [
                new KeyValuePair<string, string?>(DiagnosticPropertyNames.Name, symbol.Name),
                new KeyValuePair<string, string?>(DiagnosticPropertyNames.DisplayName, methodName)
            ]);
        }

        static string GetDesignation(IMethodSymbol symbol)
            => symbol.MethodKind switch
            {
                MethodKind.Ordinary or MethodKind.ExplicitInterfaceImplementation => "method",
                MethodKind.LocalFunction => "local function",
                _ => $"<{symbol.MethodKind}>"
            };

        static Location GetLocation(CSharpSyntaxNode syntaxNode) => syntaxNode.GetIdentifierOrDefault()?.GetLocation() ?? syntaxNode.GetLocation();
    }

    private readonly struct AnalyzerContext
    {
        private AnalyzerContext(
            SyntaxNodeAnalysisContext diagnosticContext,
            INamedTypeSymbol voidTaskTypeSymbol,
            INamedTypeSymbol resultTaskTypeSymbol)
        {
            DiagnosticContext = diagnosticContext;

            VoidTaskTypeSymbol = voidTaskTypeSymbol;
            ResultTaskTypeSymbol = resultTaskTypeSymbol;

            VoidValueTaskTypeSymbol = diagnosticContext.Compilation.GetTypeByMetadataName(Metadata.FullName.VoidValueTask);
            ResultValueTaskTypeSymbol = diagnosticContext.Compilation.GetTypeByMetadataName(Metadata.FullName.ResultValueTask);

            AsyncEnumerableTypeSymbol = diagnosticContext.Compilation.GetTypeByMetadataName(Metadata.FullName.AsyncEnumerable);
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        public SyntaxNodeAnalysisContext DiagnosticContext { get; }

        // public CancellationToken CancellationToken => DiagnosticContext.CancellationToken;

        public INamedTypeSymbol VoidTaskTypeSymbol { get; }

        public INamedTypeSymbol ResultTaskTypeSymbol { get; }

        public INamedTypeSymbol? VoidValueTaskTypeSymbol { get; }

        public INamedTypeSymbol? ResultValueTaskTypeSymbol { get; }

        public INamedTypeSymbol? AsyncEnumerableTypeSymbol { get; }

        public static AnalyzerContext? TryCreate(SyntaxNodeAnalysisContext diagnosticContext)
        {
            if (diagnosticContext.CancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var compilation = diagnosticContext.Compilation;

            return compilation.GetTypeByMetadataName(Metadata.FullName.VoidTask) is { } voidTaskTypeSymbol
                && compilation.GetTypeByMetadataName(Metadata.FullName.ResultTask) is { } resultTaskTypeSymbol
                    ? new AnalyzerContext(
                        diagnosticContext,
                        voidTaskTypeSymbol,
                        resultTaskTypeSymbol)
                    : null;
        }

        // public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
    }
}