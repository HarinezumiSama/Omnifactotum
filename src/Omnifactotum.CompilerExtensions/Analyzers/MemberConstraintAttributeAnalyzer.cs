using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Omnifactotum.CompilerExtensions.Analyzers;

/// <summary>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
[SuppressMessage("ReSharper", "InvocationIsSkipped")]
public sealed partial class MemberConstraintAttributeAnalyzer : DiagnosticAnalyzer
{
    private static readonly InternalLogger<MemberConstraintAttributeAnalyzer> Logger = new();

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface,
        DiagnosticDescriptors.ValidationConstraintTypeNoParameterlessConstructor,
        DiagnosticDescriptors.GenericValidationAttributeCanBeUsed,
        DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType);

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

        context.RegisterSyntaxNodeAction(AnalyzeAttributeLogged, SyntaxKind.Attribute);
    }

    private static void AnalyzeAttributeLogged(SyntaxNodeAnalysisContext context)
    {
        try
        {
            AnalyzeAttribute(context);
        }
        catch (Exception ex)
        {
            Logger.AppendLog($"{nameof(AnalyzeAttribute)} failed: {ex}");
            throw;
        }
    }

    [SuppressMessage("ReSharper", "CyclomaticComplexity")]
    [SuppressMessage("ReSharper", "InvertIf")]
    private static void AnalyzeAttribute(SyntaxNodeAnalysisContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node, context.CancellationToken);

        if (context.IsGeneratedCode
            || context.Node is not AttributeSyntax attributeSyntax
            || symbolInfo is not { Symbol: { } symbol })
        {
            return;
        }

        Logger.AppendLog($"{nameof(AnalyzeAttribute)}: Assembly: '{context.Compilation.Assembly.Name}'. Attribute: '{symbol.GetDiagnosticDisplayString()}'.");

        if (AnalyzerContext.TryCreate(context) is not { } analyzerContext)
        {
            return;
        }

        var isMemberConstraintAttributeConstructor = symbol.MatchesRequiredSymbol(
            analyzerContext.Symbols.Required.MemberConstraintAttributeConstructor);

        var isMemberItemConstraintAttributeConstructor = symbol.MatchesRequiredSymbol(
            analyzerContext.Symbols.Required.MemberItemConstraintAttributeConstructor);

        var isNonGenericConstraintAttribute = isMemberConstraintAttributeConstructor || isMemberItemConstraintAttributeConstructor;

        ITypeSymbol constraintTypeSymbol;
        bool isItemConstraint;
        Location reportLocation;

        if (isNonGenericConstraintAttribute)
        {
            isItemConstraint = isMemberItemConstraintAttributeConstructor;

            var argumentSyntax = attributeSyntax.ArgumentList?.Arguments.SingleOrDefault();

            if (argumentSyntax?.Expression is not TypeOfExpressionSyntax { Type: var typeSyntax })
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(typeSyntax, context.CancellationToken) is not { Symbol: ITypeSymbol typeOfTypeSymbol })
            {
                return;
            }

            constraintTypeSymbol = typeOfTypeSymbol;
            reportLocation = typeSyntax.GetLocation();
        }
        else
        {
            var containingType = symbol.ContainingType;
            var containingTypeDefinition = containingType?.OriginalDefinition;

            if (containingTypeDefinition.MatchesRequiredSymbol(analyzerContext.Symbols.Optional.GenericMemberConstraintAttribute))
            {
                isItemConstraint = false;
            }
            else if (containingTypeDefinition.MatchesRequiredSymbol(analyzerContext.Symbols.Optional.GenericMemberItemConstraintAttribute))
            {
                isItemConstraint = true;
            }
            else
            {
                return;
            }

            if (containingType is null || containingType.TypeArguments.Length != 1)
            {
                return;
            }

            constraintTypeSymbol = containingType.TypeArguments[0];
            reportLocation = GetGenericConstraintArgumentLocation(attributeSyntax) ?? attributeSyntax.GetLocation();
        }

        var hasErrors = false;

        if (isNonGenericConstraintAttribute)
        {
            if (!constraintTypeSymbol.ImplementsInterface(analyzerContext.Symbols.Required.IMemberConstraint))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.ValidationConstraintTypeNotImplementsInterface,
                        reportLocation,
                        constraintTypeSymbol.GetDiagnosticDisplayString()));

                hasErrors = true;
            }

            var hasParameterlessConstructor = constraintTypeSymbol
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Any(static constructorSymbol => constructorSymbol.MethodKind == MethodKind.Constructor && constructorSymbol.Parameters.Length == 0);

            if (!hasParameterlessConstructor)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.ValidationConstraintTypeNoParameterlessConstructor,
                        reportLocation,
                        constraintTypeSymbol.GetDiagnosticDisplayString()));

                hasErrors = true;
            }
        }

        AnalyzeConstraintValueTypeCompatibility(in analyzerContext, attributeSyntax, constraintTypeSymbol, isItemConstraint, reportLocation);

        if (hasErrors)
        {
            return;
        }

        var parseOptions = context.FilterTree.Options as CSharpParseOptions;
        var languageVersion = parseOptions?.LanguageVersion;
        Logger.AppendLog($"{nameof(AnalyzeAttribute)}: Assembly '{context.Compilation.Assembly.Name}'. Language version {languageVersion.ToUIString()}.");

        if (languageVersion is >= LanguageVersion.CSharp11 && symbol.ContainingType is { } containingTypeSymbol)
        {
            if (isMemberConstraintAttributeConstructor
                && analyzerContext.Symbols.Optional.GenericMemberConstraintAttribute is { } genericMemberConstraintAttribute)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.GenericValidationAttributeCanBeUsed,
                        attributeSyntax.GetLocation(),
                        containingTypeSymbol.GetDiagnosticDisplayString(),
                        genericMemberConstraintAttribute.GetDiagnosticDisplayString()));
            }

            if (isMemberItemConstraintAttributeConstructor
                && analyzerContext.Symbols.Optional.GenericMemberItemConstraintAttribute is { } genericMemberItemConstraintAttribute)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.GenericValidationAttributeCanBeUsed,
                        attributeSyntax.GetLocation(),
                        containingTypeSymbol.GetDiagnosticDisplayString(),
                        genericMemberItemConstraintAttribute.GetDiagnosticDisplayString()));
            }
        }
    }

    private static void AnalyzeConstraintValueTypeCompatibility(
        in AnalyzerContext analyzerContext,
        AttributeSyntax attributeSyntax,
        ITypeSymbol constraintTypeSymbol,
        bool isItemConstraint,
        Location reportLocation)
    {
        if (GetConstraintValueType(constraintTypeSymbol, analyzerContext.Symbols.Required.TypedMemberConstraintBase) is not { } constraintValueType
            || !IsAnalyzableType(constraintValueType))
        {
            return;
        }

        if (GetAnnotatedMemberType(in analyzerContext, attributeSyntax) is not { } memberType)
        {
            return;
        }

        ITypeSymbol validatedValueType;
        string validatedValueKind;

        if (isItemConstraint)
        {
            if (GetCollectionItemType(analyzerContext, memberType) is not { } itemType)
            {
                return;
            }

            validatedValueType = itemType;
            validatedValueKind = "collection item";
        }
        else
        {
            validatedValueType = memberType;
            validatedValueKind = "member";
        }

        if (validatedValueType.TypeKind is TypeKind.Error or TypeKind.Dynamic || validatedValueType is ITypeParameterSymbol)
        {
            return;
        }

        if (!IsCompatibleWithConstraintValueType(in analyzerContext, validatedValueType, constraintValueType))
        {
            analyzerContext.DiagnosticContext.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.ValidationConstraintTypeIncompatibleWithMemberType,
                    reportLocation,
                    constraintTypeSymbol.GetDiagnosticDisplayString(),
                    constraintValueType.GetDiagnosticDisplayString(),
                    validatedValueKind,
                    validatedValueType.GetDiagnosticDisplayString()));
        }
    }

    private static ITypeSymbol? GetConstraintValueType(ITypeSymbol constraintTypeSymbol, INamedTypeSymbol typedMemberConstraintBaseDefinition)
    {
        for (var currentType = constraintTypeSymbol as INamedTypeSymbol; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType is { IsGenericType: true, TypeArguments.Length: 1 }
                && currentType.OriginalDefinition.MatchesRequiredSymbol(typedMemberConstraintBaseDefinition))
            {
                return currentType.TypeArguments[0];
            }
        }

        return null;
    }

    private static ITypeSymbol? GetAnnotatedMemberType(in AnalyzerContext analyzerContext, AttributeSyntax attributeSyntax)
    {
        if (attributeSyntax.Parent is not AttributeListSyntax attributeListSyntax)
        {
            return null;
        }

        switch (attributeListSyntax.Parent)
        {
            case PropertyDeclarationSyntax propertyDeclarationSyntax:
                return analyzerContext.DiagnosticContext.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax, analyzerContext.CancellationToken)?.Type;

            case FieldDeclarationSyntax fieldDeclarationSyntax:
                {
                    var variableDeclaratorSyntax = fieldDeclarationSyntax.Declaration.Variables.FirstOrDefault();

                    return variableDeclaratorSyntax is null
                        ? null
                        : (analyzerContext.DiagnosticContext.SemanticModel.GetDeclaredSymbol(
                            variableDeclaratorSyntax,
                            analyzerContext.CancellationToken) as IFieldSymbol)?.Type;
                }

            default:
                return null;
        }
    }

    private static ITypeSymbol? GetCollectionItemType(in AnalyzerContext analyzerContext, ITypeSymbol memberType)
    {
        if (IsSimpleType(memberType))
        {
            return null;
        }

        if (memberType is IArrayTypeSymbol { IsSZArray: true } arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType;
        }

        if (analyzerContext.Symbols.Optional.ImmutableArray is { } immutableArraySymbol
            && memberType is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedMemberType
            && namedMemberType.OriginalDefinition.MatchesRequiredSymbol(immutableArraySymbol))
        {
            return namedMemberType.TypeArguments[0];
        }

        var readOnlyListItemTypes = GetGenericInterfaceArguments(memberType, SpecialType.System_Collections_Generic_IReadOnlyList_T);
        if (readOnlyListItemTypes.Count != 0)
        {
            return readOnlyListItemTypes.Count == 1 ? readOnlyListItemTypes[0] : null;
        }

        var listItemTypes = GetGenericInterfaceArguments(memberType, SpecialType.System_Collections_Generic_IList_T);
        if (listItemTypes.Count != 0)
        {
            return listItemTypes.Count == 1 ? listItemTypes[0] : null;
        }

        if (analyzerContext.Symbols.Optional.NonGenericListInterface is { } nonGenericListInterfaceSymbol
            && ImplementsOrIs(memberType, nonGenericListInterfaceSymbol))
        {
            return analyzerContext.Compilation.GetSpecialType(SpecialType.System_Object);
        }

        var enumerableItemTypes = GetGenericInterfaceArguments(memberType, SpecialType.System_Collections_Generic_IEnumerable_T);
        if (enumerableItemTypes.Count != 0)
        {
            return enumerableItemTypes.Count == 1 ? enumerableItemTypes[0] : null;
        }

        if (memberType.SpecialType == SpecialType.System_Collections_IEnumerable
            || memberType.AllInterfaces.Any(static interfaceSymbol => interfaceSymbol.SpecialType == SpecialType.System_Collections_IEnumerable))
        {
            return analyzerContext.Compilation.GetSpecialType(SpecialType.System_Object);
        }

        return null;
    }

    private static List<ITypeSymbol> GetGenericInterfaceArguments(ITypeSymbol typeSymbol, SpecialType interfaceSpecialType)
    {
        var result = new List<ITypeSymbol>();

        foreach (var candidate in EnumerateSelfAndInterfaces(typeSymbol))
        {
            if (candidate.OriginalDefinition.SpecialType == interfaceSpecialType
                && candidate.TypeArguments.Length == 1
                && !result.Any(existingItemType => existingItemType.MatchesRequiredSymbol(candidate.TypeArguments[0])))
            {
                result.Add(candidate.TypeArguments[0]);
            }
        }

        return result;
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateSelfAndInterfaces(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            yield return namedTypeSymbol;
        }

        foreach (var interfaceSymbol in typeSymbol.AllInterfaces)
        {
            yield return interfaceSymbol;
        }
    }

    private static bool ImplementsOrIs(ITypeSymbol typeSymbol, INamedTypeSymbol otherTypeSymbol)
        => typeSymbol.MatchesRequiredSymbol(otherTypeSymbol)
            || typeSymbol.AllInterfaces.Any(interfaceSymbol => interfaceSymbol.MatchesRequiredSymbol(otherTypeSymbol));

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static bool IsCompatibleWithConstraintValueType(in AnalyzerContext analyzerContext, ITypeSymbol validatedValueType, ITypeSymbol constraintValueType)
    {
        var conversion = analyzerContext.Compilation.ClassifyConversion(validatedValueType, constraintValueType);
        if (conversion.IsIdentity || (conversion.IsImplicit && (conversion.IsReference || conversion.IsBoxing)))
        {
            return true;
        }

        if (validatedValueType.IsValueType
            && GetNullableUnderlyingType(constraintValueType) is { } underlyingConstraintValueType
            && analyzerContext.Compilation.ClassifyConversion(validatedValueType, underlyingConstraintValueType).IsIdentity)
        {
            return true;
        }

        if (ContainsTypeParameter(validatedValueType))
        {
            return CanEverBeCompatibleWithConstraintValueType(validatedValueType, constraintValueType);
        }

        return false;
    }

    /// <remarks>
    ///     Determines whether the specified validated value type (which contains free type parameters) could be compatible with the specified constraint
    ///     value type under <b>some</b> substitution of those type parameters. Returns <see langword="false"/> only when no substitution could ever yield
    ///     an identity, implicit reference, or boxing conversion (in which case the constraint is definitely misapplied).
    /// </remarks>
    private static bool CanEverBeCompatibleWithConstraintValueType(ITypeSymbol validatedValueType, ITypeSymbol constraintValueType)
    {
        if (constraintValueType.SpecialType == SpecialType.System_Object)
        {
            return true;
        }

        var constraintValueTypeDefinition = constraintValueType.OriginalDefinition;

        for (var currentType = validatedValueType; currentType is not null; currentType = currentType.BaseType)
        {
            if (currentType.OriginalDefinition.MatchesRequiredSymbol(constraintValueTypeDefinition))
            {
                return true;
            }
        }

        return validatedValueType.AllInterfaces.Any(
            interfaceSymbol => interfaceSymbol.OriginalDefinition.MatchesRequiredSymbol(constraintValueTypeDefinition));
    }

    private static bool IsAnalyzableType(ITypeSymbol typeSymbol)
        => typeSymbol.TypeKind is not (TypeKind.Error or TypeKind.Dynamic) && typeSymbol is not IErrorTypeSymbol && !ContainsTypeParameter(typeSymbol);

    private static bool ContainsTypeParameter(ITypeSymbol typeSymbol)
    {
        while (true)
        {
            switch (typeSymbol)
            {
                case ITypeParameterSymbol:
                    return true;

                case IArrayTypeSymbol arrayTypeSymbol:
                    typeSymbol = arrayTypeSymbol.ElementType;
                    continue;

                case IPointerTypeSymbol pointerTypeSymbol:
                    typeSymbol = pointerTypeSymbol.PointedAtType;
                    continue;

                case INamedTypeSymbol namedTypeSymbol:
                    if (namedTypeSymbol.IsUnboundGenericType)
                    {
                        return true;
                    }

                    foreach (var typeArgument in namedTypeSymbol.TypeArguments)
                    {
                        if (ContainsTypeParameter(typeArgument))
                        {
                            return true;
                        }
                    }

                    return false;

                default:
                    return false;
            }
        }
    }

    /// <remarks>
    ///     Mirrors <c>ObjectValidator.IsSimpleTypeInternal</c>.
    /// </remarks>
    [SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
    [SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
    private static bool IsSimpleType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind is TypeKind.Enum or TypeKind.Pointer)
        {
            return true;
        }

        if (GetNullableUnderlyingType(typeSymbol) is not null)
        {
            return true;
        }

        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_IntPtr:
            case SpecialType.System_UIntPtr:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_String:
            case SpecialType.System_DateTime:
                return true;

            default:
                return false;
        }
    }

    private static ITypeSymbol? GetNullableUnderlyingType(ITypeSymbol typeSymbol)
        => typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T, TypeArguments.Length: 1 } namedTypeSymbol
            ? namedTypeSymbol.TypeArguments[0]
            : null;

    private static Location? GetGenericConstraintArgumentLocation(AttributeSyntax attributeSyntax)
    {
        var genericNameSyntax = attributeSyntax.Name switch
        {
            GenericNameSyntax genericName => genericName,
            QualifiedNameSyntax { Right: GenericNameSyntax genericName } => genericName,
            AliasQualifiedNameSyntax { Name: GenericNameSyntax genericName } => genericName,
            _ => null
        };

        return genericNameSyntax?.TypeArgumentList.Arguments.FirstOrDefault()?.GetLocation();
    }
}