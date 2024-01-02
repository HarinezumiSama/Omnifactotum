### Changes in 0.6.0 (since 0.5.0)

#### Breaking changes

- `OmnifactotumArrayExtensions`
  - `Initialize<T>()` overloads now return the passed array instead of `void`
- Enforced the `Enum` constraint on the applicable generic types and methods
  - `EnumFactotum` methods:
    - `GetValue<TEnum>()`
    - `GetValue<TEnum>()`
    - `GetAllValues<TEnum>()`
    - `GetAllFlagValues<TEnum>()`
  - `EnumFixedSizeDictionary<TKey, TValue>` class
  - `EnumFixedSizeDictionaryDeterminant<TKey>` class
  - `OmnifactotumEnumExtensions` methods:
    - `IsAllSet<TEnum>()`
    - `IsAnySet<TEnum>()`
    - `IsOneOf<TEnum>()`

#### Deprecations

- Object validation
  - Deprecated `NotNullOrWhiteSpaceStringConstraint` in favor of the newly
    added `NotBlankStringConstraint`

#### New features

- Types
  - Added delegate `Task EventHandlerAsync<>(...)` (excluding .NET Framework 4.0)
  - Added `OutFunc<...>` delegates (similar to `System.Func<>`, but having an `out` parameter of the type `TOutput`)
    - `OutFunc<TOutput, out TResult>`
    - `OutFunc<in T, TOutput, out TResult>`
    - `OutFunc<in T1, in T2, TOutput, out TResult>`
    - `OutFunc<in T1, in T2, in T3, TOutput, out TResult>`
  - Added `SemaphoreSlimBasedLock`
  - Object validation:
    - Added `NotBlankStringConstraint`
    - Added `NotNullOrEmptyCollectionConstraint`
    - Added `NotNullOrEmptyCollectionConstraint<T>`
    - Added `WebUrlConstraint`
- Extension Methods
  - Added `EventHandlerAsyncExtensions` (extension methods for `EventHandlerAsync<>`) (excluding .NET Framework 4.0)
  - `OmnifactotumArrayExtensions`
    - Using Nullable Reference Types (where applicable)
    - Added `Initialize<T>(T[], T)` (initializing with a specific constant value)
  - Updated `OmnifactotumCollectionExtensions`
    - Added `DoForEachAsync`
    - Using Nullable Reference Types for all the methods (where applicable). Particularly:
      - `GetFastCount`
      - `CollectionsEquivalent`
      - `CollectionsEqual`
      - `FindDuplicates`
      - `DisposeCollectionItemsSafely`
      - `AvoidNull`
      - `ToHashSet`
      - `ToUIString`
  - Updated `OmnifactotumDateTimeExtensions`
    - Added `EnsureKind()`, `EnsureUtc()`, and `EnsureLocal()` methods
    - Added `ToFixedStringWithMilliseconds()` method
    - Exposed `FixedStringFormat`, `FixedStringWithMillisecondsFormat`, and `PreciseFixedStringFormat` fields
  - Updated `OmnifactotumDateTimeOffsetExtensions`
    - Added `ToFixedStringWithMilliseconds()` method
    - Exposed `FixedStringFormat`, `FixedStringWithMillisecondsFormat`, and `PreciseFixedStringFormat` fields
  - Added `OmnifactotumDelegateExtensions` (extension methods for delegates) with method:
    - `GetTypedInvocations()`
  - Added `OmnifactotumEqualityComparerExtensions` (extension methods for `IEqualityComparer<T>`) with method:
    - `GetHashCodeSafely(this IEqualityComparer<T>, ...)`
  - Updated `OmnifactotumGenericObjectExtensions`:
    - Added `GetObjectReferenceDescription()`
    - Added `GetShortObjectReferenceDescription()`
    - Added `AsNullable<T>` (`where T : struct`)
    - Applied `NotNullIfNotNull` annotation on `EnsureNotNull()` (.NET Standard 2.1+ and .NET 5.0+ only)
    - Using Nullable Reference Types (where applicable). Particularly:
      - `ToStringSafely()`
      - `ToStringSafelyInvariant()`
      - `GetHashCodeSafely()`
      - `GetTypeSafely()`
      - `ToUIString()`
      - `ToPropertyString()`
      - `IsEqualByContentsTo()`
      - `Morph()`
  - Added `OmnifactotumHttpStatusCodeExtensions` with method:
    - `ToUIString(HttpStatusCode)`
  - Added `OmnifactotumNullableDateTimeExtensions` with methods:
    - `ToFixedString()`
    - `ToFixedStringWithMilliseconds()`
    - `ToPreciseFixedString()`
  - Added `OmnifactotumNullableDateTimeOffsetExtensions` with methods:
    - `ToFixedString()`
    - `ToFixedStringWithMilliseconds()`
    - `ToPreciseFixedString()`
  - Added `OmnifactotumTaskExtensions` with methods (excluding .NET Framework 4.0):
    - `AwaitAllAsync(IEnumerable<Task>)`
    - `AwaitAllAsync(IEnumerable<Task<TResult>>)`
    - `ConfigureAwaitNoCapturedContext(Task)`
    - `ConfigureAwaitNoCapturedContext(Task<TResult>)`
  - Added `OmnifactotumValueTaskExtensions` with methods (excluding .NET Framework 4.0):
    - `ConfigureAwaitNoCapturedContext(ValueTask)`
    - `ConfigureAwaitNoCapturedContext(ValueTask<TResult>)`

