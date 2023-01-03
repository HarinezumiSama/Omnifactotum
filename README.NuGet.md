# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.11.0 (since 0.10.0)

#### Breaking Changes

- `OmnifactotumEnumExtensions`: Added the `enumerationValueExpression` parameter to the `CreateEnumValueNotImplementedException<TEnum>()` and `CreateEnumValueNotSupportedException<TEnum>()` methods:
  - For .NET 5+ and higher, the `enumerationValueExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `enumerationValueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `OmnifactotumNullableEnumExtensions`: Added the `enumerationValueExpression` parameter to the `CreateEnumValueNotImplementedException<TEnum>()` and `CreateEnumValueNotSupportedException<TEnum>()` methods:
  - For .NET 5+ and higher, the `enumerationValueExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `enumerationValueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `Factotum`: Removed obsolete methods:
  - `CreateEmptyCompletedTask()`
  - `CreateEmptyFaultedTask(Exception)`
- Validation: Removed obsolete `NotNullOrWhiteSpaceStringConstraint`
- NuGet: Set `MinClientVersion` to `5.4.0`

#### Deprecations

- `OmnifactotumTypeExtensions`:
  - Deprecated `IsNullable(Type)` in favor of `IsNullableValueType(Type)`
  - Deprecated `GetCollectionElementType(Type)` in favor of `GetCollectionElementTypeOrDefault(Type)`

#### New features

- `OmnifactotumCollectionExtensions`:
  - Added the `WhereNotNull<T>` extension method
- `OmnifactotumStringExtensions`:
  - Added the `WhereNotEmpty<T>` and `WhereNotBlank<T>` extension methods
- Added `OmnifactotumImmutableArrayExtensions` (extension methods for `ImmutableArray<T>`):
  - `AvoidDefault(this ImmutableArray<T>)` extension method
- `OmnifactotumCollectionExtensions`
  - For .NET Standard 2.1+ and .NET 5+:
    - Added `ConfigureAwaitNoCapturedContext<T>(this IAsyncEnumerable<T>)`
    - Added `ConfigureAwaitNoCapturedContext<T>(this ConfiguredCancelableAsyncEnumerable<T>)`
    - Added `EnumerateToListAsync<T>(this IAsyncEnumerable<T>, CancellationToken)`
    - Added `EnumerateToArrayAsync<T>(this IAsyncEnumerable<T>, CancellationToken)`
- `OmnifactotumDisposableExtensions`
  - For .NET Standard 2.1+ and .NET 5+:
    - Added `ConfigureAwaitNoCapturedContext(this IAsyncDisposable)`

#### Minor updates and fixes

- `OmnifactotumCollectionExtensions`: Fixed/improved annotations on `ToUIString()` methods
- Fixed/improved NRT related annotations on:
  - `OmnifactotumGenericObjectExtensions`:
    - `EnsureNotNull()`
  - `OmnifactotumOperationContextExtensions`:
    - `GetAllClientCertificates()`
  - `OmnifactotumUriExtensions`:
    - `EnsureWebUri()`
    - `EnsureAbsoluteUri()`
  - `NUnitFactotum`
    - `AssertNotNull()`
- `OmnifactotumCollectionExtensions`: Improved `AvoidNull()`: Now considering `ImmutableArray<T>.IsDefault` to avoid accessing an uninitialized instance
- Applied `ConfigureAwaitNoCapturedContext` (that is, `ConfigureAwait(false)`) in:
  - `EventHandlerAsyncExtensions`:
    - `InvokeAsync<TEventArgs>(...)` extension method
    - `InvokeParallelAsync<TEventArgs>(...)` extension method
  - `OmnifactotumCollectionExtensions`:
    - `DoForEachAsync<T>()` extension methods
  - `OmnifactotumTaskExtensions`:
    - `AwaitAllAsync(this IEnumerable<Task>)`
    - `AwaitAllAsync<TResult>(this IEnumerable<Task<TResult>>)`
  - `SemaphoreSlimBasedLock`
    - `AcquireAsync(CancellationToken)`
- Minor code improvements
- Minor XML documentation improvements
