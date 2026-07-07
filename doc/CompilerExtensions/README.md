# Omnifactotum Compiler Extensions

## Analyzer Rules

| Rule ID                        | Title                                                                                    |
|:-------------------------------|:-----------------------------------------------------------------------------------------|
| [OFCA0001](./Rule-OFCA0001.md) | Asynchronous method/function lacks 'Async' suffix                                        |
| [OFCA0002](./Rule-OFCA0002.md) | Synchronous method/function has 'Async' suffix                                           |
| [OFCA0003](./Rule-OFCA0003.md) | Asynchronous method/function lacks 'CancellationToken' parameter                         |
| [OFCA0011](./Rule-OFCA0011.md) | Validation constraint type does not implement the required interface `IMemberConstraint` |
| [OFCA0012](./Rule-OFCA0012.md) | Validation constraint type does not have a required parameterless constructor            |
| [OFCA0013](./Rule-OFCA0013.md) | Validation attribute can be replaced with its generic equivalent                         |
| [OFCA0014](./Rule-OFCA0014.md) | Validation constraint type is not compatible with the type of the validated value        |

### Additional Information

- When required, a diagnostic rule can be:
  - suppressed using [#pragma warning](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/preprocessor-directives#pragma-warning); or
  - suppressed using [SuppressMessageAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.suppressmessageattribute); or
  - ignored using the [NoWarn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/errors-warnings#nowarn) project property.
