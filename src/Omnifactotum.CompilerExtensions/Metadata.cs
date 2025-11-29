using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnifactotum.CompilerExtensions;

internal static class Metadata
{
    public const string AsyncMethodSuffix = "Async";

    public static class KnownType
    {
        public static readonly Type CancellationToken = typeof(CancellationToken);

        public static readonly Type VoidTask = typeof(Task);
        public static readonly Type ResultTask = typeof(Task<>);
    }

    public static class FullName
    {
        public static readonly string CancellationToken = KnownType.CancellationToken.FullName.EnsureNotNull();

        public static readonly string VoidTask = KnownType.VoidTask.FullName.EnsureNotNull();
        public static readonly string ResultTask = KnownType.ResultTask.FullName.EnsureNotNull();

        public const string VoidValueTask = "System.Threading.Tasks.ValueTask";
        public const string ResultValueTask = "System.Threading.Tasks.ValueTask`1";

        public const string AsyncEnumerable = "System.Collections.Generic.IAsyncEnumerable`1";
    }

    public static class Name
    {
        public static readonly string CancellationToken = KnownType.CancellationToken.Name.EnsureNotNull();
    }
}