### Changes in 0.12.0 (since 0.11.0)

#### Breaking changes

- Removed support of **.NET Framework 4.6.1**

#### New features

- `TemplatedStringResolver`: Added the static `GetVariableNames()` method
- `Factotum`: Added the `Assert(...)` method
  - For .NET 5+ and higher, the `conditionExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `conditionExpression` parameter is supplied only for binary compatibility between the different target frameworks

#### Minor updates and fixes

- Minor improvements in `OmnifactotumHttpStatusCodeExtensions.ToUIString()`
- Improved documentation on `AssertionMethodAttribute` and `AssertionConditionAttribute`

---

### Changes in 0.11.0 (since 0.10.0)

#### Breaking changes

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

---

### Changes in 0.10.0 (since 0.9.0)

#### New features

- Added the `CaseInsensitiveStringKey` structure
- `OmnifactotumDateTimeExtensions`: Added the `AsKind(this DateTime, DateTimeKind)` extension method

---

### Changes in 0.9.0 (since 0.8.0)

#### Breaking changes

- `ByReferenceEqualityComparer<T>` class: Enforced the `class` constraint on `T`
- `DirectedGraphNode<T>`: Removed the parameterless constructor overload and applied the corresponding default value in the remaining constructor
- `EnumFixedSizeDictionaryDeterminant<TKey>`: The `Size` property became `protected` as per inheritance (used to be `public`)
- `EnumFixedSizeDictionary<TKey, TValue, TDeterminant>`: The constructor previously accepting `IDictionary<TKey, TValue>` now accepts `IEnumerable<KeyValuePair<TKey, TValue>>` (as per inheritance)
- `FixedSizeDictionary<TKey, TValue, TDeterminant>` and `EnumFixedSizeDictionary<TKey, TValue>` now implement `IReadOnlyDictionary<TKey, TValue>`
- `Factotum.ProcessRecursively<T>()` method: Removed overloads not having the `RecursiveProcessingContext<T>` parameter and applied the default value for this parameter in the other overloads
- `FixedSizeDictionaryDeterminant<TKey>`: The `Size` property became `protected` (used to be `public`)
- `FixedSizeDictionary<TKey, TValue, TDeterminant>`: The constructor previously accepting `IDictionary<TKey, TValue>` now accepts `IEnumerable<KeyValuePair<TKey, TValue>>`
- `KeyedEqualityComparer<T, TKey>`: Removed the constructor without the key comparer parameter and applied the default value to the key comparer parameter in the remaining constructor
- `OmnifactotumDateTimeExtensions`: Added the `valueExpression` parameter to the `EnsureKind()`, `EnsureUtc()`, and `EnsureLocal()` methods
  - For .NET 5+ and higher, the `valueExpression` parameter is marked with `CallerArgumentExpression`
  - For older .NET versions, the `valueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `OmnifactotumEnumExtensions`: Made methods generic and enforced the `Enum` constraint on:
  - `GetName()`
  - `GetQualifiedName()`
  - `GetFullName()`
  - `IsDefined()`
  - `EnsureDefined()`
  - `CreateEnumValueNotImplementedException()`
  - `CreateEnumValueNotSupportedException()`
- `OmnifactotumEnumExtensions`: Added the `enumerationValueExpression` parameter to the `EnsureDefined<TEnum>()` method
  - For .NET 5+ and higher, the `enumerationValueExpression` parameter is marked with `CallerArgumentExpression`
  - For older .NET versions, the `enumerationValueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `OmnifactotumGenericObjectExtensions`: Removed the `ToPropertyString<T>()` methods
- `OmnifactotumGenericObjectExtensions`: Added the `valueExpression` parameter to the `EnsureNotNull<T>()` methods
  - For .NET 5+ and higher, the `valueExpression` parameter is marked with `CallerArgumentExpression`
  - For older .NET versions, the `valueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `OmnifactotumUriExtensions`: Added the `valueExpression` parameter to the `EnsureAbsoluteUri()` and `EnsureWebUri()` methods
  - For .NET 5+ and higher, the `valueExpression` parameter is marked with `CallerArgumentExpression`
    - For older .NET versions, the `valueExpression` parameter is supplied only for binary compatibility between the different target frameworks
- `RecursiveProcessingContext<T>` class: Removed the parameterless constructor and applied the default value for the parameter in the parameterized constructor
- `SyncValueContainer<T>`: Removed parameterless constructor
- **Validation** types: Marked Validation attributes non-CLS compliant (as per compiler warnings)
  - `BaseValidatableMemberAttribute`
  - `ValidatableMemberAttribute`
  - `BaseMemberConstraintAttribute`
  - `MemberConstraintAttribute`
  - `MemberItemConstraintAttribute`
- `ValueContainer<T>`: Removed parameterless constructor
-`VirtualTreeNode.Create<T>` method: Added optional parameter `IReadOnlyCollection<VirtualTreeNode<T>>? children`
- `VirtualTreeNode<T>`: Replaced all constructors with single `VirtualTreeNode(T value, IReadOnlyCollection<VirtualTreeNode<T>>? children = null)`
- `VirtualTreeNodeBase<T>`: Redesigned constructors and made the remaining constructor non visible outside assembly (`private protected`)
- `VirtualTreeNodeRoot<T>`: Replaced all constructors with single `VirtualTreeNodeRoot(IReadOnlyCollection<VirtualTreeNode<T>>? children = null)`
- `WeakReferenceBasedCache<TKey, TValue>`
  - Applied the `notnull` constraint on the `TKey` type parameter
  - Removed the constructor overload without the `IEqualityComparer<TKey>? keyEqualityComparer` parameter and applied the corresponding default value in the remaining constructor

#### New features

