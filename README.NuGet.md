# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.13.0 (since 0.12.0)

#### Breaking Changes

- `OmnifactotumEnumExtensions`: `EnsureDefined<TEnum>(this TEnum ...)` now returns the input value instead of `void`
- `TemplatedStringResolver`: `GetVariableNames()` now returns `HashSet<string>` instead of `string[]`

#### New features

- `OmnifactotumExceptionExtensions`: Added the `IsOriginatedFrom<TOriginatingException>(this Exception?)` extension method
- `OmnifactotumStringExtensions`: Added the `ToSecuredUIString(this string? ...)` extension method

#### Minor updates and fixes

- `TemplatedStringResolver`: `GetVariableNames()` now uses the same resolver function for the variable name comparer as in the `TemplatedStringResolver` constructor
