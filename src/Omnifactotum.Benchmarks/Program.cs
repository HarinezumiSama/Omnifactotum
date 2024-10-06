using System;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using Omnifactotum.Benchmarks;

try
{
    var config = new ManualConfig()
        .AddLogger(DynamicallyPrefixedLogger.CreateTimestampPrefixedLogger(ConsoleLogger.Default))
        .WithOptions(ConfigOptions.DisableOptimizationsValidator);

    var summaries = BenchmarkSwitcher
        .FromAssemblies(
            new[]
            {
                typeof(Program).Assembly
            })
        .RunAll(config)
        .ToArray();

    if (summaries.Length == 0)
    {
        throw new ApplicationException("No benchmarks were run.");
    }

    var failedSummaries = summaries.Where(s => s.ValidationErrors.Any() || s.Reports.Any(r => !r.Success)).ToArray();
    if (failedSummaries.Length != 0)
    {
        var details = string.Join(",\x0020", failedSummaries.Select(s => $@"""{s.Title}"""));
        throw new ApplicationException($"The following benchmarks failed: {details}");
    }
}
catch (Exception ex)
{
    Console.ResetColor();
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex);
    Console.ResetColor();

    Console.WriteLine();
    return 1;
}

return 0;
