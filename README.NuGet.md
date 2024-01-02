# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

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
