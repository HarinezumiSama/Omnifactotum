using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Omnifactotum.SourceGenerators;

#if DEBUG
/// <summary>
///     TODO
/// </summary>
#endif
[Generator(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "ArgumentsStyleAnonymousFunction")]
[SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
public sealed partial class GeneratedToStringSourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
    {
        initializationContext.RegisterPostInitializationOutput(
            context => context.AddSource(
                $"{Constants.GeneratedToString.Attribute.FullName}{Constants.GeneratedFileExtension}",
                SourceText.From(AttributeSourceCode, Encoding.UTF8)));

        var provider = initializationContext.SyntaxProvider
            .ForAttributeWithMetadataName(
                Constants.GeneratedToString.Attribute.FullName,
                predicate: static (node, _) => node is TypeDeclarationSyntax,
                transform: CreateFoundTypeData)
            .Where(data => data is not null)
            .Select((data, _) => data!)
            .Collect();

        initializationContext.RegisterSourceOutput(
            initializationContext.CompilationProvider.Combine(provider),
            (context, t) => GenerateCode(context, t.Left, t.Right));
    }

    private static FoundTypeData? CreateFoundTypeData(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        if (context.Attributes is not { Length: 1 } attributeDatas || attributeDatas[0] is not { } attributeData)
        {
            return null;
        }

        var typeSyntax = (TypeDeclarationSyntax)context.TargetNode;

        return new FoundTypeData(typeSyntax, typeSymbol, attributeData);
    }

    private static void GenerateCode(SourceProductionContext context, Compilation compilation, ImmutableArray<FoundTypeData> typeDatas)
    {
        foreach (var typeData in typeDatas)
        {
            var typeSyntax = typeData.TypeSyntax;
            var typeSymbol = typeData.TypeSymbol;
            var attributeData = typeData.AttributeData;

            var semanticModel = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
            var typeInfo = semanticModel.GetTypeInfo(typeSyntax);
            Trace.Assert(typeInfo.GetType() is not null); // TEMP: Just using the variable

            if (typeSyntax.ReportGeneratedToStringAnalysisErrors(compilation, context.ReportDiagnostic))
            {
                continue;
            }

            var attributeNamedArguments = attributeData.NamedArguments.ToImmutableDictionary(StringComparer.Ordinal);

            var includeTypeName =
                attributeNamedArguments.TryGetValue(Constants.GeneratedToString.Attribute.PropertyNames.IncludeTypeName, out var includeTypeNameValue)
                && (bool)includeTypeNameValue.Value!;

            var @namespace = typeSymbol.ContainingNamespace?.ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)) ?? string.Empty;

            var hasNamespace = !string.IsNullOrEmpty(@namespace);
            var typeName = typeSyntax.Identifier.Text;

            // var namespacesAndTypes = semanticModel.LookupNamespacesAndTypes(
            //     typeSyntax.Identifier.SpanStart,
            //     null,
            //     typeSyntax.Identifier.Text);
            //
            // var sourceName = namespacesAndTypes is { Length: 1 }
            //     ? namespacesAndTypes[0].ToDisplayString()
            //     : typeSyntax.Identifier.Text;

            using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
            using var writer = new IndentedTextWriter(stringWriter);

            if (hasNamespace)
            {
                writer.WriteLine($"namespace {@namespace}");
                writer.WriteLine("{");
                writer.Indent++;
            }

            writer.WriteLine($"{typeSyntax.Modifiers.ToString()} {typeSyntax.Keyword.ToString()} {typeName}{typeSyntax.TypeParameterList?.ToString()}");

            writer.WriteLine("{");
            writer.Indent++;

            // ***

            writer.WriteLine("public override string ToString()");

            writer.Indent++;

            writer.Write("=> $\"");
            if (includeTypeName)
            {
                writer.Write($"{{nameof({typeName})}}:\x0020");
            }

            var propertySyntaxes = typeSyntax.Members
                .OfType<PropertyDeclarationSyntax>()
                .Where(syntax => !syntax.Modifiers.Any(SyntaxKind.StaticKeyword))
                .ToArray();

            var parts = propertySyntaxes
                .Select(syntax => syntax.Identifier.Text)
                .Select(name => $"{{nameof({name})}} = {{{name}}}");

            writer.Write(string.Join(",\x0020", parts));

            //// // TODO
            //// writer.WriteLine("=> throw new System.NotImplementedException();");

            writer.WriteLine("\";");

            writer.Indent--;

            // ***

            writer.Indent--;
            writer.WriteLine("}");

            if (hasNamespace)
            {
                writer.Indent--;
                writer.WriteLine("}");
            }

            var sourceBaseName = string.Join(
                Type.Delimiter.ToString(),
                new[] { @namespace, typeName }.Where(s => !string.IsNullOrEmpty(s)));

            var sourceName = $"{sourceBaseName}{Constants.GeneratedFileExtension}";

            context.AddSource(
                sourceName,
                SourceText.From(stringWriter.ToString(), Encoding.UTF8));
        }
    }

    private sealed class FoundTypeData
    {
        public FoundTypeData(TypeDeclarationSyntax typeSyntax, INamedTypeSymbol typeSymbol, AttributeData attributeData)
        {
            TypeSyntax = typeSyntax ?? throw new ArgumentNullException(nameof(typeSyntax));
            TypeSymbol = typeSymbol ?? throw new ArgumentNullException(nameof(typeSymbol));
            AttributeData = attributeData ?? throw new ArgumentNullException(nameof(attributeData));
        }

        public TypeDeclarationSyntax TypeSyntax { get; }

        public INamedTypeSymbol TypeSymbol { get; }

        public AttributeData AttributeData { get; }
    }
}