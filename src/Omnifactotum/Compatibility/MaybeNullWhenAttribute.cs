// Source: https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)

//// ReSharper disable RedundantAttributeUsageProperty

//// ReSharper disable once CheckNamespace :: Namespace must be declared with this name
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    ///     Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        /// <summary>
        ///     Initializes the attribute with the specified return value condition.
        /// </summary>
        /// <param name="returnValue">
        ///     The return value condition. If the method returns this value, the associated parameter may be null.
        /// </param>
        public MaybeNullWhenAttribute(bool returnValue) => ReturnValue = returnValue;

        /// <summary>
        ///     Gets the return value condition.
        /// </summary>
        [Omnifactotum.Annotations.UsedImplicitly]
        public bool ReturnValue { get; }
    }
}
#endif