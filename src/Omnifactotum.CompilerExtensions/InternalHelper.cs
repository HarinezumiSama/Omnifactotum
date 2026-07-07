using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerExtensions;

internal static class InternalHelper
{
    private const string NullUIString = "<null>";

    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers")]
    public static readonly string NewLine = Environment.NewLine;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static T EnsureNotNull<T>(
        this T? value,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(value))]
#endif
        string? valueExpression = null)
        where T : class
        => value ?? throw new ArgumentNullException(
            nameof(value),
            valueExpression is null ? null : $"The following expression is null: {{ {valueExpression} }}.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static string ToUIString<TEnum>(this TEnum value)
        where TEnum : struct, Enum
        => $"'{value:G}'";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static string ToUIString<TEnum>(this TEnum? value)
        where TEnum : struct, Enum
        => value is null ? NullUIString : value.Value.ToUIString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static string ToUIString(this string? value) => value is null ? NullUIString : "'" + value + "'";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerStepThrough]
    public static string ToUIString(this IEnumerable<string>? values)
        => values is null ? NullUIString : string.Join(",\x0020", values.Select(static value => value.ToUIString()));

    public static string GetDiagnosticDisplayString(this ISymbol? symbol)
        => symbol is null ? NullUIString : symbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);

    public static bool MatchesRequiredSymbol(this ISymbol? symbol, ISymbol? otherSymbol)
        => symbol is not null && otherSymbol is not null && SymbolEqualityComparer.Default.Equals(symbol, otherSymbol);

    public static INamedTypeSymbol? GetConstructedFromOrDefault(this ITypeSymbol? symbol)
        => symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol ? namedTypeSymbol.ConstructedFrom : null;

    public static SyntaxToken? GetIdentifierOrDefault(this SyntaxNode syntaxNode)
    {
        return syntaxNode.EnsureNotNull() switch
        {
            MethodDeclarationSyntax syntax => syntax.Identifier,
            LocalFunctionStatementSyntax syntax => syntax.Identifier,
            _ => null
        };
    }

    [SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
    public static bool ImplementsAnyInterface(this IMethodSymbol methodSymbol)
    {
        if (methodSymbol is null)
        {
            throw new ArgumentNullException(nameof(methodSymbol));
        }

        if (methodSymbol.MethodKind == MethodKind.ExplicitInterfaceImplementation || methodSymbol.ExplicitInterfaceImplementations.Length != 0)
        {
            return true;
        }

        var containingType = methodSymbol.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        foreach (var @interface in containingType.AllInterfaces)
        {
            foreach (var member in @interface.GetMembers())
            {
                var implementation = containingType.FindImplementationForInterfaceMember(member);
                if (methodSymbol.MatchesRequiredSymbol(implementation))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool ImplementsInterface(this ITypeSymbol typeSymbol, ITypeSymbol interfaceSymbol)
    {
        if (typeSymbol is null)
        {
            throw new ArgumentNullException(nameof(typeSymbol));
        }

        if (interfaceSymbol is null)
        {
            throw new ArgumentNullException(nameof(interfaceSymbol));
        }

        if (interfaceSymbol.TypeKind != TypeKind.Interface)
        {
            throw new ArgumentException(
                $"The specified symbol '{interfaceSymbol.GetDiagnosticDisplayString()}' is not an interface.",
                nameof(interfaceSymbol));
        }

        return typeSymbol.AllInterfaces.Any(interfaceTypeSymbol => interfaceTypeSymbol.MatchesRequiredSymbol(interfaceSymbol));
    }

    public static string? TryGetTargetFrameworkString(
        this AnalyzerConfigOptionsProvider? optionsProvider,
        SyntaxTree? syntaxTree,
        IInternalLogger logger,
        string projectDescription)
    {
        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (projectDescription is null)
        {
            throw new ArgumentNullException(nameof(projectDescription));
        }

        if (optionsProvider is null)
        {
            return null;
        }

        return InternalTryGetTargetFramework(optionsProvider.GlobalOptions, logger, projectDescription, "Global options")
            ?? (syntaxTree is null ? null : InternalTryGetTargetFramework(optionsProvider.GetOptions(syntaxTree), logger, projectDescription, "Options"));

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        static string? InternalTryGetTargetFramework(
            AnalyzerConfigOptions options,
            IInternalLogger logger,
            string projectDescription,
            string designation)
        {
            const string PropertyPrefix = "build_property.";

#if DEBUG
            var optionsKeys = options.Keys.Where(key => key.StartsWith(PropertyPrefix, StringComparison.OrdinalIgnoreCase)).ToArray();

            logger.AppendLog(
                $"{nameof(TryGetTargetFrameworkString)}: {projectDescription}. {designation}: {string.Join(
                    ",\x0020",
                    optionsKeys.Select(
                        key =>
                        {
                            options.TryGetValue(key, out var value);
                            return $"{key} = {value.ToUIString()}";
                        }))}.");
#endif

            // For SDK-style projects
            if (options.TryGetValue($"{PropertyPrefix}TargetFramework", out var targetFramework) && !string.IsNullOrWhiteSpace(targetFramework))
            {
                return targetFramework;
            }

            // Useful to detect multi-targeting (but doesn't tell you which "active" one):
            if (options.TryGetValue($"{PropertyPrefix}TargetFrameworks", out var targetFrameworks) && !string.IsNullOrWhiteSpace(targetFrameworks))
            {
                // In practice, in VS multi-targeting usually shows up as separate configured projects,
                // so TargetFramework above is typically present per configured project.
                return targetFrameworks;
            }

            // Fallbacks (older project systems / some hosts):
            if (options.TryGetValue($"{PropertyPrefix}TargetFrameworkMoniker", out var moniker) && !string.IsNullOrWhiteSpace(moniker))
            {
                return moniker;
            }

            return null;
        }
    }
}