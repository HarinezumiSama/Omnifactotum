namespace Omnifactotum
{
    internal static class OmnifactotumConstants
    {
        public const char SlashChar = '/';
        public const char DoubleQuoteChar = '"';

        public static readonly string Slash = SlashChar.ToString();
        public static readonly string DoubleSlash = Slash + Slash;

        public static readonly string DoubleQuote = DoubleQuoteChar.ToString();
        public static readonly string DoubleDoubleQuote = DoubleQuote + DoubleQuote;
    }
}