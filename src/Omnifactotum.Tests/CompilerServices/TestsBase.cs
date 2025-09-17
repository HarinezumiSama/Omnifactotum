using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace Omnifactotum.Tests.CompilerServices;

[TestFixture]
internal abstract class TestsBase
{
    protected static ReferenceAssemblies CreateReferenceAssemblies()
    {
#if NET9_0
        const string NetVersion = "9.0";
#elif NET8_0
        const string NetVersion = "8.0";
#elif NET7_0
        const string NetVersion = "7.0";
#elif NET6_0
        const string NetVersion = "6.0";
#elif NET5_0
        const string NetVersion = "5.0";
#elif NETCOREAPP3_1
        const string NetVersion = "3.1";
#else
        const string NetVersion = "<N/A>";
#error Unexpected target .NET version (NetVersion)
#endif

#if NETCOREAPP3_1
        const string TargetFramework = "netcoreapp3.1";
#elif NET5_0_OR_GREATER
        const string TargetFramework = $"net{NetVersion}";
#else
        const string TargetFramework = "<N/A>";
#error Unexpected target .NET version (TargetFramework)
#endif

        var referenceAssemblies =
            new ReferenceAssemblies(
                    TargetFramework,
                    new PackageIdentity(
                        "Microsoft.NETCore.App.Ref",
                        $"{NetVersion}.0"),
                    Path.Combine("ref", TargetFramework))
                .AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.AspNetCore.App.Ref", $"{NetVersion}.0")));

        return referenceAssemblies;
    }

    protected static string RemoveExtension(string filePath) => Path.GetFullPath(Path.ChangeExtension(filePath, null));

    public abstract record BaseTestData
    {
        private readonly string _initialSource;

        public required string InitialSource
        {
            get => _initialSource;

            [MemberNotNull(nameof(_initialSource))]
            init => _initialSource = value.ReplaceLineEndings();
        }

        public required ImmutableList<DiagnosticResult> ExpectedDiagnostics { get; init; }

        public ImmutableList<string>? DisabledDiagnosticIds { get; init; }
    }

    public sealed record AnalyzerTestData : BaseTestData;

    public sealed record CodeFixTestData : BaseTestData
    {
        private readonly string _fixedSource;

        public required int CodeActionIndex { get; init; }

        public required string FixedSource
        {
            get => _fixedSource;

            [MemberNotNull(nameof(_fixedSource))]
            init => _fixedSource = value.ReplaceLineEndings();
        }
    }
}