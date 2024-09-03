# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

---

### Changes in 0.22.0 (since 0.21.0)

#### Breaking changes

- `OmnifactotumStringBuilderExtensions`: Fixed namespace: `System` -> `System.Text`

---

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
