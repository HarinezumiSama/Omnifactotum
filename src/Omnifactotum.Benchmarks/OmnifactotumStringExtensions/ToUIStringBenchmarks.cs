using System;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Omnifactotum.Benchmarks.OmnifactotumStringExtensions;

[SimpleJob(RuntimeMoniker.NetCoreApp31)]
// [SimpleJob(RuntimeMoniker.Net50)]
// [SimpleJob(RuntimeMoniker.Net60)]
// [SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[ArtifactsPath($"""..\..\..\..\..\benchmarks\{nameof(ToUIStringBenchmarks)}""")]
[IterationCount(10)]
[WarmupCount(3)]
[ProcessCount(1)]
[InvocationCount(100_000)]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
[SuppressMessage("ReSharper", "MissingXmlDoc")]
[SuppressMessage("ReSharper", "UseRawString")]
public class ToUIStringBenchmarks
{
    private static readonly string NoQuotesLongString = "6ba832ef27be44b39701c283c9dfa0f8".Replicate(32);

    [Benchmark(Baseline = true)]
    public (string, string, string) RunBaseline()
        => (default(string).ToUIString(), string.Empty.ToUIString(), NoQuotesLongString.ToUIString());

    //// TEMP
    public (string, string, string) RunSomethingElse()
        => ("null", @"""""", @"""NoQuotesLongString""");
}