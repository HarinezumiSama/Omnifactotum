# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)

### Changes in 0.15.0 (since [0.14.1](https://www.nuget.org/packages/Omnifactotum/0.14.1))

#### Breaking Changes

- `ObjectValidator.Validate()` method: Added `instanceExpression` parameter 
  - For .NET 5+ and higher, the `instanceExpression` parameter is marked with the `CallerArgumentExpression` attribute
  - For the older .NET versions, the `instanceExpression` parameter is supplied only for binary compatibility between the different target frameworks

#### New features

- `ElapsedTimeProviderExtensions`: Added the `GetStoppedElapsed()` extension method
- `OmnifactotumCollectionExtensions`: Added the `Flatten()` extension method
- `OmnifactotumEnumExtensions`: Added the `ToUIString()` extension method
- `OmnifactotumExceptionExtensions`: Added the `IsOriginatedFrom(this Exception?, Type)` extension method
- Added `OmnifactotumStopwatchExtensions` with the `GetStoppedElapsed()` extension method
- `OmnifactotumStringExtensions`: Added new extension methods for `System.String`:
  - `WithSingleLeadingSlash(string)`
  - `WithoutLeadingSlash(string)`
- Object validation
  - Added `RegexStringConstraintBase`
  - Added `ValueRangeConstraintBase<T>`

#### Minor updates and fixes

- Applied `MeansImplicitUse` annotations to `TKeyConstraint` and `TValueConstraint` in `KeyValuePairConstraint<TKey, TValue, TKeyConstraint, TValueConstraint>`
