// Source: https://github.com/dotnet/runtime/blob/v5.0.0/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !(NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER)

//// ReSharper disable once CheckNamespace :: Namespace must be declared with this name
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    ///     Specifies that the output will be non-null if the named parameter is non-null.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue,
        AllowMultiple = true)]
    internal sealed class NotNullIfNotNullAttribute : Attribute
    {
        /// <summary>
        ///     Initializes the attribute with the associated parameter name.
        /// </summary>
        /// <param name="parameterName">
        ///     The associated parameter name. The output will be non-null if the argument to the parameter specified is non-null.
        /// </param>
        public NotNullIfNotNullAttribute(string parameterName) => ParameterName = parameterName;

        /// <summary>
        ///     Gets the associated parameter name.
        /// </summary>
        [Omnifactotum.Annotations.UsedImplicitly]
        public string ParameterName { get; }
    }
}
#endif