#if NETSTANDARD && !NETSTANDARD1_0_OR_GREATER
#error [NETSTANDARD] The compiler does not support `NETSTANDARD*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NETSTANDARD2_1 && !NETSTANDARD2_1_OR_GREATER
#error [NETSTANDARD2_1] The compiler does not support `NETSTANDARD*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NETCOREAPP && !NETCOREAPP1_0_OR_GREATER
#error [NETCOREAPP] The compiler does not support `NETCOREAPP*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NETCOREAPP3_1 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER)
#error [NETCOREAPP3_1] The compiler does not support `NETCOREAPP*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET5_0 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER || !NET5_0_OR_GREATER)
#error [NET5_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET6_0 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER || !NET5_0_OR_GREATER || !NET6_0_OR_GREATER)
#error [NET6_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET7_0 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER || !NET5_0_OR_GREATER || !NET6_0_OR_GREATER || !NET7_0_OR_GREATER)
#error [NET7_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET8_0 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER || !NET5_0_OR_GREATER || !NET6_0_OR_GREATER || !NET7_0_OR_GREATER || !NET8_0_OR_GREATER)
#error [NET8_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET9_0 && (!NETCOREAPP1_0_OR_GREATER || !NETCOREAPP2_0_OR_GREATER || !NETCOREAPP3_0_OR_GREATER || !NETCOREAPP3_1_OR_GREATER || !NET5_0_OR_GREATER || !NET6_0_OR_GREATER || !NET7_0_OR_GREATER || !NET8_0_OR_GREATER || !NET9_0_OR_GREATER)
#error [NET9_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif
