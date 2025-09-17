#if !NET6_0_OR_GREATER
using System;
using System.Text.RegularExpressions;

//// ReSharper disable once CheckNamespace :: Compatibility (.NET 6+)
namespace Omnifactotum.Tests;

internal static class StringExtensions
{
    private static readonly Regex LineEndingRegex = new(@"\u000d\u000a|\u000d|\u000a", RegexOptions.Singleline | RegexOptions.Compiled);

    public static string ReplaceLineEndings(this string value)
        => value is null ? throw new ArgumentNullException(nameof(value)) : LineEndingRegex.Replace(value, Environment.NewLine);
}
#endif