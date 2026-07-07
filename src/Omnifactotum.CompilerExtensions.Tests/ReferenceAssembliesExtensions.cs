using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.Testing;

namespace Omnifactotum.CompilerExtensions.Tests;

internal static class ReferenceAssembliesExtensions
{
    [SuppressMessage("ReSharper", "UseCollectionExpression", Justification = "Multiple target frameworks.")]
    public static ReferenceAssemblies AddRuntimeAssemblies(this ReferenceAssemblies referenceAssemblies, params Assembly[] assemblies)
    {
        if (referenceAssemblies is null)
        {
            throw new ArgumentNullException(nameof(referenceAssemblies));
        }

        if (assemblies is null)
        {
            throw new ArgumentNullException(nameof(assemblies));
        }

        var normalizedAssemblyPaths = assemblies.Select(static assembly => RemoveExtension(assembly.Location.EnsureNotNull())).ToImmutableArray();
        return referenceAssemblies.AddAssemblies(normalizedAssemblyPaths);
    }

    private static string RemoveExtension(string filePath) => Path.GetFullPath(Path.ChangeExtension(filePath, null));
}