using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerExtensions.Analyzers;

public sealed partial class MemberConstraintAttributeAnalyzer
{
    private readonly struct AnalyzerContext
    {
        private AnalyzerContext(SyntaxNodeAnalysisContext diagnosticContext, CSharpCompilation compilation, SymbolsContainer symbols)
        {
            DiagnosticContext = diagnosticContext;
            CancellationToken = diagnosticContext.CancellationToken;
            Compilation = compilation;
            Symbols = symbols;
        }

        public SyntaxNodeAnalysisContext DiagnosticContext { get; }

        public CancellationToken CancellationToken { get; }

        public CSharpCompilation Compilation { get; }

        public SymbolsContainer Symbols { get; }

        public static AnalyzerContext? TryCreate(SyntaxNodeAnalysisContext diagnosticContext)
        {
            if (diagnosticContext.CancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (diagnosticContext.Compilation is not CSharpCompilation compilation)
            {
                return null;
            }

            if (compilation.GetTypeByMetadataName(Metadata.FullName.SystemType) is not { } systemTypeTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.MemberConstraintAttribute) is not { } memberConstraintAttributeTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.MemberItemConstraintAttribute) is not { } memberItemConstraintAttributeTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.IMemberConstraint) is not { } iMemberConstraintTypeSymbol
                || compilation.GetTypeByMetadataName(Metadata.FullName.TypedMemberConstraintBase) is not { } typedMemberConstraintBaseTypeSymbol)
            {
                return null;
            }

            var memberConstraintAttributeConstructorMethodSymbol = memberConstraintAttributeTypeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .SingleOrDefault(
                    symbol => symbol.MethodKind == MethodKind.Constructor && symbol.Parameters.Length == 1
                        && symbol.Parameters[0].Type.MatchesRequiredSymbol(systemTypeTypeSymbol));

            if (memberConstraintAttributeConstructorMethodSymbol is null)
            {
                return null;
            }

            var memberItemConstraintAttributeConstructorMethodSymbol = memberItemConstraintAttributeTypeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .SingleOrDefault(
                    symbol => symbol.MethodKind == MethodKind.Constructor && symbol.Parameters.Length == 1
                        && symbol.Parameters[0].Type.MatchesRequiredSymbol(systemTypeTypeSymbol));

            if (memberItemConstraintAttributeConstructorMethodSymbol is null)
            {
                return null;
            }

            var symbols = new SymbolsContainer(
                new RequiredSymbolsContainer(
                    systemType: systemTypeTypeSymbol,
                    memberConstraintAttribute: memberConstraintAttributeTypeSymbol,
                    memberConstraintAttributeConstructor: memberConstraintAttributeConstructorMethodSymbol,
                    memberItemConstraintAttribute: memberItemConstraintAttributeTypeSymbol,
                    memberItemConstraintAttributeConstructor: memberItemConstraintAttributeConstructorMethodSymbol,
                    iMemberConstraint: iMemberConstraintTypeSymbol,
                    typedMemberConstraintBase: typedMemberConstraintBaseTypeSymbol),
                new OptionalSymbolsContainer(compilation));

            return new AnalyzerContext(diagnosticContext: diagnosticContext, compilation: compilation, symbols: symbols);
        }
    }

    private readonly struct SymbolsContainer
    {
        /// <summary>
        /// </summary>
        public SymbolsContainer(RequiredSymbolsContainer required, OptionalSymbolsContainer optional)
        {
            Required = required;
            Optional = optional;
        }

        public RequiredSymbolsContainer Required { get; }

        public OptionalSymbolsContainer Optional { get; }
    }

    private readonly struct RequiredSymbolsContainer
    {
        /// <summary>
        /// </summary>
        public RequiredSymbolsContainer(
            INamedTypeSymbol systemType,
            INamedTypeSymbol memberConstraintAttribute,
            IMethodSymbol memberConstraintAttributeConstructor,
            INamedTypeSymbol memberItemConstraintAttribute,
            IMethodSymbol memberItemConstraintAttributeConstructor,
            INamedTypeSymbol iMemberConstraint,
            INamedTypeSymbol typedMemberConstraintBase)
        {
            SystemType = systemType;
            MemberConstraintAttribute = memberConstraintAttribute;
            MemberConstraintAttributeConstructor = memberConstraintAttributeConstructor;
            MemberItemConstraintAttribute = memberItemConstraintAttribute;
            MemberItemConstraintAttributeConstructor = memberItemConstraintAttributeConstructor;
            IMemberConstraint = iMemberConstraint;
            TypedMemberConstraintBase = typedMemberConstraintBase;
        }

        public INamedTypeSymbol SystemType { get; }

        public INamedTypeSymbol MemberConstraintAttribute { get; }

        public IMethodSymbol MemberConstraintAttributeConstructor { get; }

        public INamedTypeSymbol MemberItemConstraintAttribute { get; }

        public IMethodSymbol MemberItemConstraintAttributeConstructor { get; }

        public INamedTypeSymbol IMemberConstraint { get; }

        public INamedTypeSymbol TypedMemberConstraintBase { get; }
    }

    private readonly struct OptionalSymbolsContainer
    {
        /// <summary>
        /// </summary>
        public OptionalSymbolsContainer(Compilation compilation)
        {
            GenericMemberConstraintAttribute = compilation.GetTypeByMetadataName(Metadata.FullName.GenericMemberConstraintAttribute);
            GenericMemberItemConstraintAttribute = compilation.GetTypeByMetadataName(Metadata.FullName.GenericMemberItemConstraintAttribute);
            ImmutableArray = compilation.GetTypeByMetadataName(Metadata.FullName.ImmutableArray);
            NonGenericListInterface = compilation.GetTypeByMetadataName(Metadata.FullName.NonGenericListInterface);
        }

        public INamedTypeSymbol? GenericMemberConstraintAttribute { get; }

        public INamedTypeSymbol? GenericMemberItemConstraintAttribute { get; }

        public INamedTypeSymbol? ImmutableArray { get; }

        public INamedTypeSymbol? NonGenericListInterface { get; }
    }
}