# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

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
