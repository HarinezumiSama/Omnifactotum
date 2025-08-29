# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

---

### Changes in 0.23.0 (since 0.22.0)

#### Breaking changes

- `MemberConstraintValidationError`: Removed the obsolete property `ErrorMessage`
- Removed obsolete validation constraints
  - `NotBlankStringConstraint` (use `NotNullAndNotBlankStringConstraint` instead)
  - `NotNullOrEmptyCollectionConstraint` (use `NotNullAndNotEmptyCollectionConstraint` instead)
  - `NotNullOrEmptyCollectionConstraint<T>` (use `NotNullAndNotEmptyCollectionConstraint<T>` instead)
  - `NotNullOrEmptyStringConstraint` (use `NotNullAndNotEmptyStringConstraint` instead)
  - `RegexStringConstraintBase` (use `NotNullRegexStringConstraintBase` instead)
  - `WebUrlConstraint` (use `NotNullWebUrlConstraint` instead)

#### New features

- Implemented building the NuGet package for .NET 8 and .NET 9
- Added the `CaseInsensitiveString` structure
- `OmnifactotumArrayExtensions`
  - Added `EmptyIfNull(this T[]?)`
- `OmnifactotumCollectionExtensions`
  - Added `EmptyIfNull(this IEnumerable<T>?)`
  - Added `ReplaceItems<TCollection, T>(this TCollection collection, IEnumerable<T>) where TCollection : ICollection<T>`
- `OmnifactotumImmutableArrayExtensions`
  - Added `EmptyIfDefault(this ImmutableArray<T>)`
  - Added `EmptyIfNullOrDefault<T>(this ImmutableArray<T>?)`
- Added `OmnifactotumKeyValuePairExtensions` with `ToValueTuple<TKey, TValue>()`
- `OmnifactotumReadOnlySpanExtensions`
  - Added `ToUIString(this ReadOnlySpan<char>)`
- `OmnifactotumStringExtensions`
  - Added `EmptyIfNull(this string?)`
- `OmnifactotumTaskExtensions`
  - Added `EnsureNotNullAsync(this Task<T?>)` (.NET 5+)
- `OmnifactotumValueTaskExtensions`
  - Added `EnsureNotNullAsync(this ValueTask<T?>)` (.NET 5+)
- Added `OmnifactotumValueTupleExtensions` with `ToKeyValuePair<TKey, TValue>()` and `ToDictionaryEntry<TKey, TValue>()`

#### Deprecations

- Deprecated the `CaseInsensitiveStringKey` structure in favor of the `CaseInsensitiveString` structure
- `OmnifactotumArrayExtensions`
  - Deprecated `AvoidNull(this T[]?)` in favor of `EmptyIfNull(this T[]?)`
- `OmnifactotumCollectionExtensions`
  - Deprecated `AvoidNull(this IEnumerable<T>?)` in favor of `EmptyIfNull(this IEnumerable<T>?)`
  - Deprecated `SetItems<T>(ICollection<T> collection, IEnumerable<T>)` in favor of `ReplaceItems<TCollection, T>()`
- `OmnifactotumGenericObjectExtensions`
  - Deprecated `AsArray<T>(this T)`
  - Deprecated `AsCollection<T>(this T)`
  - Deprecated `AsList<T>(this T)`
  - Deprecated `AvoidNull<T>(this T?, Func<T>)`
- `OmnifactotumImmutableArrayExtensions`
  - Deprecated `AvoidNull(this ImmutableArray<T>)` in favor of `EmptyIfDefault(this ImmutableArray<T>)`
  - Deprecated `AvoidNullOrDefault<T>(this ImmutableArray<T>?)` in favor of `EmptyIfNullOrDefault<T>(this ImmutableArray<T>?)`
- `OmnifactotumStringExtensions`
  - Deprecated `AvoidNull(this string?)` in favor of `EmptyIfNull(this string?)`

#### Updates and fixes

- Documentation fix in `OmnifactotumStringExtensions.TrimPrefix()`
- Performance improvements in `OmnifactotumStringExtensions.ToUIString()` and `OmnifactotumStringBuilderExtensions.AppendUIString()`
- `MemberConstraintBase.FormatValue<TValue>()`: Fixed formatting of a known enumeration value
- `OmnifactotumCollectionExtensions.FindDuplicates<T,TKey>(...)`: Minor refactoring/optimization
