### Extension method classes in `Omnifactotum`

#### Adding a new class with extension methods

1. Place the new extension method class under the namespace of the type being extended.
1. Use the following format for the class name: `Omnifactotum<Class>Extensions`, where `<Class>` is the name of the type being extended.

For example, to extend `System.IO.Stream`, create a file `OmnifactotumStreamExtensions.cs` in the directory `src\Omnifactotum\ExtensionMethods`:
```C#
//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System.IO
{
    /// <summary>
    ///     Contains extension methods for the <see cref="Stream"/> class.
    /// </summary>
    public static class OmnifactotumStreamExtensions
    {
        public static string DoSomething(this Stream value)
        {
            // ...
        }
    }
}
```