- Added support of **.NET 6**
- Enabled **Nullable Reference Types** for all the types across the entire project (except `Omnifactotum.Annotations.*`)
- `ComparableObjectBase`: Implemented `IComparable`
- `OmnifactotumCollectionExtensions`: Added the `Chunk<TSource>(this IEnumerable<TSource> source, int size)` extension method to use with .NET versions prior to 6.0
- Added `OmnifactotumNullableEnumExtensions`:
  - `CreateEnumValueNotImplementedException<TEnum>()`
  - `CreateEnumValueNotSupportedException<TEnum>()`
- `OmnifactotumStringExtensions`:
  - Added the `TrimPostfix()` method
  - Added the `TrimPrefix()` method
- Added `VirtualTreeNodeRoot` static helper class
- **Validation**:
  - Added `EnumValueDefinedConstraint<TEnum>` constraint
  - Added `NullableEnumValueDefinedConstraint<TEnum>` constraint

#### Minor updates and fixes

- Applied `AggressiveInlining` (or `AggressiveInlining` and `AggressiveOptimization`), where appropriate, in:
  - `ComparableObjectBase`
  - `DirectedGraphNode`
  - `EquatableObjectBase`
  - `KeyedEqualityComparer`
  - `OmnifactotumDateTimeExtensions`
  - `OmnifactotumDisposableExtensions`
  - `OmnifactotumEnumExtensions`
  - `OmnifactotumHashCodeHelper`
  - `OmnifactotumMathExtensions`
  - `OmnifactotumMethodBaseExtensions`
  - `OmnifactotumNullableBooleanExtensions`
  - `OmnifactotumOperationContextExtensions`
  - `OmnifactotumSetExtensions`
  - `OmnifactotumTypeExtensions`
  - `OmnifactotumUriExtensions`
  - `ValueRange`
  - `ValueRange<T>`
- Improved annotations in:
  - `ComparableValueCapsule<T>`
  - `EnumFactotum`
  - `EquatableValueCapsule<T>`
  - `Factotum`
  - `OmnifactotumArrayExtensions`
  - `OmnifactotumDictionaryExtensions`
  - `OmnifactotumEnumExtensions`
  - `OmnifactotumExpressionExtensions`
  - `OmnifactotumHashCodeHelper`
  - `OmnifactotumMathExtensions`
  - `OmnifactotumMethodBaseExtensions`
  - `OmnifactotumNullableBooleanExtensions`
  - `OmnifactotumOperationContextExtensions`
  - `OmnifactotumTypeExtensions`
  - `ReadOnlySet<T>`
  - `ValueRange`
  - `ValueRange<T>`
- Minor optimizations in
  - `Factotum.SetDefaultValues<T>()`
  - `OmnifactotumAssemblyExtensions.GetLocalPath()`

---

### Changes in 0.8.0 (since 0.7.0)

#### New features

- Added `TemplatedStringResolver` (a templated string is defined in a way similar to C# interpolated string)

---

### Changes in 0.7.0 (since 0.6.0)

#### Breaking changes

- **Removed** support of **.NET Framework 4.0**
- `OmnifactotumGenericObjectExtensions`: Removed the `Morph()` methods (null propagation can be used instead)
- Removed the `AsyncFactotum` class in favor of `async`/`await`
- The `OmnifactotumCollectionExtensions.DoForEachAsync()` overloads now accept `CancellationToken` and pass it to `actionAsync`

#### Deprecations

- `Factotum.CreateEmptyCompletedTask()` (use `Task.CompletedTask`)
- `Factotum.CreateEmptyFaultedTask()` (use `Task.FromException(Exception)`)

#### New features

- Extension Methods
  - `OmnifactotumNullableTimeSpanExtensions`:
    - Added the `ToFixedString()` method
    - Added the `ToFixedStringWithMilliseconds()` method
    - Added the `ToPreciseFixedString()` method
  - `OmnifactotumTimeSpanExtensions`:
    - Added the `ToFixedString()` method
    - Added the `ToFixedStringWithMilliseconds()` method
    - Added the `ToPreciseFixedString()` method
  - `OmnifactotumUriExtensions`:
    - Added the `EnsureAbsoluteUri()` method
    - Added the `EnsureWebUri()` method
- Internally added `System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute` and `System.Diagnostics.CodeAnalysis.NotNullWhenAttribute` to make these attributes available when targeting older frameworks (prior to .NET Standard 2.1 and .NET 5.0). Affected methods:
  - `OmnifactotumGenericObjectExtensions`
    - `EnsureNotNull<T>()`
  - `OmnifactotumSecureStringExtensions`
    - `IsNullOrEmpty()`
    - `ToPlainText()`
  - `OmnifactotumStringExtensions`
    - `IsNullOrEmpty()`
    - `IsNullOrWhiteSpace()`
    - `IsWebUri`
    - `ToSecureString()`
  - `OmnifactotumUriExtensions`
    - `EnsureAbsoluteUri()`
    - `EnsureWebUri()`
    - `IsWebUri`

#### Minor updates and fixes

- `OmnifactotumGenericObjectExtensions`:
  - Fixed nullability annotations on the `ToPropertyString()` methods
  - Fixed documentation on the `EnsureNotNull()` method
- `OmnifactotumSecureStringExtensions`: Fixed `ContractAnnotation` on the `IsNullOrEmpty()` method
- `OmnifactotumStringExtensions`: Fixed `ContractAnnotation` on the `IsNullOrEmpty()` and `IsNullOrWhiteSpace()` methods
- Applied missing `InstantHandle` annotation on `ObjectValidationResult.GetException(...)`
- Improvements in `OmnifactotumDictionaryExtensions`:
  - Using Nullable Reference Types (where applicable)
  - Applied `AggressiveInlining` (where applicable)
