// Source: https://github.com/dotnet/runtime/blob/v5.0.0/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)

//// ReSharper disable RedundantAttributeUsageProperty

//// ReSharper disable once CheckNamespace :: Namespace must be declared with this name
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    ///     Specifies that an output will not be <see langword="null"/> even if the corresponding type allows it.
    ///     Specifies that an input argument was not <see langword="null"/> when the call returns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class NotNullAttribute : Attribute
    {
    }
}
#endif