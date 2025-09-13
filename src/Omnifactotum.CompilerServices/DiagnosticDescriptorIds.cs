namespace Omnifactotum.CompilerServices;

/// <remarks>
///     ID format: <c>OFCAxxxx</c> (OmniFactotum Code Analysis)
/// </remarks>
internal static class DiagnosticDescriptorIds
{
    public const string AsyncMethodMissingAsyncSuffix = "OFCA0001";
    public const string SyncMethodHasAsyncSuffix = "OFCA0002";
}