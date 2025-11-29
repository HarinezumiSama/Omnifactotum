using Microsoft.CodeAnalysis;

namespace Omnifactotum.CompilerExtensions;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor AsyncMethodMissingAsyncSuffix = new(
        id: DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix,
        title: $"Asynchronous method/function lacks '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"The asynchronous {{0}} '{{1}}' is missing the '{Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{BaseHelpLinkUri}{DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix}.md");

    public static readonly DiagnosticDescriptor SyncMethodHasAsyncSuffix = new(
        id: DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix,
        title: $"Synchronous method/function has '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"The synchronous {{0}} '{{1}}' has the misleading '{Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{BaseHelpLinkUri}{DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix}.md");

    public static readonly DiagnosticDescriptor AsyncMethodMissingCancellationTokenParameter = new(
        id: DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter,
        title: $"Asynchronous method/function lacks '{Metadata.Name.CancellationToken}' parameter",
        messageFormat: $"The asynchronous {{0}} '{{1}}' is missing a '{Metadata.Name.CancellationToken}' parameter",
        category: DiagnosticCategories.WellKnown.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: $"{BaseHelpLinkUri}{DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter}.md");

    ////private const string BaseHelpLinkUri = "https://github.com/HarinezumiSama/Omnifactotum/blob/master/doc/Analyzers/Rule-"; // ❗TEMP
    private const string BaseHelpLinkUri = "https://github.com/HarinezumiSama/Omnifactotum/blob/develop/doc/Analyzers/Rule-"; // ❗TEMP

    /// <remarks>
    ///     Calling <see cref="DiagnosticDescriptorIds.Validate"/> here since the static constructor of <see cref="DiagnosticDescriptorIds"/> is not called
    ///     (presumably because the constants are literals that don't require type initialization).
    /// </remarks>
    static DiagnosticDescriptors() => DiagnosticDescriptorIds.Validate();
}