using System.Runtime.CompilerServices;

namespace Omnifactotum;

internal static class OmnifactotumConstants
{
    public const char SlashChar = '/';
    public const char DoubleQuoteChar = '"';
    public const char SingleQuoteChar = '\'';

    public static readonly string Slash = SlashChar.ToString();
    public static readonly string DoubleSlash = Slash + Slash;

    public static readonly string DoubleQuote = DoubleQuoteChar.ToString();
    public static readonly string DoubleDoubleQuote = DoubleQuote + DoubleQuote;

    public static readonly string SingleQuote = SingleQuoteChar.ToString();
    public static readonly string DoubleSingleQuote = SingleQuote + SingleQuote;

    internal static class MethodOptimizationOptions
    {
        internal const MethodImplOptions Standard = MethodImplOptions.AggressiveInlining;

#if NET5_0_OR_GREATER
        internal const MethodImplOptions Maximum = Standard | MethodImplOptions.AggressiveOptimization;
#else
        internal const MethodImplOptions Maximum = Standard;
#endif
    }

    internal static class ComparisonResult
    {
        internal const int LessThan = -1;
        internal const int Equal = 0;
        internal const int GreaterThan = 1;
    }
}