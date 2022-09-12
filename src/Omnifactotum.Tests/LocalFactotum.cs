#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER)
using System;
#endif

namespace Omnifactotum.Tests;

internal static class LocalFactotum
{
    public static string GetArgumentExceptionParameterDetails(string parameterName)
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
        return $"\x0020(Parameter '{parameterName}')";
#else
        return $"{Environment.NewLine}Parameter name: {parameterName}";
#endif
    }
}