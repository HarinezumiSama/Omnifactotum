using Microsoft.CodeAnalysis;

namespace Omnifactotum.CompilerServices;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor AsyncMethodMissingAsyncSuffix = new(
        id: DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix,
        title: $"Asynchronous method/function lacks '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"The asynchronous {{0}} '{{1}}' is missing the '{Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: null);

    public static readonly DiagnosticDescriptor SyncMethodHasAsyncSuffix = new(
        id: DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix,
        title: $"Synchronous method/function has '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"The synchronous {{0}} '{{1}}' has the misleading '{Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: null);
}