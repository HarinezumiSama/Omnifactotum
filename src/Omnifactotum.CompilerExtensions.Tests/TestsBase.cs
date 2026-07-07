using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

#if !NET6_0_OR_GREATER
using System;
#endif

namespace Omnifactotum.CompilerExtensions.Tests;

[TestFixture]
internal abstract class TestsBase
{
    [SuppressMessage("ReSharper", "UseCollectionExpression", Justification = "Multiple target frameworks.")]
    protected static ReferenceAssemblies CreateReferenceAssemblies()
    {
#if NET461
        return ReferenceAssemblies.NetFramework.Net461.Default.AddPackages(
            ImmutableArray.Create(
                new PackageIdentity("System.Collections.Immutable", "7.0.0"),
                new PackageIdentity("System.Threading.Tasks.Extensions", "4.5.4"),
                new PackageIdentity("Microsoft.Bcl.AsyncInterfaces", "5.0.0")));
#elif NET472
        return ReferenceAssemblies.NetFramework.Net472.Default.AddPackages(
            ImmutableArray.Create(
                new PackageIdentity("System.Threading.Tasks.Extensions", "4.5.4"),
                new PackageIdentity("Microsoft.Bcl.AsyncInterfaces", "5.0.0")));
#elif NETCOREAPP2_1
        return ReferenceAssemblies.NetCore.NetCoreApp21;
#elif NETCOREAPP3_1
        return ReferenceAssemblies.NetCore.NetCoreApp31;
#elif NET5_0
        return ReferenceAssemblies.Net.Net50;
#elif NET6_0
        return ReferenceAssemblies.Net.Net60;
#elif NET7_0
        return ReferenceAssemblies.Net.Net70;
#elif NET8_0
        return ReferenceAssemblies.Net.Net80;
#elif NET9_0
        return ReferenceAssemblies.Net.Net90;
#elif NET10_0
        return ReferenceAssemblies.Net.Net100;
#else
#error Unexpected target .NET version (NetVersion)
#endif
    }

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

    public abstract record CodeChangeTestData : BaseTestData
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

    public sealed record CodeFixTestData : CodeChangeTestData;
}