### Changes in 0.19.0 (since 0.18.0)

#### Breaking changes

- Object validation:
  - Removed `ValidationErrorCollection`
  - `ObjectValidatorContext`: Removed the `Errors` property from public API
  - Removed deprecated methods in `OmnifactotumTypeExtensions`:
    - `IsNullable()` (`IsNullableValueType()` to be used instead)
    - `GetCollectionElementType()` (`GetCollectionElementTypeOrDefault()` to be used instead)
  - `IMemberConstraint`: Replaced `Validate(ObjectValidatorContext, MemberConstraintValidationContext, object?)` with `Validate(MemberConstraintValidationContext, object?)` since `MemberConstraintValidationContext` now has a reference to `ObjectValidatorContext`
  - `MemberConstraintBase`:
    - Replaced `ValidateValue(ObjectValidatorContext, MemberConstraintValidationContext, object?)` with `ValidateValue(MemberConstraintValidationContext, object?)`
  - `TypedMemberConstraintBase<T>`:
    - Replaced `ValidateTypedValue(ObjectValidatorContext, MemberConstraintValidationContext, T value)` with `ValidateTypedValue(MemberConstraintValidationContext, T value)`
    - Replaced `ValidateMember<TMember>(ObjectValidatorContext, MemberConstraintValidationContext, T, Expression<Func<T, TMember>>, Type)` with `ValidateMember<TMember>(MemberConstraintValidationContext, T, Expression<Func<T, TMember>>, Type)`

#### New features

- Object validation:
  - Added generic `MemberConstraintAttribute<TMemberConstraint>` and `MemberItemConstraintAttribute<TMemberConstraint>` (.NET 7+)
  - `ObjectValidationResult`: Added the `FailureMessage` property (and used it in `GetException()`)
- `OmnifactotumCollectionExtensions`: Added `ToUIString()` for `IEnumerable<KeyValuePair<string, string?>>?`

#### Updates and fixes

- Object validation:
  - Improved the failure message in `NotNullConstraint<T>` (included the `T` type name)
  - `MemberConstraintValidationContext`: Added a reference to `ObjectValidatorContext`
- `OmnifactotumCollectionExtensions` and `OmnifactotumStringExtensions`: Implemented safe processing of collections w.r.t. `ImmutableArray<T>`
- Applied `DebuggerDisplay` annotation:
  - `FixedSizeDictionary<TKey, TValue, TDeterminant>`
  - `ReadOnlyItemCollection<T>`
  - `ReadOnlySet<T>`
  - `RecursiveProcessingContext`
  - `SemaphoreSlimBasedLock`
  - `StopwatchElapsedTimeProvider`
  - `TemplatedStringResolver`
  - Validation:
    - `MemberConstraintValidationContext`
    - `ObjectValidationException`
    - `ObjectValidationResult`
    - `ObjectValidatorContext`
    - `ValidationErrorCollection`

---

### Changes in 0.18.0 (since 0.17.0)

#### Breaking changes

- Object Validation
  - `NotNullConstraint` is now inherited from `MemberConstraintBase` instead of `NotNullConstraint<object>`
  - `NotNullConstraint<T>` is now sealed
  - Removed `Omnifactotum.Validation.ObjectValidationResult.GetException(Func<...>, string?)`
  - Removed `Omnifactotum.Validation.Constraints.MemberConstraintValidationError.GetDefaultDescription()`
  - Removed `Omnifactotum.Validation.Constraints.MemberConstraintValidationError.GetDefaultDescription(MemberConstraintValidationError)`
  - Member constraint's constructor can now be non-public

#### New features

- Object Validation
  - Implemented support for `ImmutableArray<T>` in member constraints:
    - `NotNullConstraint`
    - `NotNullConstraint<T>`
    - `NotNullOrEmptyCollectionConstraint`
    - `NotNullOrEmptyCollectionConstraint<T>`
- `OmnifactotumTypeExtensions`
  - Added `GetInterfaceMethodImplementation(this Type, MethodInfo)`

#### Updates and fixes

- Object Validation
  - Improved/added support for `ImmutableArray<T>`, `IReadOnlyList<T>`, `IList<T>`, `IEnumerable<T>`, and `IList`
  - Improved type casting in expressions
  - Improved message format of the exception created by `ObjectValidationResult.GetException()`
  - Validating early that a member constraint has a parameterless constructor
  - Slightly optimized member constraint creation

---

### Changes in 0.17.0 (since 0.16.0)

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

---

### Changes in 0.16.0 (since 0.15.0)

#### Breaking changes

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

---

### Changes in 0.15.0 (since 0.14.1)

#### Breaking changes

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

---

### Changes in 0.14.1 (since 0.14.0)

#### Minor updates and fixes

- Improvements in `Factotum.Assert(...)`
  - The `DoesNotReturnIf` attribute has been applied on the `condition` parameter
  - Now the method referenced by the `createAssertionFailureException` parameter can return `null` (`OmnifactotumAssertionException` is used in this case)
- Minor improvements in XML documentation in `OmnifactotumMethodBaseExtensions` and `OmnifactotumTypeExtensions`

---

### Changes in 0.14.0 (since 0.13.0)

#### Breaking changes

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

#### Breaking changes

- `OmnifactotumEnumExtensions`: `EnsureDefined<TEnum>(this TEnum ...)` now returns the input value instead of `void`
- `TemplatedStringResolver`: `GetVariableNames()` now returns `HashSet<string>` instead of `string[]`

#### New features

- `OmnifactotumExceptionExtensions`: Added the `IsOriginatedFrom<TOriginatingException>(this Exception?)` extension method
- `OmnifactotumStringExtensions`: Added the `ToSecuredUIString(this string? ...)` extension method

#### Minor updates and fixes

- `TemplatedStringResolver`: `GetVariableNames()` now uses the same resolver function for the variable name comparer as in the `TemplatedStringResolver` constructor

---

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
