using System;
using System.Diagnostics.CodeAnalysis;
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
        messageFormat: $"[{DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix}] The asynchronous {{0}} '{{1}}' is missing the '{
            Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.AsyncMethodMissingAsyncSuffix));

    public static readonly DiagnosticDescriptor SyncMethodHasAsyncSuffix = new(
        id: DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix,
        title: $"Synchronous method/function has '{Metadata.AsyncMethodSuffix}' suffix",
        messageFormat: $"[{DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix}] The synchronous {{0}} '{{1}}' has the misleading '{
            Metadata.AsyncMethodSuffix}' suffix",
        category: DiagnosticCategories.WellKnown.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.SyncMethodHasAsyncSuffix));

    public static readonly DiagnosticDescriptor AsyncMethodMissingCancellationTokenParameter = new(
        id: DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter,
        title: $"Asynchronous method/function lacks '{Metadata.Name.CancellationToken}' parameter",
        messageFormat: $"[{DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter}] The asynchronous {{0}} '{{1}}' is missing a '{
            Metadata.Name.CancellationToken}' parameter",
        category: DiagnosticCategories.WellKnown.Design,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.AsyncMethodMissingCancellationTokenParameter));

    public static readonly DiagnosticDescriptor ValidationConstraintTypeNotImplementsInterface = new(
        id: DiagnosticDescriptorIds.ValidationConstraintTypeNotImplementsInterface,
        title: $"Validation constraint type does not implement the required interface '{Metadata.Name.IMemberConstraint}'",
        messageFormat: $"[{DiagnosticDescriptorIds.ValidationConstraintTypeNotImplementsInterface
        }] Validation constraint type '{{0}}' does not implement the required interface '{Metadata.FullName.IMemberConstraint}'",
        category: DiagnosticCategories.WellKnown.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.ValidationConstraintTypeNotImplementsInterface));

    public static readonly DiagnosticDescriptor ValidationConstraintTypeNoParameterlessConstructor = new(
        id: DiagnosticDescriptorIds.ValidationConstraintTypeNoParameterlessConstructor,
        title: "Validation constraint type does not have a required parameterless constructor",
        messageFormat: $"[{DiagnosticDescriptorIds.ValidationConstraintTypeNoParameterlessConstructor
        }] Validation constraint type '{{0}}' does not have a required parameterless constructor",
        category: DiagnosticCategories.WellKnown.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.ValidationConstraintTypeNoParameterlessConstructor));

    public static readonly DiagnosticDescriptor GenericValidationAttributeCanBeUsed = new(
        id: DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed,
        title: "Validation attribute can be replaced with its generic equivalent",
        messageFormat: $"[{
            DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed}] Validation attribute '{{0}}' can be replaced with its generic equivalent '{{1}}'",
        category: DiagnosticCategories.WellKnown.Maintainability,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.GenericValidationAttributeCanBeUsed));

    public static readonly DiagnosticDescriptor ValidationConstraintTypeIncompatibleWithMemberType = new(
        id: DiagnosticDescriptorIds.ValidationConstraintTypeIncompatibleWithMemberType,
        title: "Validation constraint type is not compatible with the type of the validated value",
        messageFormat: $"[{DiagnosticDescriptorIds.ValidationConstraintTypeIncompatibleWithMemberType
        }] Validation constraint type '{{0}}' can only validate values of type '{{1}}' and is not compatible with the {{2}} type '{{3}}'",
        category: DiagnosticCategories.WellKnown.Correctness,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        helpLinkUri: InternalConstants.GetHelpLinkUri(DiagnosticDescriptorIds.ValidationConstraintTypeIncompatibleWithMemberType));

    /// <remarks>
    ///     Calling <see cref="DiagnosticDescriptorIds.Validate"/> here since the static constructor of <see cref="DiagnosticDescriptorIds"/> is not called
    ///     (presumably because the constants are literals and referencing them doesn't invoke type initializer).
    /// </remarks>
    static DiagnosticDescriptors() => DiagnosticDescriptorIds.Validate();

    private static class InternalConstants
    {
        private const string FallbackSourceCodeReference = "master";

        private static readonly string EncodedSourceCodeReference = HttpUtility.UrlEncode(GetSourceCodeReference());

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public static string GetHelpLinkUri(string diagnosticId)
        {
            if (string.IsNullOrEmpty(diagnosticId))
            {
                throw new ArgumentException("The diagnostic ID can be neither empty string nor null.", nameof(diagnosticId));
            }

            return $"https://github.com/HarinezumiSama/Omnifactotum/blob/{EncodedSourceCodeReference}/doc/CompilerExtensions/Rule-{diagnosticId}.md";
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