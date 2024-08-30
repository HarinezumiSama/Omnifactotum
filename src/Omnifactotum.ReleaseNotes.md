### Changes in 0.21.0 (since 0.20.0)

#### New features

- Added `OmnifactotumStringBuilderExtensions` with `StringBuilder AppendUIString(this StringBuilder, string?)` and `StringBuilder AppendSecuredUIString(this StringBuilder, string?, int, int)`
- `ObjectValidator`: Added `EnsureValid<T>(...)` method (shortcut for `ObjectValidator.Validate(...).EnsureSucceeded()`)
- Added `KeyedComparer<T, TKey>` (implements `IComparer<T>` and `IComparer`)
- Added `OmnifactotumNullableCharExtensions` with `ToUIString(this char? value)` method
- `OmnifactotumStringExtensions`: Added
  - `ToTitleCase(this string?, CultureInfo?)`
  - `ToTitleCaseForced(this string?, CultureInfo?)`
  - `ToTitleCaseInvariant(this string?)`
  - `ToTitleCaseInvariantForced(this string?)`
- `OmnifactotumExceptionExtensions`: Added `EnumerateRecursively(this Exception?)` extension method

#### Updates and fixes

- Applied `MeansImplicitUse` annotation to `TMemberConstraint` in `MemberConstraintAttribute<TMemberConstraint>`
- Optimized `OmnifactotumCharExtensions.ToUIString(this char)`
- `IsOriginatedFrom<TOriginatingException>(this Exception?)` and `IsOriginatedFrom(this Exception?, Type)` are now using `OmnifactotumExceptionExtensions.EnumerateRecursively(this Exception?)`

---

### Changes in 0.20.0 (since 0.19.0)

#### Breaking changes

- Object validation
  - Moved validation attributes from the namespace `Omnifactotum.Validation.Constraints` to `Omnifactotum.Validation.Annotations`
    - `BaseMemberConstraintAttribute`
    - `BaseValidatableMemberAttribute`
    - `MemberConstraintAttribute`
    - `MemberConstraintAttribute<T>` (.NET 7+)
    - `MemberItemConstraintAttribute`
    - `MemberItemConstraintAttribute<T>` (.NET 7+)
    - `ValidatableMemberAttribute`
  - `MemberConstraintExtensions`
    - `AddError(this IMemberConstraint, MemberConstraintValidationContext, string?)` -> `AddError(this IMemberConstraint, MemberConstraintValidationContext, ValidationErrorDetails?)`
  - `MemberConstraintBase`
    - `AddError(MemberConstraintValidationContext, string?)` -> `AddError(MemberConstraintValidationContext, ValidationErrorDetails?)`
    - Removed obsolete method `AddError(ObjectValidatorContext, MemberConstraintValidationContext, string)`
    - Removed obsolete method `AddDefaultError(ObjectValidatorContext, MemberConstraintValidationContext)`

#### New features

- Object validation
  - Added `ValidationErrorDetails` with the `Text` and `Description` properties (used in `MemberConstraintBase.AddError()` and `MemberConstraintExtensions.AddError()`)
    - A `string` value can be implicitly converted to `ValidationErrorDetails`
  - Object validation: Added constraints
    - `NotNullAndNotBlankStringConstraint` (replaces `NotBlankStringConstraint`)
    - `NotNullAndNotEmptyCollectionConstraint` (replaces `NotNullOrEmptyCollectionConstraint`)
    - `NotNullAndNotEmptyCollectionConstraint<T>` (replaces `NotNullOrEmptyCollectionConstraint<T>`)
    - `NotNullAndNotEmptyStringConstraint` (replaces `NotNullOrEmptyStringConstraint`)
    - `NotNullRegexStringConstraintBase` (replaces `RegexStringConstraintBase`)
    - `NotNullWebUrlConstraint` (replaces `WebUrlConstraint`)
    - `OptionalNotBlankStringConstraint`
    - `OptionalNotEmptyCollectionConstraint`
    - `OptionalNotEmptyCollectionConstraint<T>`
    - `OptionalNotEmptyStringConstraint`
    - `OptionalRegexStringConstraintBase`
    - `OptionalWebUrlConstraint`

#### Deprecations

- Object validation
  - `MemberConstraintValidationError`
    - The `ErrorMessage` property is deprecated in favor of the `Details` property of type `ValidationErrorDetails` (`ErrorMessage` is equivalent to `Details.Text`)
  - Deprecated constraints
    - `NotBlankStringConstraint` in favor of `NotNullAndNotBlankStringConstraint`
    - `NotNullOrEmptyCollectionConstraint` in favor of `NotNullAndNotEmptyCollectionConstraint`
    - `NotNullOrEmptyCollectionConstraint<T>` in favor of `NotNullAndNotEmptyCollectionConstraint<T>`
    - `NotNullOrEmptyStringConstraint` in favor of `NotNullAndNotEmptyStringConstraint`
    - `RegexStringConstraintBase` in favor of `NotNullRegexStringConstraintBase`
    - `WebUrlConstraint` in favor of `NotNullWebUrlConstraint`

---

### Changes in 0.19.0 (since 0.18.0)

#### Breaking changes

- Object validation
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

- Object validation
  - Added generic `MemberConstraintAttribute<TMemberConstraint>` and `MemberItemConstraintAttribute<TMemberConstraint>` (.NET 7+)
  - `ObjectValidationResult`: Added the `FailureMessage` property (and used it in `GetException()`)
- `OmnifactotumCollectionExtensions`: Added `ToUIString()` for `IEnumerable<KeyValuePair<string, string?>>?`

#### Updates and fixes

- Object validation
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
