# Omnifactotum
Provides own *helper* and *functional* classes as well as *extension methods* for standard .NET classes.

**Note**: `Factotum` is a Latin word literally meaning "do everything" (that is, a *handyman* or *Jack-of-all-trades*).

### Status and Statistics
- [![Build status](https://ci.appveyor.com/api/projects/status/8kcys4vgvk1cd1gg?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/omnifactotum)

- [![NuGet package version](https://img.shields.io/nuget/v/Omnifactotum.svg) ![NuGet package downloads](https://img.shields.io/nuget/dt/Omnifactotum.svg)](https://www.nuget.org/packages/Omnifactotum/) [![Omnifactotum on fuget.org](https://www.fuget.org/packages/Omnifactotum/badge.svg)](https://www.fuget.org/packages/Omnifactotum) ![Libraries.io SourceRank](https://img.shields.io/librariesio/sourcerank/nuget/Omnifactotum) ![Dependents (via libraries.io)](https://img.shields.io/librariesio/dependents/nuget/Omnifactotum)

- ![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/HarinezumiSama/Omnifactotum/latest)

- [![GitHub](https://img.shields.io/github/license/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/blob/master/LICENSE) ![Total lines of code](https://img.shields.io/tokei/lines/github/HarinezumiSama/Omnifactotum) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/HarinezumiSama/Omnifactotum) ![GitHub language count](https://img.shields.io/github/languages/count/HarinezumiSama/Omnifactotum) ![GitHub top language](https://img.shields.io/github/languages/top/HarinezumiSama/Omnifactotum) ![GitHub repo size](https://img.shields.io/github/repo-size/HarinezumiSama/Omnifactotum)

- [![GitHub open issues](https://img.shields.io/github/issues-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/issues?q=is%3Aissue+is%3Aopen) [![GitHub open pull requests](https://img.shields.io/github/issues-pr-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/pulls?q=is%3Apr+is%3Aopen)

### The *non-exaustive* list of .NET classes covered

- Reflection
  - [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/?term=system.reflection.assembly) extension methods
  - [**`ICustomAttributeProvider`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Reflection.ICustomAttributeProvider) extension methods (`ICustomAttributeProvider` is implemented by [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Reflection.Assembly), [**`MethodInfo`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Reflection.MethodInfo), [**`PropertyInfo`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Reflection.PropertyInfo), [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Type) etc.)
  - [**`MethodBase`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Reflection.MethodBase) extension methods
  - [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Type) extension methods
- Array (**`T[]`**) and collection ([**`IEnumerable<T>`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Collections.Generic.IEnumerable%3CT%3E) and [**`ICollection<T>`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Collections.Generic.ICollection%3CT%3E)) extension methods
- [**`DateTime`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.DateTime) extension methods
- [**`DateTimeOffset`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.DateTimeOffset) extension methods
- [**`Dictionary<TKey, TValue>`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Collections.Generic.Dictionary%3CTKey%2CTValue%3E) extension methods
- [**`IDisposable`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.IDisposable) extension methods
- [**`Enum`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Enum) extension methods
- [**`Exception`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Exception) extension methods
- [**`Expression<TDelegate>`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Linq.Expressions.Expression%3CTDelegate%3E) extension methods
- Arbitrary [**`Object`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Object) generic extension methods
- Math extension methods for numeric types
- [**`Nullable<Boolean> (bool?)`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Nullable%3CT%3E) extension methods
- [**`ISet`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.Collections.Generic.ISet%3CT%3E) extension methods
- [**`String`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.String) extension methods
- [**`TimeSpan`**](https://docs.microsoft.com/en-us/dotnet/api/?term=System.TimeSpan) extension methods

### The *non-exaustive* list of own helper classes

- `AsyncFactotum`
- `EnumFactotum`
- `Factotum`

### The *non-exaustive* list of own functional classes

- `ByReferenceEqualityComparer<T>`
- `ColoredConsoleTraceListener`
- `ObjectValidator`
- `DirectedGraph<T>` and `DirectedGraphNode<T>`
- `EnumFixedSizeDictionary<TKey, TValue>`
- `FixedSizeDictionary<TKey, TValue>`
- `KeyedEqualityComparer<T, TKey>`
- `ReadOnlyDictionary<TKey, TValue>` <sub><sup>(.NET Framework 4.0 target only; [`System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.readonlydictionary-2?view=net-5.0) is available as of .NET Framework 4.5+)</sup></sub>
- `ReadOnlySet<T>`
- `ValueRange<T>`
- `VirtualTreeNode<T>` and `VirtualTreeNodeRoot<T>`
- `WeakReferenceBasedCache<TKey, TValue>`