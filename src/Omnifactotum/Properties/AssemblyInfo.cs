using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if NETFRAMEWORK && !NET461_OR_GREATER
#error [NETFRAMEWORK] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NETSTANDARD && !NETSTANDARD1_0_OR_GREATER
#error [NETSTANDARD] The compiler does not support `NETSTANDARD*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NETCOREAPP && !NETCOREAPP1_0_OR_GREATER
#error [NETCOREAPP] The compiler does not support `NETCOREAPP*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

#if NET5_0 && !NET5_0_OR_GREATER
#error [NET5_0] The compiler does not support `NET*_OR_GREATER` preprocessor symbols required to compile this project.
#endif

// Miscellaneous
[assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("121251ac-cddf-4d5b-bea6-15b3cb8fbb4b")]

// Friend test assemblies
[assembly: InternalsVisibleTo("Omnifactotum.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000"
    + "01000100c18c17a450bec4ea1d697b35548ccc0f46aab066e0a64399ad09cc24061b1a7af34d77199308630917670d31bf0898238ac438"
    + "2e94d23ccdb1094c7f428a074efe56a320f030118c61d62cbf6af9a36ede5b95ce2799ed32876bace35a4b81bb31e82916945a7f9f9a75"
    + "b2e581054cecd4af62af50a563babe5baf00a854f49f")]