using System;
using System.Linq;
using System.Reflection;
using System.Web;
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
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix));

    public static readonly DiagnosticDescriptor SyncMethodHasAsyncSuffix = new(
        id: DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix,
        title: $"Synchronous method/function has '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"The synchronous {{0}} '{{1}}' has the misleading '{Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix));

    public static readonly DiagnosticDescriptor AsyncMethodMissingCancellationTokenParameter = new(
        id: DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter,
        title: $"Asynchronous method/function lacks '{Metadata.Name.CancellationToken}' parameter",
        messageFormat: $"The asynchronous {{0}} '{{1}}' is missing a '{Metadata.Name.CancellationToken}' parameter",
        category: DiagnosticCategories.WellKnown.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter));

    /// <remarks>
    ///     Calling <see cref="DiagnosticDescriptorIds.Validate"/> here since the static constructor of <see cref="DiagnosticDescriptorIds"/> is not called
    ///     (presumably because the constants are literals that don't require type initialization).
    /// </remarks>
    static DiagnosticDescriptors() => DiagnosticDescriptorIds.Validate();

    private static class InternalConstants
    {
        private const string FallbackSourceCodeReference = "master";

        private static readonly string EncodedSourceCodeReference = HttpUtility.UrlEncode(GetSourceCodeReference());

        public static string GetHelpLinkUri(string diagnosticId)
        {
            if (string.IsNullOrEmpty(diagnosticId))
            {
                throw new ArgumentException("The diagnostic ID can be neither empty string nor null.", nameof(diagnosticId));
            }

            return $"https://github.com/HarinezumiSama/Omnifactotum/blob/{EncodedSourceCodeReference}/doc/Analyzers/Rule-{diagnosticId}.md";
        }

        private static string GetSourceCodeReference() => InternalGetAssemblyMetadataValue("SourceCodeRevisionId") ?? FallbackSourceCodeReference;

        private static string? InternalGetAssemblyMetadataValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The value can be neither empty string nor null.", nameof(key));
            }

            var attribute = typeof(InternalConstants).Assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(attribute => attribute.Key == key);

            return attribute?.Value;
        }
    }
}