using System.Threading.Tasks;

namespace Omnifactotum.CompilerServices;

internal static class Metadata
{
    public const string AsyncMethodSuffix = "Async";

    //// public static class Namespace
    //// {
    //// }
    ////
    //// public static class Name
    //// {
    //// }

    public static class FullName
    {
        public static readonly string VoidTask = SystemType.VoidTask.FullName.EnsureNotNull();
        public static readonly string ResultTask = SystemType.ResultTask.FullName.EnsureNotNull();

        public static readonly string VoidValueTask = SystemType.VoidValueTask.FullName.EnsureNotNull();
        public static readonly string ResultValueTask = SystemType.ResultValueTask.FullName.EnsureNotNull();
    }

    public static class SystemType
    {
        public static readonly System.Type VoidTask = typeof(Task);
        public static readonly System.Type ResultTask = typeof(Task<>);

        public static readonly System.Type VoidValueTask = typeof(ValueTask);
        public static readonly System.Type ResultValueTask = typeof(ValueTask<>);
    }
}