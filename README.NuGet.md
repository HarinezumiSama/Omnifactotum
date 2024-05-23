# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

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
