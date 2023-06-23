# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

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
