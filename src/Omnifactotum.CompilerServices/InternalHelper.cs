using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Omnifactotum.CompilerServices;

internal static class InternalHelper
{
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

    public static string GetDiagnosticDisplayString(this ISymbol symbol)
        => symbol.EnsureNotNull().ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);

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
    public static bool ImplementsInterface(this IMethodSymbol methodSymbol)
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
}