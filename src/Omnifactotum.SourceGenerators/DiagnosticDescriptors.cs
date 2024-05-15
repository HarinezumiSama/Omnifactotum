using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Omnifactotum.SourceGenerators;

[SuppressMessage("ReSharper", "ArgumentsStyleOther")]
[SuppressMessage("ReSharper", "ArgumentsStyleStringLiteral")]
[SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
[SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
internal static class DiagnosticDescriptors
{
    public static class GeneratedToString
    {
        private const string Category = nameof(GeneratedToStringSourceGenerator);

#pragma warning disable RS2008 // TODO: Enable Release tracking analyzer

        public static readonly DiagnosticDescriptor IncompatibleCSharpVersion = new(
            id: "OFSG0001",
            title: "Incompatible C# version",
            messageFormat: $"The type '{{0}}' is annotated with '{
                Constants.GeneratedToString.Attribute.Name}' but compiled using an incompatible C# version {{1}} (must be {{2}} or higher)",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor InvalidModifiers = new(
            id: "OFSG0002",
            title: "The type must have compatible modifiers",
            messageFormat: $"The type '{{0}}' must be partial, not static, and not file local when annotated with '{
                Constants.GeneratedToString.Attribute.Name}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ToStringMethodAlreadyDefined = new(
            id: "OFSG0003",
            title: $"{nameof(ToString)}() is already defined",
            messageFormat: $"The type '{{0}}' cannot define its own '{nameof(ToString)}()' method when annotated with '{
                Constants.GeneratedToString.Attribute.Name}'",
            category: Category,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008
    }
}