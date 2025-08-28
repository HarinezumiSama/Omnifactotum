using System.Diagnostics;
using System.Runtime.CompilerServices;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum.ExtensionMethods;

internal static class InternalExtensionFactotum
{
#if NET5_0_OR_GREATER
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    public static string? GetEnsureNotNullAsyncArgumentNullMessage(string? valueExpression = null)
        => valueExpression is null ? null : $"The following expression is null: {{ {valueExpression} }}.";

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [DebuggerStepThrough]
    public static string? GetEnsureNotNullAsyncExpressionForEnsureNotNull(string? resultTaskExpression = null)
        => resultTaskExpression is null ? null : $"await {resultTaskExpression}";
#endif
}