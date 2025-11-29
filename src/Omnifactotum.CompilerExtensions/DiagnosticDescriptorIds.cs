using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Omnifactotum.CompilerExtensions;

/// <remarks>
///     ID format: <c>OFCAxxxx</c> (OmniFactotum Code Analysis).
/// </remarks>
internal static class DiagnosticDescriptorIds
{
    public const string AsyncMethodMissingAsyncSuffix = "OFCA0001";
    public const string SyncMethodHasAsyncSuffix = "OFCA0002";
    public const string AsyncMethodMissingCancellationTokenParameter = "OFCA0003";

    internal static void Validate()
    {
        var fieldInfos = typeof(DiagnosticDescriptorIds)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(static info => info.IsLiteral && info.FieldType == typeof(string))
            .ToArray();

        var comparer = StringComparer.Ordinal;

        var duplicates = fieldInfos
            .Select(static info => new KeyValuePair<string, string>(info.Name, (string)info.GetValue(null).EnsureNotNull()))
            .GroupBy(static pair => pair.Value, comparer)
            .Select(
                grouping => new KeyValuePair<string, string[]>(
                    grouping.Key,
                    grouping.Select(static pair => pair.Key).OrderBy(static s => s, comparer).ToArray()))
            .Where(static item => item.Value.Length > 1)
            .OrderBy(static pair => pair.Key, comparer)
            .ToArray();

        if (duplicates.Length == 0)
        {
            return;
        }

        var duplicatesString = JoinItems(duplicates.Select(static pair => $"{pair.Key} ({JoinItems(pair.Value)})"));
        throw new InvalidOperationException($"Duplicate rule IDs are defined in '{typeof(DiagnosticDescriptorIds).FullName}': {duplicatesString}.");

        static string JoinItems(IEnumerable<string> items) => string.Join(",\x0020", items);
    }
}