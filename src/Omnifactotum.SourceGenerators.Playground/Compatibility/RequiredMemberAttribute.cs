//// ReSharper disable CheckNamespace
//// ReSharper disable RedundantAttributeUsageProperty

#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute
{
}

#endif