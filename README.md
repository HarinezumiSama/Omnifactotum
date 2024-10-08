﻿# Omnifactotum

`Omnifactotum` is the ultimate solution for **.NET developers** who want to streamline their development process. It provides its own **helper** and **functional** classes and interfaces as well as the **extension methods** for the standard .NET types. `Omnifactotum` is compatible with the older and newer .NET versions. It's the perfect way to reduce errors and save time, allowing developers to focus on creating quality code.

`Factotum` is a Latin word literally meaning "*do everything*", that is, a *handyman* or *Jack-of-all-trades*.

### Release Notes

- [Release Notes](./src/Omnifactotum.ReleaseNotes.md)

### Build Status
| Branch    | Status                                                                                                                                                                                        |
|:----------|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `master`  | [![Build status (master)](https://ci.appveyor.com/api/projects/status/8kcys4vgvk1cd1gg/branch/master?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/omnifactotum/branch/master)    |
| `develop` | [![Build status (develop)](https://ci.appveyor.com/api/projects/status/8kcys4vgvk1cd1gg/branch/develop?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/omnifactotum/branch/develop) |

### Statistics

- NuGet package
  - [![NuGet package version](https://img.shields.io/nuget/v/Omnifactotum.svg) ![NuGet package downloads](https://img.shields.io/nuget/dt/Omnifactotum.svg)](https://www.nuget.org/packages/Omnifactotum/) ![Libraries.io SourceRank](https://img.shields.io/librariesio/sourcerank/nuget/Omnifactotum) ![Dependents (via libraries.io)](https://img.shields.io/librariesio/dependents/nuget/Omnifactotum)

- GitHub
  - ![GitHub Release](https://img.shields.io/github/v/release/HarinezumiSama/Omnifactotum)
     ![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/HarinezumiSama/Omnifactotum/latest) ![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/HarinezumiSama/Omnifactotum/total)
  - [![GitHub](https://img.shields.io/github/license/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/blob/master/LICENSE) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/HarinezumiSama/Omnifactotum) ![GitHub language count](https://img.shields.io/github/languages/count/HarinezumiSama/Omnifactotum) ![GitHub top language](https://img.shields.io/github/languages/top/HarinezumiSama/Omnifactotum) ![GitHub repo size](https://img.shields.io/github/repo-size/HarinezumiSama/Omnifactotum)
  - [![GitHub open issues](https://img.shields.io/github/issues-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/issues?q=is%3Aissue+is%3Aopen) [![GitHub open pull requests](https://img.shields.io/github/issues-pr-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/pulls?q=is%3Apr+is%3Aopen)

### The *non-exhaustive* list of .NET classes covered

- Reflection
  - [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly) extension methods
  - [**`ICustomAttributeProvider`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.icustomattributeprovider) extension methods (`ICustomAttributeProvider` is implemented by [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly), [**`MethodInfo`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo), [**`PropertyInfo`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo), [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/system.type) etc.)
  - [**`MethodBase`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodbase) extension methods
  - [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/system.type) extension methods
- Array (**`T[]`**) and collection ([**`IEnumerable<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) and [**`ICollection<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1)) extension methods
- Arbitrary [**`Object`**](https://docs.microsoft.com/en-us/dotnet/api/system.object) generic extension methods
- [**`Boolean?`**](https://docs.microsoft.com/en-us/dotnet/api/system.boolean) ([`Nullable<Boolean>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)) extension methods
- [**`Char`**](https://docs.microsoft.com/en-us/dotnet/api/system.char) extension methods
- [**`Char?`**](https://docs.microsoft.com/en-us/dotnet/api/system.char) ([`Nullable<Char>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)) extension methods
- [**`DateTime`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetime) extension methods
- [**`DateTime?`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetime) ([`Nullable<DateTime>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)) extension methods
- [**`DateTimeOffset`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset) extension methods
- [**`DateTimeOffset?`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset) ([`Nullable<DateTimeOffset>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)) extension methods
- [**`Delegate`**](https://docs.microsoft.com/en-us/dotnet/api/system.delegate) extension methods
- [**`Enum`**](https://docs.microsoft.com/en-us/dotnet/api/system.enum) extension methods
- [**`Exception`**](https://docs.microsoft.com/en-us/dotnet/api/system.exception) extension methods
- [**`Expression<TDelegate>`**](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression-1) extension methods
- [**`HttpStatusCode`**](https://docs.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode) extension methods
- [**`IDictionary<TKey, TValue>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2) extension methods
- [**`IDisposable`**](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable) extension methods
- [**`IEqualityComparer<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1) extension methods
- [**`ImmutableArray<T>`**](https://learn.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutablearray-1) extension methods
- [**`ISet<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iset-1) extension methods
- Math extension methods for numeric types
- [**`ReadOnlySpan<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.readonlyspan-1) extension methods
- [**`SecureString`**](https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring) extension methods
- [**`Span<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.span-1) extension methods
- [**`Stopwatch`**](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch) extension methods
- [**`String`**](https://docs.microsoft.com/en-us/dotnet/api/system.string) extension methods
- [**`StringBuilder`**](https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder) extension methods
- [**`Task`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) extension methods
- [**`Task<TResult>`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) extension methods
- [**`TimeSpan`**](https://docs.microsoft.com/en-us/dotnet/api/system.timespan) extension methods
- [**`TimeSpan?`**](https://docs.microsoft.com/en-us/dotnet/api/system.timespan) ([`Nullable<TimeSpan>`](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1)) extension methods
- [**`Uri`**](https://docs.microsoft.com/en-us/dotnet/api/system.uri) extension methods
- [**`ValueTask`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) extension methods
- [**`ValueTask<TResult>`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1) extension methods

### The *non-exhaustive* list of own helper classes

- `EnumFactotum`
- `Factotum`

### The *non-exhaustive* list of own functional classes

- `ByReferenceEqualityComparer<T>`
- `CaseInsensitiveStringKey`
- `ColoredConsoleTraceListener`
- `DirectedGraph<T>` and `DirectedGraphNode<T>`
- `EnumFixedSizeDictionary<TKey, TValue>`
- `FixedSizeDictionary<TKey, TValue>`
- `KeyedComparer<T, TKey>`
- `KeyedEqualityComparer<T, TKey>`
- `LocalComputerCurrentDateTimeProvider` (implements `ICurrentDateTimeProvider`)
- `ObjectValidator`
- `ReadOnlyItemCollection<T>` (the read-only wrapper for [`ICollection<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1))
- `ReadOnlySet<T>`
- `SemaphoreSlimBasedLock`
- `StopwatchElapsedTimeProvider` (implements `IElapsedTimeProvider`)
- `SyncValueContainer<T>` (implements `IValueContainer<T>`)
- `TemplatedStringResolver`
- `ValueContainer<T>` (implements `IValueContainer<T>`)
- `ValueRange<T>`
- `VirtualTreeNode<T>` and `VirtualTreeNodeRoot<T>`
- `WeakReferenceBasedCache<TKey, TValue>`

### The *non-exhaustive* list of own abstractions (interfaces)

- `ICurrentDateTimeProvider`
- `IElapsedTimeProvider`
- `IValueContainer<T>`

### The *non-exhaustive* list of own delegates

- `Task EventHandlerAsync<>(...)`
- `OutFunc<TOutput, out TResult>`
- `OutFunc<in T, TOutput, out TResult>`
- `OutFunc<in T1, in T2, TOutput, out TResult>`
- `OutFunc<in T1, in T2, in T3, TOutput, out TResult>`
