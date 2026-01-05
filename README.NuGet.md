# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)
- [Omnifactotum Analyzers](https://github.com/HarinezumiSama/Omnifactotum/blob/master/doc/Analyzers/README.md)

---

### Changes in 0.24.0 (since 0.23.0)

#### Breaking changes

- `CaseInsensitiveString`: Now a null `string` corresponds to a null `CaseInsensitiveString` and the underlying string value cannot be null
- Newly added **Omnifactotum Compiler Extensions** may break your code if it does not comply with the added analyzer rules (for more details, refer to the _New features_ section)

#### New features

- Added **Omnifactotum Compiler Extensions**
  - Analyzers:
    - OFCA0001: Asynchronous method/function lacks 'Async' suffix
    - OFCA0002: Synchronous method/function has 'Async' suffix
    - OFCA0003: Asynchronous method/function lacks 'CancellationToken' parameter
  - Code fixers for:
    - OFCA0001: Asynchronous method/function lacks 'Async' suffix
    - OFCA0002: Synchronous method/function has 'Async' suffix
- Reinstated support of:
  - `.NET Framework 4.6.1`
  - `.NET Framework 4.7.2`
  - `.NET Standard 2.0`
- `CaseInsensitiveString`
  - Added the `Empty` static field (corresponds to `string.Empty`)
- `OmnifactotumStringBuilderExtensions`:
  - Added `AppendWhiteSpace(this StringBuilder)`
  - Added `AppendWhiteSpaces(this StringBuilder, int)`
- `OmnifactotumValueTupleExtensions`:
  - Added `ToValueRange<T>(this ValueTuple<T, T>)`
- `ValueRangeExtensions`:
  - Added `GetMidpoint<T>(this ValueRange<T>)` (.NET 7+)
  - Added `ToValueTuple<T>(this ValueRange<T>)`

#### Updates and fixes

- Object validation
  - Fixed retrieving `Count` of supported collections (the previous approach did not work particularly for `ObservableCollection<T>`)