#### Minor Updates

- `OmnifactotumArrayExtensions`: Minor optimization in `AvoidNull()`
- `OmnifactotumGenericObjectExtensions`
  - `ToStringSafely()` and `ToStringSafelyInvariant()` now never return `null` and instead fall back to `string.Empty`
- `OmnifactotumStringExtensions`: Fixed annotation in `ToUIString()`
- Applied `MethodImplOptions.AggressiveInlining` (and, when possible, `MethodImplOptions.AggressiveOptimization`) to the methods, where reasonable. Affected classes:
  - `OmnifactotumGenericObjectExtensions`
  - `OmnifactotumCollectionExtensions`

---

### Changes in 0.5.0 (since 0.4.1)

#### Breaking changes

- `ReadOnlySet` class:
  - `IsReadOnly` property now implements `ICollection<T>.IsReadOnly` **explicitly**
- `IValueContainer<T>` moved from the namespace `Omnifactotum` to `Omnifactotum.Abstractions`

#### New features

- Added `ICurrentDateTimeProvider`, `CurrentDateTimeProviderExtensions`, and `LocalComputerCurrentDateTimeProvider`
- Added `IElapsedTimeProvider`, `ElapsedTimeProviderExtensions`, and `StopwatchElapsedTimeProvider`
- The `ReadOnlySet` class now implements `IReadOnlySet` (.NET 5+)
- Added new extension methods for `System.String`:
  - `IsWebUri(string)`
  - `ToSecureString(string)`
  - `WithoutTrailingSlash(string)`
  - `WithSingleTrailingSlash(string)`
- Added new extension methods for `System.Uri`:
  - `IsWebUri(Uri)`
  - `ToUIString(Uri)`
  - `WithoutTrailingSlash(Uri)`
  - `WithSingleTrailingSlash(Uri)`
- Added new extension methods for `System.Security.SecureString`:
  - `IsNullOrEmpty(SecureString)`
  - `ToPlainText(SecureString)`
- Added `ContractAnnotation` to `OmnifactotumGenericObjectExtensions.EnsureNotNull`
- Improvements in the `OmnifactotumStringExtensions` class:
  - Using Nullable Reference Types (where applicable)
  - Added the `ContractAnnotation` annotations (where applicable)
  - Added the `AggressiveInlining` flag (where applicable)
  - Improved parameter annotations (using `NotNullWhen`, `ItemCanBeNull` etc. where applicable)
  - Other minor improvements

#### Minor Updates

- Polished the XML documentation
- Minor optimizations in the `Factotum` class:
  - Applied `MethodImplOptions.AggressiveInlining` where applicable
  - Forwarding `CreateEmptyCompletedTask()` to `Task.CompletedTask` (except for NET 4.0)
  - Forwarding `CreateEmptyFaultedTask(Exception)` to `Task.FromException(Exception)` (except for NET 4.0)

