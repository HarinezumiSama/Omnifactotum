# [Omnifactotum Analyzers](./README.md)

## Rule `OFCA0003`

- `Asynchronous method/function lacks 'CancellationToken' parameter`
  - There might be certain exceptions to this rule when it is not applicable and thus can be ignored (see _[Additional Information](#additional-information)_).

### Additional Information

- When required, a diagnostic rule can be:
  - suppressed using [#pragma warning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning); or
  - suppressed using [SuppressMessageAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.suppressmessageattribute); or
  - ignored using the [NoWarn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) project property.
