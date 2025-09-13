using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Omnifactotum.CompilerServices;

internal sealed class InternalLogger<T>
    where T : class
{
    private static readonly string MutexName = $@"Global\{typeof(InternalLogger<T>).Namespace}:{typeof(InternalLogger<T>).Name}|{typeof(T).FullName}";

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static readonly string LogFilePath = $@"C:\__{typeof(T).Namespace}.log";

    private static readonly string Prefix = typeof(T).Name;

    ////[Conditional("DEBUG")]
    [Conditional("__NON_EXISTING__")] //// To make the compiler remove calls to this method when `[Conditional("DEBUG")]` is commented out
    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers", Justification = "TEMP: For debugging")]
    public void AppendLog(string message)
    {
        using var mutex = new Mutex(false, MutexName);

        if (!mutex.WaitOne())
        {
            return;
        }

        try
        {
            var detailedMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff} | {Prefix}] {message}";

            Trace.WriteLine(detailedMessage);

            // Uncomment the line below temporarily (❗) when needed for debugging purposes
            ////System.IO.File.AppendAllText(LogFilePath, $"{detailedMessage}{InternalHelper.NewLine}{InternalHelper.NewLine}");
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
}