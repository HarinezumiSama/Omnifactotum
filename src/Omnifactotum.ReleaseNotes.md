### Changes in 0.17.0 (since 0.16.0)

#### Breaking Changes

#### New features

- Added `OmnifactotumCharExtensions`
  - `ToUIString(this char value)`
- Added `OmnifactotumSpanExtensions`
  - `ToHexString(this Span<byte> bytes, ...)`
  - `TransformMultilineString(...)`
- Added `ValueRangeExtensions`
  - `Enumerate<T>(...)`
  - `ToArray<T>(...)`
- `OmnifactotumReadOnlySpanExtensions`
  - Added `TransformMultilineString(...)`
- `OmnifactotumStringExtensions`
  - Added `EnsureNotBlank<T>`
  - Added `EnsureNotEmpty<T>`
  - Added `TransformMultilineString(...)`
- Validation
  - Added `NullableValueRangeConstraintBase<T>` constraint
  - Added `OptionalEnumValueDefinedConstraint<T>` constraint
  - Added `OptionalValueRangeConstraintBase<T>` constraint

#### Minor updates and fixes

- Applied `System.Diagnostics.Contracts.PureAttribute`, `Omnifactotum.Annotations.Pure`, and/or `Omnifactotum.Annotations.MustUseReturnValueAttribute` annotations where reasonable
- Applied `Omnifactotum.Annotations.NotNullAttribute` where reasonable
- Fixed/improved XML-documentation in:
  - `NullableEnumValueDefinedConstraint<TEnum>`
  - `OmnifactotumStringExtensions`
- Minor code style fixes/improvements

### Changes in 0.16.0 (since 0.15.0)

#### Breaking Changes

- `OmnifactotumArrayExtensions`: Removed `ToHexString(this byte[], bool)` and `ToHexString(this byte[])` in favor of `ToHexString(this byte[] bytes, string? separator = null, bool upperCase = false)`

#### New features

- `MemberConstraintBase`: Added the static protected method `string FormatValue<TValue>(TValue value)` (used in `EnumValueDefinedConstraint<TEnum>`, `NullableEnumValueDefinedConstraint<TEnum>`, `RegexStringConstraintBase`, `ValueRangeConstraintBase<T>`, and `WebUrlConstraint` to format the invalid value and valid value(s))
- `OmnifactotumEnumExtensions`
  - Added the `string GetDescription<TEnum>(this TEnum)` extension method
  - Added the `ulong ToUInt64<TEnum>(this TEnum)` extension method
- `OmnifactotumImmutableArrayExtensions`
  - Added the `AvoidNullOrDefault<T>(this ImmutableArray<T>?)` extension method
- Added `OmnifactotumReadOnlySpanExtensions` with the `ToHexString(this ReadOnlySpan<byte> bytes, string? separator = null, bool useUpperCase = false)` extension method
- `ValueRange<T>`
  - Added the `string ToString(string boundarySeparator)` method
  - .NET 7+: `ValueRange<T>` implements `IEqualityOperators<ValueRange<T>, ValueRange<T>, bool>`
- `ValueRangeConstraintBase<T>`: Added constructor `ValueRangeConstraintBase(T lower, T upper)`

#### Minor updates and fixes

- `OmnifactotumArrayExtensions`
  - `ToHexString(this byte[], string?, bool)` is now optimized compared to the older implementation (less heap allocations)
- `EnumFactotum`: Fix in `GetAllFlagValues<TEnum>()`
- `ValueRange<T>`
  - `string ToString()`: Changed the result format from `[Lower; Upper]` to `[Lower ~ Upper]`
- `ValueRangeConstraintBase<T>`: Included invalid value in the error message

### Changes in 0.15.0 (since 0.14.1)

#### Breaking Changes

- `ObjectValidator.Validate()` method: Added `instanceExpression` parameter
  - For .NET 5+ and higher, the `instanceExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `instanceExpression` parameter is supplied only for binary compatibility between the different target frameworks

#### New features

- `ElapsedTimeProviderExtensions`: Added the `GetStoppedElapsed()` extension method
- `OmnifactotumCollectionExtensions`: Added the `Flatten()` extension method
- `OmnifactotumEnumExtensions`: Added the `ToUIString()` extension method
- `OmnifactotumExceptionExtensions`: Added the `IsOriginatedFrom(this Exception?, Type)` extension method
- Added `OmnifactotumStopwatchExtensions` with the `GetStoppedElapsed()` extension method
- `OmnifactotumStringExtensions`: Added new extension methods for `System.String`:
  - `WithSingleLeadingSlash(string)`
  - `WithoutLeadingSlash(string)`
- Object validation
  - Added `RegexStringConstraintBase`
  - Added `ValueRangeConstraintBase<T>`

#### Minor updates and fixes

- Applied `MeansImplicitUse` annotations to `TKeyConstraint` and `TValueConstraint` in `KeyValuePairConstraint<TKey, TValue, TKeyConstraint, TValueConstraint>`

### Changes in 0.14.1 (since 0.14.0)

#### Minor updates and fixes

- Improvements in `Factotum.Assert(...)`
  - The `DoesNotReturnIf` attribute has been applied on the `condition` parameter
  - Now the method referenced by the `createAssertionFailureException` parameter can return `null` (`OmnifactotumAssertionException` is used in this case)
- Minor improvements in XML documentation in `OmnifactotumMethodBaseExtensions` and `OmnifactotumTypeExtensions`

---

### Changes in 0.14.0 (since 0.13.0)

#### Breaking Changes

- Dropped support of **.NET Framework 4.7.2** and **.NET Standard 2.0**
- **.NET 7+**: Removed the following extension methods since the analogous ones are available since .NET 7:
  - `OmnifactotumCollectionExtensions`
    - `AsReadOnly<T>(this IList<T>)`
  - `OmnifactotumDictionaryExtensions`
    - `AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue>)`

#### New features

- Implemented compiling package for .NET 7

---

### Changes in 0.13.0 (since 0.12.0)

#### Breaking Changes

- `OmnifactotumEnumExtensions`: `EnsureDefined<TEnum>(this TEnum ...)` now returns the input value instead of `void`
- `TemplatedStringResolver`: `GetVariableNames()` now returns `HashSet<string>` instead of `string[]`

#### New features

- `OmnifactotumExceptionExtensions`: Added the `IsOriginatedFrom<TOriginatingException>(this Exception?)` extension method
- `OmnifactotumStringExtensions`: Added the `ToSecuredUIString(this string? ...)` extension method

#### Minor updates and fixes

- `TemplatedStringResolver`: `GetVariableNames()` now uses the same resolver function for the variable name comparer as in the `TemplatedStringResolver` constructor

---

### Changes in 0.12.0 (since 0.11.0)

#### Breaking Changes

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

---

### Changes in 0.10.0 (since 0.9.0)

#### New features

- Added the `CaseInsensitiveStringKey` structure
- `OmnifactotumDateTimeExtensions`: Added the `AsKind(this DateTime, DateTimeKind)` extension method

---

### Changes in 0.9.0 (since 0.8.0)

#### Breaking Changes

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

#### Breaking Changes

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

---

### Changes in 0.6.0 (since 0.5.0)

#### Breaking Changes

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

#### Breaking Changes

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

#### Breaking Changes

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
