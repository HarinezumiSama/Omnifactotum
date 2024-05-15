//// ReSharper disable CheckNamespace
//// ReSharper disable RedundantAttributeUsageProperty

#if !NET7_0_OR_GREATER

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;

    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public const string RefStructs = nameof(RefStructs);

    public const string RequiredMembers = nameof(RequiredMembers);
}

#endif