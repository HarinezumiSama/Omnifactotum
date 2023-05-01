# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.14.1 (since 0.14.0)

#### Minor updates and fixes

- Improvements in `Factotum.Assert(...)`
  - Applied the `DoesNotReturnIf` attribute on the `condition` parameter
  - Now the method referenced by the `createAssertionFailureException` parameter can return `null` (`OmnifactotumAssertionException` is used in this case)
- Minor improvements in XML documentation in `OmnifactotumMethodBaseExtensions` and `OmnifactotumTypeExtensions`
