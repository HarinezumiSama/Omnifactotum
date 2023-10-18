# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

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
