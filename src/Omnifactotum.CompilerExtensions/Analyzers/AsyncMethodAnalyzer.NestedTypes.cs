using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerExtensions.Analyzers;

public sealed partial class AsyncMethodAnalyzer
{
    private readonly struct AnalyzerContext
    {
        private AnalyzerContext(SyntaxNodeAnalysisContext diagnosticContext, TypeSymbolsContainer typeSymbols)
        {
            DiagnosticContext = diagnosticContext;
            TypeSymbols = typeSymbols;
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        public SyntaxNodeAnalysisContext DiagnosticContext { get; }

        // public CancellationToken CancellationToken => DiagnosticContext.CancellationToken;

        public TypeSymbolsContainer TypeSymbols { get; }

        public static AnalyzerContext? TryCreate(SyntaxNodeAnalysisContext diagnosticContext)
        {
            if (diagnosticContext.CancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var compilation = diagnosticContext.Compilation;

            if (compilation.GetTypeByMetadataName(Metadata.FullName.CancellationToken) is not { } cancellationTokenTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.VoidTask) is not { } voidTaskTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.ResultTask) is not { } resultTaskTypeSymbol)
            {
                return null;
            }

            var typeSymbols = new TypeSymbolsContainer(
                new RequiredTypeSymbolsContainer(
                    cancellationToken: cancellationTokenTypeSymbol,
                    voidTask: voidTaskTypeSymbol,
                    resultTask: resultTaskTypeSymbol),
                new OptionalTypeSymbolsContainer(compilation));

            return new AnalyzerContext(diagnosticContext: diagnosticContext, typeSymbols: typeSymbols);
        }

        // public void ThrowIfCancellationRequested() => CancellationToken.ThrowIfCancellationRequested();
    }

    private readonly struct TypeSymbolsContainer
    {
        /// <summary>
        /// </summary>
        public TypeSymbolsContainer(RequiredTypeSymbolsContainer required, OptionalTypeSymbolsContainer optional)
        {
            Required = required;
            Optional = optional;
        }

        public RequiredTypeSymbolsContainer Required { get; }

        public OptionalTypeSymbolsContainer Optional { get; }
    }

    private readonly struct RequiredTypeSymbolsContainer
    {
        /// <summary>
        /// </summary>
        public RequiredTypeSymbolsContainer(
            INamedTypeSymbol cancellationToken,
            INamedTypeSymbol voidTask,
            INamedTypeSymbol resultTask)
        {
            CancellationToken = cancellationToken;
            VoidTask = voidTask;
            ResultTask = resultTask;
        }

        public INamedTypeSymbol CancellationToken { get; }

        public INamedTypeSymbol VoidTask { get; }

        public INamedTypeSymbol ResultTask { get; }
    }

    private readonly struct OptionalTypeSymbolsContainer
    {
        /// <summary>
        /// </summary>
        public OptionalTypeSymbolsContainer(Compilation compilation)
        {
            VoidValueTask = compilation.GetTypeByMetadataName(Metadata.FullName.VoidValueTask);
            ResultValueTask = compilation.GetTypeByMetadataName(Metadata.FullName.ResultValueTask);
            AsyncEnumerable = compilation.GetTypeByMetadataName(Metadata.FullName.AsyncEnumerable);
        }

        public INamedTypeSymbol? VoidValueTask { get; }

        public INamedTypeSymbol? ResultValueTask { get; }

        public INamedTypeSymbol? AsyncEnumerable { get; }
    }
}