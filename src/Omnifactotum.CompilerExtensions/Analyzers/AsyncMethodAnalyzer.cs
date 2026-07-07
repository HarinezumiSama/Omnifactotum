using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerExtensions.Analyzers;

/// <summary>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InvocationIsSkipped")]
public sealed partial class AsyncMethodAnalyzer : DiagnosticAnalyzer
{
    private static readonly InternalLogger<AsyncMethodAnalyzer> Logger = new();

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        DiagnosticDescriptors.AsyncMethodMissingAsyncSuffix,
        DiagnosticDescriptors.SyncMethodHasAsyncSuffix,
        DiagnosticDescriptors.AsyncMethodMissingCancellationTokenParameter);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        Logger.AppendLog($"{nameof(Initialize)}()");

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
            $"{nameof(AnalyzeMethodDeclaration)}: Assembly: '{context.Compilation.Assembly.Name}'. Node type: '{
                context.Node.GetType().Name}'. Declared symbol type: '{declaredSymbol?.GetType().Name}'.");

        if (context.IsGeneratedCode
            || context.Node is not CSharpSyntaxNode syntaxNode
            || declaredSymbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        Logger.AppendLog(
            $"{nameof(AnalyzeMethodDeclaration)}: Assembly: '{context.Compilation.Assembly.Name}'. Method/function: '{
                methodSymbol.GetDiagnosticDisplayString()}'.");

        if (AnalyzerContext.TryCreate(context) is not { } analyzerContext)
        {
            return;
        }

        if (methodSymbol.OverriddenMethod is not null || methodSymbol.ImplementsAnyInterface())
        {
            return;
        }

        var isAsyncMethod = methodSymbol.ReturnType.MatchesRequiredSymbol(analyzerContext.TypeSymbols.Required.VoidTask)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.TypeSymbols.Required.ResultTask)
            || methodSymbol.ReturnType.MatchesRequiredSymbol(analyzerContext.TypeSymbols.Optional.VoidValueTask)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.TypeSymbols.Optional.ResultValueTask)
            || methodSymbol.ReturnType.GetConstructedFromOrDefault().MatchesRequiredSymbol(analyzerContext.TypeSymbols.Optional.AsyncEnumerable);

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

            var cancellationTokenParameterSymbols = methodSymbol.Parameters
                .Where(symbol => symbol.Type.MatchesRequiredSymbol(analyzerContext.TypeSymbols.Required.CancellationToken))
                .ToArray();

            if (cancellationTokenParameterSymbols.Length == 0)
            {
                var diagnostic = Diagnostic.Create(
                    DiagnosticDescriptors.AsyncMethodMissingCancellationTokenParameter,
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
}