# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### More details

- [Complete Release Notes](https://github.com/HarinezumiSama/Omnifactotum/blob/master/src/Omnifactotum.ReleaseNotes.md)
- [ReadMe](https://github.com/HarinezumiSama/Omnifactotum/blob/master/README.md)
- [Omnifactotum Compiler Extensions](https://github.com/HarinezumiSama/Omnifactotum/blob/master/doc/CompilerExtensions/README.md)

---

### Changes in 0.25.0 (since 0.24.0)

#### Breaking changes

- **.NET 10+**: Removed `System.Collections.Generic.OmnifactotumSetExtensions.AsReadOnly<T>()` since .NET 10+ has `System.Collections.Generic.CollectionExtensions.AsReadOnly<T>(ISet<T>)`

#### New features

- Added support for the **.NET 10** target framework
- **Omnifactotum Compiler Extensions**
  - Analyzers for the object validation attributes (`MemberConstraintAttribute`, `MemberConstraintAttribute<TMemberConstraint>`, `MemberItemConstraintAttribute`, and `MemberItemConstraintAttribute<TMemberConstraint>`):
    - OFCA0011: Validation constraint type does not implement the required interface `IMemberConstraint`
    - OFCA0012: Validation constraint type does not have a required parameterless constructor
    - OFCA0013: Validation attribute can be replaced with its generic equivalent
    - OFCA0014: Validation constraint type is not compatible with the type of the validated value
  - Code fixers for:
    - OFCA0013: Validation attribute can be replaced with its generic equivalent
- Added the package icon