---

### Changes in 0.4.1 (since 0.4.0)

- Added the `Factotum.For<TObject>.Identity` method (same as `Factotum.Identity<T>`)
- Added `Factotum.For<TObject>.IdentityMethod` (the cached reference to the `Factotum.For<TObject>.Identity` method)
- Using the `Deterministic` build option
- Using Portable PDBs
- Using `snupkg` format of the symbol package

---

### Changes in 0.4.0 (since 0.3.0.119)

#### Major Updates

- `Omnifactotum` is now the multi-target package for:
  - .NET Framework 4.0, 4.6.1, and 4.7.2
  - .NET Standard 2.0 and 2.1
  - .NET 5.0
- Addressed multi-target compatibility issues in:
  - `ColoredConsoleTraceListener`
  - `OmnifactotumAssemblyExtensions`
  - `OmnifactotumCollectionExtensions`
  - `OmnifactotumDictionaryExtensions`
  - `OmnifactotumOperationContextExtensions`
  - `KeyValuePair` (static helper)
  - Omnifactotum's `ReadOnlyDictionary<TKey, TValue>`
  - `WinEventLog`
- Added `OmnifactotumKeyValuePair` static helper for facilitating migration from .NET Framework to .NET Standard/Core

#### Breaking changes

- `Factotum`: Removed the methods `ToPropertyString` and `AreEqualByContents` in favor of methods `ToPropertyString` and `IsEqualByContentsTo` in `OmnifactotumGenericObjectExtensions`
- `OmnifactotumArrayExtensions`: The method `AsReadOnly()` moved to `OmnifactotumCollectionExtensions`
- `OmnifactotumCustomAttributeProviderExtensions`: Removed the obsolete method `GetCustomAttributes`
- `OmnifactotumGenericObjectExtensions`: Removed the methods `Affirm` and `ComputePredicate`
- `OmnifactotumMathExtensions`: Removed the methods `Sqr(decimal)`, `SqrChecked(float)`, `SqrChecked(double)` since their expected behavior cannot be achieved

#### New features

- `OmnifactotumCollectionExtensions`: Added `ToUIString` implementation for collections of strings and collections of nullable value type instances
- `OmnifactotumGenericObjectExtensions`: Implemented `Morph()` overloads for nullable value types (to complement already existing method for the reference types)
- `OmnifactotumNullableBooleanExtensions`: Added an overload of `ToString` accepting value provider delegates
- Added `ReadOnlyItemCollection<T>` (read-only wrapper for `ICollection<T>`)
- Added partial support of Nullable Reference Types:
  - `OmnifactotumGenericObjectExtensions.EnsureNotNull`
- Exposed the class `OmnifactotumRepresentationConstants` (formerly `OmnifactotumConstants`)
- Applied `PureAttribute`, `InstantHandleAttribute`, and `NoEnumerationAttribute` annotations in certain appropriate cases

#### Fixes and improvements

- Improved documentation
- `KeyedEqualityComparer<T, TKey>`: `IEqualityComparer.Equals` and `IEqualityComparer.GetHashCode` now don't throw an exception f an argument is not compatible with the type `T`
- `OmnifactotumAssemblyExtensions`
- `OmnifactotumCollectionHashCodeHelper` (as per the fix in `OmnifactotumHashCodeHelper`)
- `OmnifactotumDateTimeOffsetExtensions`
- `OmnifactotumEnumExtensions`: `GetName`, `GetQualifiedName`, `GetFullName`, `EnsureDefined`, and `IsDefined`
- `OmnifactotumGenericObjectExtensions`:
  - `ToUIString()` now uses the invariant culture if the value is `IFormattable`
  - Fix in `IsEqualByContentsTo()` for the case of a type with no fields
- `OmnifactotumHashCodeHelper`: Fix in CombineHashCodeValues (for case when the next hash code is zero)
- `OmnifactotumMethodBaseExtensions`
- `OmnifactotumTypeExtensions`
- Updated JetBrains Annotations in `Omnifactotum.Annotations`

---

