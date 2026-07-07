using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnifactotum.CompilerExtensions;

internal static class Metadata
{
    public const string AsyncMethodSuffix = "Async";

    public static class KnownType
    {
        public static readonly Type SystemType = typeof(Type);
        public static readonly Type CancellationToken = typeof(CancellationToken);

        public static readonly Type VoidTask = typeof(Task);
        public static readonly Type ResultTask = typeof(Task<>);
    }

    public static class FullName
    {
        public const string MemberConstraintAttribute = "Omnifactotum.Validation.Annotations.MemberConstraintAttribute";
        public const string GenericMemberConstraintAttribute = "Omnifactotum.Validation.Annotations.MemberConstraintAttribute`1";
        public const string MemberItemConstraintAttribute = "Omnifactotum.Validation.Annotations.MemberItemConstraintAttribute";
        public const string GenericMemberItemConstraintAttribute = "Omnifactotum.Validation.Annotations.MemberItemConstraintAttribute`1";
        public const string IMemberConstraint = $"Omnifactotum.Validation.Constraints.{Name.IMemberConstraint}";
        public const string TypedMemberConstraintBase = "Omnifactotum.Validation.Constraints.TypedMemberConstraintBase`1";

        public const string VoidValueTask = "System.Threading.Tasks.ValueTask";
        public const string ResultValueTask = "System.Threading.Tasks.ValueTask`1";

        public const string AsyncEnumerable = "System.Collections.Generic.IAsyncEnumerable`1";
        public const string NonGenericListInterface = "System.Collections.IList";
        public const string ImmutableArray = "System.Collections.Immutable.ImmutableArray`1";

        public static readonly string SystemType = KnownType.SystemType.FullName.EnsureNotNull();
        public static readonly string CancellationToken = KnownType.CancellationToken.FullName.EnsureNotNull();

        public static readonly string VoidTask = KnownType.VoidTask.FullName.EnsureNotNull();
        public static readonly string ResultTask = KnownType.ResultTask.FullName.EnsureNotNull();
    }

    public static class Name
    {
        public const string IMemberConstraint = "IMemberConstraint";

        public static readonly string CancellationToken = KnownType.CancellationToken.Name.EnsureNotNull();
    }
}