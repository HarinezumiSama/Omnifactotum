# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.14.0 (since 0.13.0)

#### Breaking Changes

- Dropped support of **.NET Framework 4.7.2** and **.NET Standard 2.0**
- **.NET 7+**: Removed the following extension methods since the analogous ones are available since .NET 7:
  - `OmnifactotumCollectionExtensions`
    - `AsReadOnly<T>(this IList<T>)`
  - `OmnifactotumDictionaryExtensions`
    - `AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue>)`

#### New features

- Implemented compiling package for .NET 7
