# Omnifactotum

Provides own **helper** and **functional** classes and interfaces as well as **extension methods** for the standard .NET types.

**Note**: `Factotum` is a Latin word literally meaning "*do everything*" (that is, a *handyman* or *Jack-of-all-trades*).

### Release Notes

- [Release Notes](./src/Omnifactotum.ReleaseNotes.md)

### Build Status
| Branch  | Status  |
| :------ | :------ |
| `master` | [![Build status (master)](https://ci.appveyor.com/api/projects/status/8kcys4vgvk1cd1gg/branch/master?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/omnifactotum/branch/master) |
| `develop` | [![Build status (develop)](https://ci.appveyor.com/api/projects/status/8kcys4vgvk1cd1gg/branch/develop?svg=true)](https://ci.appveyor.com/project/HarinezumiSama/omnifactotum/branch/develop) |

### Statistics

- [![NuGet package version](https://img.shields.io/nuget/v/Omnifactotum.svg) ![NuGet package downloads](https://img.shields.io/nuget/dt/Omnifactotum.svg)](https://www.nuget.org/packages/Omnifactotum/) [![Omnifactotum on fuget.org](https://www.fuget.org/packages/Omnifactotum/badge.svg)](https://www.fuget.org/packages/Omnifactotum) ![Libraries.io SourceRank](https://img.shields.io/librariesio/sourcerank/nuget/Omnifactotum) ![Dependents (via libraries.io)](https://img.shields.io/librariesio/dependents/nuget/Omnifactotum)

- ![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/HarinezumiSama/Omnifactotum/latest)

- [![GitHub](https://img.shields.io/github/license/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/blob/master/LICENSE) ![Total lines of code](https://img.shields.io/tokei/lines/github/HarinezumiSama/Omnifactotum) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/HarinezumiSama/Omnifactotum) ![GitHub language count](https://img.shields.io/github/languages/count/HarinezumiSama/Omnifactotum) ![GitHub top language](https://img.shields.io/github/languages/top/HarinezumiSama/Omnifactotum) ![GitHub repo size](https://img.shields.io/github/repo-size/HarinezumiSama/Omnifactotum)

- [![GitHub open issues](https://img.shields.io/github/issues-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/issues?q=is%3Aissue+is%3Aopen) [![GitHub open pull requests](https://img.shields.io/github/issues-pr-raw/HarinezumiSama/Omnifactotum)](https://github.com/HarinezumiSama/Omnifactotum/pulls?q=is%3Apr+is%3Aopen)

### The *non-exhaustive* list of .NET classes covered

- Reflection
  - [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly) extension methods
  - [**`ICustomAttributeProvider`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.icustomattributeprovider) extension methods (`ICustomAttributeProvider` is implemented by [**`Assembly`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly), [**`MethodInfo`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodinfo), [**`PropertyInfo`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.propertyinfo), [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/system.type) etc.)
  - [**`MethodBase`**](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.methodbase) extension methods
  - [**`Type`**](https://docs.microsoft.com/en-us/dotnet/api/system.type) extension methods
- Array (**`T[]`**) and collection ([**`IEnumerable<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1) and [**`ICollection<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1)) extension methods
- Arbitrary [**`Object`**](https://docs.microsoft.com/en-us/dotnet/api/system.object) generic extension methods
- [**`DateTime`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetime) extension methods
- [**`DateTimeOffset`**](https://docs.microsoft.com/en-us/dotnet/api/system.datetimeoffset) extension methods
- [**`Delegate`**](https://docs.microsoft.com/en-us/dotnet/api/system.delegate) extension methods
- [**`Dictionary<TKey, TValue>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2) extension methods
- [**`Enum`**](https://docs.microsoft.com/en-us/dotnet/api/system.enum) extension methods
- [**`Exception`**](https://docs.microsoft.com/en-us/dotnet/api/system.exception) extension methods
- [**`Expression<TDelegate>`**](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression-1) extension methods
- [**`HttpStatusCode`**](https://docs.microsoft.com/en-us/dotnet/api/system.net.httpstatuscode) extension methods
- [**`IDisposable`**](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable) extension methods
- [**`IEqualityComparer<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iequalitycomparer-1) extension methods
- [**`ISet<T>`**](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.iset-1) extension methods
- Math extension methods for numeric types
- [**`Nullable<Boolean>` (`bool?`)**](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) extension methods
- [**`Nullable<DateTime>` (`DateTime?`)**](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) extension methods
- [**`Nullable<DateTimeOffset>` (`DateTimeOffset?`)**](https://docs.microsoft.com/en-us/dotnet/api/system.nullable-1) extension methods
- [**`SecureString`**](https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring) extension methods
- [**`String`**](https://docs.microsoft.com/en-us/dotnet/api/system.string) extension methods
- [**`Task`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task) extension methods (excluding .NET Framework 4.0)
- [**`Task<TResult>`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1) extension methods (excluding .NET Framework 4.0)
- [**`TimeSpan`**](https://docs.microsoft.com/en-us/dotnet/api/system.timespan) extension methods
- [**`Uri`**](https://docs.microsoft.com/en-us/dotnet/api/system.uri) extension methods
- [**`ValueTask`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask) extension methods (excluding .NET Framework 4.0)
- [**`ValueTask<TResult>`**](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1) extension methods (excluding .NET Framework 4.0)

### The *non-exhaustive* list of own helper classes

- `AsyncFactotum`
- `EnumFactotum`
- `Factotum`

### The *non-exhaustive* list of own functional classes

- `ByReferenceEqualityComparer<T>`
- `ColoredConsoleTraceListener`
- `DirectedGraph<T>` and `DirectedGraphNode<T>`
- `EnumFixedSizeDictionary<TKey, TValue>`
- `FixedSizeDictionary<TKey, TValue>`
- `KeyedEqualityComparer<T, TKey>`
- `LocalComputerCurrentDateTimeProvider` (implements `ICurrentDateTimeProvider`)
- `ObjectValidator`
- `ReadOnlyDictionary<TKey, TValue>`
  - **NOTE**: `.NET Framework 4.0` only; as of `.NET Framework 4.5+` [`System.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.readonlydictionary-2?view=netframework-4.5) is available.
- `ReadOnlyItemCollection<T>` (the read-only wrapper for [`ICollection<T>`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.icollection-1))
- `ReadOnlySet<T>`
- `SemaphoreSlimBasedLock`
- `StopwatchElapsedTimeProvider` (implements `IElapsedTimeProvider`)
- `SyncValueContainer<T>` (implements `IValueContainer<T>`)
- `ValueContainer<T>` (implements `IValueContainer<T>`)
- `ValueRange<T>`
- `VirtualTreeNode<T>` and `VirtualTreeNodeRoot<T>`
- `WeakReferenceBasedCache<TKey, TValue>`

### The *non-exhaustive* list of own abstractions (interfaces)

- `ICurrentDateTimeProvider`
- `IElapsedTimeProvider`
- `IValueContainer<T>`

### The *non-exhaustive* list of own delegates

- `Task EventHandlerAsync<>(...)` (excluding .NET Framework 4.0)
- `OutFunc<TOutput, out TResult>`
- `OutFunc<in T, TOutput, out TResult>`
- `OutFunc<in T1, in T2, TOutput, out TResult>`
- `OutFunc<in T1, in T2, in T3, TOutput, out TResult>`

### Dealing with Compatibility Issues

Due to certain inconsistencies between `.NET Standard 2.0` and `.NET Core 2.x`, you may need to apply one or more workarounds as described below when using **`Omnifactotum`** with your projects compiled for `.NET Standard 2.0` or `.NET Core 2.x` **-or-** a mix of `.NET Standard 2.0` and/or `.NET Core 2.x` and/or `.NET Framework 4.x`:

| Omnifactotum's Class or Method | Workaround |
| :----------------------------- | :--------- |
| `System.Collections.Generic.KeyValuePair` | Use [`Omnifactotum.OmnifactotumKeyValuePair`](./src/Omnifactotum/OmnifactotumKeyValuePair.cs). |
| <code>[OmnifactotumCollectionExtensions](./src/Omnifactotum/ExtensionMethods/OmnifactotumCollectionExtensions.cs).ToHashSet(...)</code> | Use static method invocation instead of extension method invocation. That is: `OmnifactotumCollectionExtensions.ToHashSet(collection)` instead of `collection.ToHashSet()`. |
| <code>[OmnifactotumDictionaryExtensions](./src/Omnifactotum/ExtensionMethods/OmnifactotumDictionaryExtensions.cs).GetValueOrDefault</code> | Use static method invocation instead of extension method invocation. That is: `OmnifactotumDictionaryExtensions.GetValueOrDefault(dictionary)` instead of `dictionary.GetValueOrDefault()`. |