### Changes in 0.3.0.119 (since 0.3.0.117)
- Omnifactotum: Removed NuGet dependency to the `MSBuildTasks` package since it is only used for development

---

### Changes in 0.3.0.117 (since 0.3.0.114)
- Omnifactotum: `Factotum` and `Factotum.For<TObject>`: Improved annotations

---

### Changes in 0.3.0.114 (since 0.3.0.101)
- `OmnifactotumAssemblyExtensions`: Improvements
- **BREAKING CHANGE**: `OmnifactotumDisposableExtensions`: `DisposeSafely` now works only for reference and nullable types (2 overloads)
- **BREAKING CHANGE**: `OmnifactotumCollectionExtensions`: `DisposeCollectionItemsSafely` now works only for reference and nullable types (2 overloads)
- **BREAKING CHANGE**: `Factotum`: `DisposeAndNull` now works also for nullable types (besides reference types)
- Introduced the `AsyncFactotum` class with the overloaded methods `ComputeAsync` and `ExecuteAsync`
- Fix in `MemberConstraintBase.CastTo<T>` (and hence in `TypedMemberConstraintBase`) for nullable types
- Improved annotations in Object Validator and related classes
- Minor improvements

---

### Changes in 0.3.0.101 (since 0.3.0.90)
- `KeyedEqualityComparer<T, TKey>`: Fixes and improvements
- `KeyedEqualityComparer` static helper class has been introduced
- `Factotum`: `CreateEmptyCompletedTask` and `CreateEmptyFaultedTask` methods have been introduced
- `OmnifactotumGenericObjectExtensions`: `EnsureNotNull` (for nullable) has been introduced
- `IValueContainer<T>` has been introduced for `ValueContainer<T>` and `SyncValueContainer<T>`
- `ValueContainer` and `SyncValueContainer` helper static classes have been introduced
- Improvements and fixes in `OmnifactotumMethodBaseExtensions` and `OmnifactotumTypeExtensions`
- `OmnifactotumStringExtensions`: `TrimSafely`, `TrimStartSafely`, `TrimEndSafely` and `Shorten` methods now never return null

---

### Changes in 0.3.0.90 (since 0.3.0.86)
- **BREAKING CHANGE** `OmnifactotumCustomAttributeProviderExtensions`: `GetCustomAttributes` has been renamed to `GetCustomAttributeArray` (for compatibility with FW 4.5+)

---

### Changes in 0.3.0.86 (since 0.3.0.83)
- **BREAKING CHANGE** `OmnifactotumDictionaryExtensions`: The method `GetValueOrCreate` has been renamed to `GetOrCreateValue` for readability and in order to avoid confusion with `GetValueOrDefault`
- `OmnifactotumDictionaryExtensions`: Improved annotations
- `ValueContainer` and `SyncValueContainer`:
    - Support for equality comparison
    - Added `DebuggerDisplay` attribute and `ToString()` method
- `OmnifactotumMathExtensions` class has been introduced:
    - `Sqr` (square)
    - `Sqrt` (square root)
    - `Abs` (absolute value)

---

### Changes in 0.3.0.83 (since 0.3.0.82)
- `FixedSizeDictionary`: Improved annotations
- `FixedSizeDictionary`: Implemented version verification in the enumerator

---

### Changes in 0.3.0.82 (since 0.3.0.80)
- `FixedSizeDictionary`: Removed index verification in the internal determinant since this verification highly affected performance

---

### Changes in 0.3.0.80 (since 0.3.0.79)
- `EnumFixedSizeDictionary`: Fix in determining the size

---

### Changes in 0.3.0.79 (since 0.3.0.74)
- `FixedSizeDictionary` and `EnumFixedSizeDictionary` have been introduced.
- Applied Omnifactotum Annotations to the `Factotum` and `OmnifactotumGenericObjectExtensions` classes.

---

### Changes in 0.3.0.74 (since 0.2.0.59)
- **BREAKING CHANGE** Object Validation: Redesign and support of `IEnumerable`.
- **BREAKING CHANGE** Renamed `EnumHelper` to `EnumFactotum`.
- **BREAKING CHANGE** `OmnifactotumExpressionExtensions` is now in the namespace `System.Linq.Expressions` (instead of System).
