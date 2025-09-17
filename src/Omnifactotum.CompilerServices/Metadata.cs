using System.Threading.Tasks;

namespace Omnifactotum.CompilerServices;

internal static class Metadata
{
    public const string AsyncMethodSuffix = "Async";

    public static class FullName
    {
        public static readonly string VoidTask = typeof(Task).FullName.EnsureNotNull();
        public static readonly string ResultTask = typeof(Task<>).FullName.EnsureNotNull();

        public static readonly string VoidValueTask = "System.Threading.Tasks.ValueTask";
        public static readonly string ResultValueTask = "System.Threading.Tasks.ValueTask`1";

        public static readonly string AsyncEnumerable = "System.Collections.Generic.IAsyncEnumerable`1";
    }
}