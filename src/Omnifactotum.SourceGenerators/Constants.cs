using System;

namespace Omnifactotum.SourceGenerators;

internal static class Constants
{
    public const string GeneratedFileExtension = ".g.cs";

    internal static class GeneratedToString
    {
        internal static class Attribute
        {
            public const string Name = "GeneratedToStringAttribute";
            public const string Namespace = "Omnifactotum.Annotations";

            public static readonly string FullName = $"{Namespace}{Type.Delimiter}{Name}";

            internal static class PropertyNames
            {
                public const string IncludeTypeName = nameof(IncludeTypeName);
            }
        }
    }
}