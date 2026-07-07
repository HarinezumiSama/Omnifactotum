# [Omnifactotum Compiler Extensions](./README.md)

## Analyzer Rule `OFCA0013`

- `Validation attribute can be replaced with its generic equivalent`
  - When the C# language version is 11 or higher, a non-generic `MemberConstraintAttribute` (or `MemberItemConstraintAttribute`) that specifies the constraint type via `typeof(...)` can be replaced with its strongly-typed generic equivalent (for example, `[MemberConstraint(typeof(MyConstraint))]` can be replaced with `[MemberConstraint<MyConstraint>]`).

### Additional Information

- When required, a diagnostic rule can be:
  - suppressed using [#pragma warning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning); or
  - suppressed using [SuppressMessageAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.suppressmessageattribute); or
  - ignored using the [NoWarn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) project property.
