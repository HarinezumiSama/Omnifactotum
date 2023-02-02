# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.12.0 (since 0.11.0)

#### Breaking Changes

- Removed support of **.NET Framework 4.6.1**

#### New features

- `TemplatedStringResolver`: Added the static `GetVariableNames()` method
- `Factotum`: Added the `Assert(...)` method
  - For .NET 5+ and higher, the `conditionExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `conditionExpression` parameter is supplied only for binary compatibility between the different target frameworks

#### Minor updates and fixes

- Minor improvements in `OmnifactotumHttpStatusCodeExtensions.ToUIString()`
- Improved documentation on `AssertionMethodAttribute` and `AssertionConditionAttribute`
