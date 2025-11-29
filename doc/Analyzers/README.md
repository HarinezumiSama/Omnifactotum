# Omnifactotum Analyzers

## Rules

| Rule ID                        | Title                                                            |
|:-------------------------------|:-----------------------------------------------------------------|
| [OFCA0001](./Rule-OFCA0001.md) | Asynchronous method/function lacks 'Async' suffix                |
| [OFCA0002](./Rule-OFCA0002.md) | Synchronous method/function has 'Async' suffix                   |
| [OFCA0003](./Rule-OFCA0003.md) | Asynchronous method/function lacks 'CancellationToken' parameter |

### Additional Information

- When required, a diagnostic rule can be:
  - suppressed using [#pragma warning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning); or
  - suppressed using [SuppressMessageAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.suppressmessageattribute); or
  - ignored using the [NoWarn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) project property.
