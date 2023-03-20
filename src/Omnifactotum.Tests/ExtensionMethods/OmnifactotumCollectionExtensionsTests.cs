using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumCollectionExtensions))]
internal sealed class OmnifactotumCollectionExtensionsTests
{
    [Test]
    public void TestDoForEachWhenInvalidArgumentsThenThrows()
    {
        const int[]? NullArray = null;

        Assert.That(() => NullArray!.DoForEach(_ => { }), Throws.ArgumentNullException);
        Assert.That(() => new[] { "a" }.DoForEach(default(Action<string>)!), Throws.ArgumentNullException);
    }

    [Test]
    public void TestDoForEachWhenValidArgumentsThenSucceeds()
    {
        var stringBuilder = new StringBuilder();

        new[] { 'A', '/', 'z' }.DoForEach(c => stringBuilder.Append(c).Append('.'));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("A./.z."));
    }

    [Test]
    public void TestDoForEachWithIndexWhenInvalidArgumentsThenThrows()
    {
        const int[]? NullArray = null;

        Assert.That(() => NullArray!.DoForEach((_, _) => { }), Throws.ArgumentNullException);
        Assert.That(() => new[] { "a" }.DoForEach(default(Action<string, int>)!), Throws.ArgumentNullException);
    }

    [Test]
    public void TestDoForEachWithIndexWhenValidArgumentsThenSucceeds()
    {
        var stringBuilder = new StringBuilder();

        new[] { 'A', '/', 'z' }.DoForEach((c, i) => stringBuilder.Append(c).Append(':').Append(i).Append('.'));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("A:0./:1.z:2."));
    }

    [Test]
    public void TestDoForEachAsyncWhenInvalidArgumentsThenThrows()
    {
        const int[]? NullArray = null;

        Assert.That(() => NullArray!.DoForEachAsync((_, _) => Task.CompletedTask), Throws.ArgumentNullException);

        Assert.That(
            () => new[] { "a" }.DoForEachAsync(default(Func<string, CancellationToken, Task>)!),
            Throws.ArgumentNullException);
    }

    [Test]
    public async Task TestDoForEachAsyncWhenValidArgumentsThenSucceedsAsync()
    {
        var stringBuilder = new StringBuilder();

        await new[] { 'a', '/', 'Z' }.DoForEachAsync(
            async (item, token) =>
            {
                await Task.Delay(0, token);
                stringBuilder.Append(item).Append('.');
            });

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a./.Z."));
    }

    [Test]
    public void TestDoForEachAsyncWhenValidArgumentsAndCancelledThenThrowsOperationCanceledException()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var stringBuilder = new StringBuilder();

        const char LastItem = 'Z';

        Assert.That(
            async () =>
                await new[] { 'a', '/', LastItem }.DoForEachAsync(
                    async (item, token) =>
                    {
                        if (item == LastItem)
                        {
                            cancellationTokenSource.Cancel();
                        }

                        await Task.Delay(0, token);
                        stringBuilder.Append(item).Append('.');
                    },
                    cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a./."));

        Assert.That(
            async () =>
                await new[] { 42 }.DoForEachAsync(
                    async (item, _) =>
                    {
                        await Task.CompletedTask;
                        stringBuilder.Append(item).Append('.');
                    },
                    cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a./."));
    }

    [Test]
    public void TestDoForEachAsyncWithIndexWhenInvalidArgumentsThenThrows()
    {
        const int[]? NullArray = null;

        Assert.That(() => NullArray!.DoForEachAsync((_, _, _) => Task.CompletedTask), Throws.ArgumentNullException);

        Assert.That(
            () => new[] { "a" }.DoForEachAsync(default(Func<string, int, CancellationToken, Task>)!),
            Throws.ArgumentNullException);
    }

    [Test]
    public async Task TestDoForEachAsyncWithIndexWhenValidArgumentsThenSucceedsAsync()
    {
        var stringBuilder = new StringBuilder();

        await new[] { 'a', '/', 'Z' }.DoForEachAsync(
            async (item, index, token) =>
            {
                await Task.Delay(0, token);
                stringBuilder.Append(item).Append(':').Append(index).Append('.');
            });

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a:0./:1.Z:2."));
    }

    [Test]
    public void TestDoForEachAsyncWithIndexWhenValidArgumentsAndCancelledThenThrowsOperationCanceledException()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var stringBuilder = new StringBuilder();

        const char LastItem = 'Z';

        Assert.That(
            async () =>
                await new[] { 'a', '/', LastItem }.DoForEachAsync(
                    async (item, index, token) =>
                    {
                        if (item == LastItem)
                        {
                            cancellationTokenSource.Cancel();
                        }

                        await Task.Delay(0, token);
                        stringBuilder.Append(item).Append(':').Append(index).Append('.');
                    },
                    cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a:0./:1."));

        Assert.That(
            async () =>
                await new[] { 42 }.DoForEachAsync(
                    async (item, index, _) =>
                    {
                        await Task.CompletedTask;
                        stringBuilder.Append(item).Append(':').Append(index).Append('.');
                    },
                    cancellationToken),
            Throws.InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));

        Assert.That(stringBuilder.ToString(), Is.EqualTo("a:0./:1."));
    }

    [Test]
    public void TestSetItemsWhenInvalidArgumentThenThrows()
    {
        List<string>? nullList = null;

        Assert.That(() => nullList!.SetItems(new[] { string.Empty }), Throws.ArgumentNullException);
        Assert.That(() => new List<string>().SetItems(nullList!), Throws.ArgumentNullException);
    }

    [Test]
    public void TestSetItemsValidArgumentsThenSucceeds()
    {
        static int[] CreateIntItems1() => new[] { 17, 42, -19 };
        static int[] CreateIntItems2() => new[] { 19, -37, 17, 0 };

        InvokeTestSetItems<int, Collection<int>>(CreateIntItems1, CreateIntItems2);
        InvokeTestSetItems<int, List<int>>(CreateIntItems1, CreateIntItems2);

        static string[] CreateStringItems1() => new[] { "az", "zA", "qwerty" };
        static string[] CreateStringItems2() => new[] { nameof(TestSetItemsValidArgumentsThenSucceeds), string.Empty };

        InvokeTestSetItems<string, Collection<string>>(CreateStringItems1, CreateStringItems2);
        InvokeTestSetItems<string, List<string>>(CreateStringItems1, CreateStringItems2);

        static KeyValuePair<int, string>[] CreateKeyValuePairItems1()
            => new[] { KeyValuePair.Create(17, "seventeen"), KeyValuePair.Create(-1, "minus one") };

        static KeyValuePair<int, string>[] CreateKeyValuePairItems2()
            => new[] { KeyValuePair.Create(-13, "minus thirteen"), KeyValuePair.Create(0, "zero"), KeyValuePair.Create(int.MaxValue, "wow") };

        InvokeTestSetItems<KeyValuePair<int, string>, Dictionary<int, string>>(CreateKeyValuePairItems1, CreateKeyValuePairItems2);
    }

    [Test]
    [TestCaseSource(typeof(ToUIStringForStringCollectionTestCases))]
    public void TestToUIStringForStringCollection(string?[] values, string expectedResult)
    {
        var actualResult = values.ToUIString();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(typeof(ToUIStringForNullableCollectionTestCases))]
    public void TestToUIStringForNullableCollection(int?[] values, string expectedResult)
    {
        var actualResult = values.ToUIString();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

#if !NET7_0_OR_GREATER
    [Test]
    public void TestAsReadOnlyNegative()
        => Assert.That(() => default(IList<string>)!.AsReadOnly(), Throws.TypeOf<ArgumentNullException>());

    [Test]
    public void TestAsReadOnly()
    {
        var initialValues = new[] { "foo", "bar" };

        IList<string> list = new List<string>(initialValues);
        var readOnly = list.AsReadOnly();

        Assert.That(readOnly, Is.Not.Null);
        Assert.That(readOnly, Is.TypeOf<ReadOnlyCollection<string>>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        var readOnlyAsCollection = (ICollection<string>)readOnly;
        var readOnlyAsList = (IList<string>)readOnly;

        Assert.That(() => readOnlyAsCollection.Clear(), Throws.TypeOf<NotSupportedException>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        Assert.That(() => readOnlyAsCollection.Add("something"), Throws.TypeOf<NotSupportedException>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        Assert.That(() => readOnlyAsCollection.Remove("foo"), Throws.TypeOf<NotSupportedException>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        Assert.That(() => readOnlyAsList[0] = "Foo 2", Throws.TypeOf<NotSupportedException>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        Assert.That(() => readOnlyAsList.RemoveAt(0), Throws.TypeOf<NotSupportedException>());
        Assert.That(readOnly, Is.EqualTo(initialValues));

        list[0] = "not foo";
        Assert.That(readOnly, Is.EqualTo(list));

        list.Add("double bar");
        Assert.That(readOnly, Is.EqualTo(list));
    }
#endif

#if !NET6_0_OR_GREATER
    [Test]
    public void TestChunkWhenInvalidArgumentThenThrows()
    {
        Assert.That(() => ((int[]?)null!).Chunk(1), Throws.ArgumentNullException);
        Assert.That(() => Array.Empty<int>().Chunk(0), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => Array.Empty<int>().Chunk(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => Array.Empty<int>().Chunk(int.MinValue), Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void TestChunkWhenValidArgumentsThenSucceeds()
    {
        var chunks0By3 = Array.Empty<int>().Chunk(3).ToArray();
        Assert.That(chunks0By3, Is.Empty);

        var chunks10By3 = Enumerable.Range(1, 10).Chunk(3).ToArray();
        Assert.That(chunks10By3.Length, Is.EqualTo(4));
        Assert.That(chunks10By3[0], Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(chunks10By3[1], Is.EqualTo(new[] { 4, 5, 6 }));
        Assert.That(chunks10By3[2], Is.EqualTo(new[] { 7, 8, 9 }));
        Assert.That(chunks10By3[3], Is.EqualTo(new[] { 10 }));

        var chunks3By1 = Enumerable.Range(1, 3).Chunk(1).ToArray();
        Assert.That(chunks3By1.Length, Is.EqualTo(3));
        Assert.That(chunks3By1[0], Is.EqualTo(new[] { 1 }));
        Assert.That(chunks3By1[1], Is.EqualTo(new[] { 2 }));
        Assert.That(chunks3By1[2], Is.EqualTo(new[] { 3 }));
    }

#endif

    [Test]
    public void TestAvoidNullWhenArgumentIsNullOrItsEquivalentThenSucceeds()
    {
        ExecuteTestCase(default(IEnumerable<int>));
        ExecuteTestCase(default(IEnumerable<string>));

        ExecuteTestCase(default(int[]));
        ExecuteTestCase(default(string[]));

        ExecuteTestCase(default(List<int>));
        ExecuteTestCase(default(List<string>));

        ExecuteTestCase(default(ImmutableList<int>));
        ExecuteTestCase(default(ImmutableList<string>));

        ExecuteTestCase(default(ImmutableArray<int>));
        ExecuteTestCase(default(ImmutableArray<string>));

        [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
        static void ExecuteTestCase<T>(IEnumerable<T>? input) => Assert.That(() => input.AvoidNull(), Is.EqualTo(Enumerable.Empty<T>()));
    }

    [Test]
    public void TestAvoidNullWhenArgumentIsNotNullNorItsEquivalentThenSucceeds()
    {
        ExecuteTestCase(17.AsArray().Concat(23.AsArray()), new[] { 17, 23 });
        ExecuteTestCase("Hello".AsArray().Concat("world".AsArray()), new[] { "Hello", "world" });

        ExecuteTestCase(new[] { 19, 29 }, new[] { 19, 29 });
        ExecuteTestCase(new[] { "Bye", "all" }, new[] { "Bye", "all" });

        ExecuteTestCase(new List<int> { 23, 31 }, new[] { 23, 31 });
        ExecuteTestCase(new List<string> { "Hello", "all" }, new[] { "Hello", "all" });

        ExecuteTestCase(ImmutableList.Create(3, -7), new[] { 3, -7 });
        ExecuteTestCase(ImmutableList.Create("Hello", "people"), new[] { "Hello", "people" });

        ExecuteTestCase(ImmutableArray.Create(-3, 7), new[] { -3, 7 });
        ExecuteTestCase(ImmutableArray.Create("Bye", "people"), new[] { "Bye", "people" });

        [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        static void ExecuteTestCase<TEnumerable, TElement>(TEnumerable input, TElement[] expectedResult)
            where TEnumerable : IEnumerable<TElement>?
        {
            expectedResult.AssertNotNull();

            Constraint constraint = Is.EqualTo(expectedResult);
            if (!typeof(TEnumerable).IsValueType)
            {
                constraint &= Is.SameAs(input);
            }

            Assert.That(() => input.AvoidNull(), constraint);
            Assert.That(() => input!.ToArray(), Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public void TestWhereNotNullWhenInvalidArgumentAndReferenceTypeElementThenThrows()
    {
        // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<T>` instance actually invokes `WhereNotNull()`

        Assert.That(() => ((IEnumerable<object?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());
        Assert.That(() => ((IEnumerable<string?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());
        Assert.That(() => ((IEnumerable<int[]?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());
        Assert.That(() => ((IEnumerable<List<int>?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());

        static EqualConstraint CreateConstraint() => Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("source");
    }

    [Test]
    public void TestWhereNotNullWhenValidArgumentAndReferenceTypeElementThenSucceeds()
    {
        ExecuteTestCase(Array.Empty<string?>(), Array.Empty<string>());
        ExecuteTestCase(new string?[] { null }, Array.Empty<string>());
        ExecuteTestCase(new string?[] { null, null }, Array.Empty<string>());
        ExecuteTestCase(new[] { null, string.Empty, "\x0020\t\r\n\x0020", null }, new[] { string.Empty, "\x0020\t\r\n\x0020" });
        ExecuteTestCase(new[] { null, "q", null }, new[] { "q" });

        ExecuteTestCase(
            new[] { "Hello\x0020world", "?", null, string.Empty, "\t\x0020\r\n", null, "Bye!" },
            new[] { "Hello\x0020world", "?", string.Empty, "\t\x0020\r\n", "Bye!" });

        ExecuteTestCase(Array.Empty<int[]?>(), Array.Empty<int[]>());
        ExecuteTestCase(new int[]?[] { null, null }, Array.Empty<int[]>());
        ExecuteTestCase(new int[]?[] { null, null }, Array.Empty<int[]>());
        ExecuteTestCase(new[] { null, new[] { 42 }, null }, new[] { new[] { 42 } });

        ExecuteTestCase(
            new[] { new[] { 1, 2, 3 }, null, new[] { 42, -42, 17 }, new[] { -13 }, null, new[] { 11 }, new[] { 7 } },
            new[] { new[] { 1, 2, 3 }, new[] { 42, -42, 17 }, new[] { -13 }, new[] { 11 }, new[] { 7 } });

        //// ReSharper disable once SuggestBaseTypeForParameter
        static void ExecuteTestCase<T>(T?[] input, T[] expectedResult)
            where T : class
        {
            // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<T>` instance actually invokes `WhereNotNull()`
            Assert.That(() => input.AsEnumerable().WhereNotNull().ToArray(), Is.EqualTo(expectedResult) & Is.TypeOf<T[]>());
        }
    }

    [Test]
    public void TestWhereNotNullWhenInvalidArgumentAndValueTypeElementThenThrows()
    {
        // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<T>` instance actually invokes `WhereNotNull()`

        Assert.That(() => ((IEnumerable<int?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());
        Assert.That(() => ((IEnumerable<char?>?)null)!.WhereNotNull().ToArray(), CreateConstraint());

        static EqualConstraint CreateConstraint() => Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("source");
    }

    [Test]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    public void TestWhereNotNullWhenValidArgumentAndValueTypeElementThenSucceeds()
    {
        ExecuteTestCase(new int?[0], new int[0]);
        ExecuteTestCase(new int?[] { null }, new int[0]);
        ExecuteTestCase(new int?[] { null, null }, new int[0]);
        ExecuteTestCase(new int?[] { null, 42, null }, new[] { 42 });
        ExecuteTestCase(new int?[] { 42, null, 17, -11, null, 13, 7 }, new[] { 42, 17, -11, 13, 7 });

        ExecuteTestCase(new char?[0], new char[0]);
        ExecuteTestCase(new char?[] { null }, new char[0]);
        ExecuteTestCase(new char?[] { null, null }, new char[0]);
        ExecuteTestCase(new char?[] { null, 'q', null }, new[] { 'q' });
        ExecuteTestCase(new char?[] { 'w', null, 'z', '•', null, 'é', 'ò' }, new[] { 'w', 'z', '•', 'é', 'ò' });

        //// ReSharper disable once SuggestBaseTypeForParameter
        static void ExecuteTestCase<T>(T?[] input, T[] expectedResult)
            where T : struct
        {
            // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<T>` instance actually invokes `WhereNotNull()`
            Assert.That(() => input.AsEnumerable().WhereNotNull().ToArray(), Is.EqualTo(expectedResult) & Is.TypeOf<T[]>());
        }
    }

    [Test]
    public void TestEnumerateToListAsyncWhenInvalidArgumentThenThrows()
    {
        ExecuteTestCase<int>();
        ExecuteTestCase<string>();
        ExecuteTestCase<object>();

        static void ExecuteTestCase<T>()
        {
            Assert.That(() => ((IAsyncEnumerable<T>?)null)!.EnumerateToListAsync(CancellationToken.None), Throws.ArgumentNullException);

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Assert.That(() => ((IAsyncEnumerable<T>?)null)!.EnumerateToListAsync(cancellationToken), Throws.ArgumentNullException);
        }
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task TestEnumerateToListAsyncWhenValidArgumentsThenSucceedsAsync(bool shouldCheckCancellationTokenValue)
    {
        const int Value1 = 17;
        const int Value2 = -23;
        const int Value3 = 11;

        using var cancellationTokenSource1 = new CancellationTokenSource();

        await ExecuteRegularTestCaseAsync(
            CreateInputAsync(shouldCheckCancellationTokenValue),
            new List<int> { Value1, Value2, Value3 },
            cancellationTokenSource1.Token);

        using var cancellationTokenSource2 = new CancellationTokenSource();

        ExecuteCancellationTestCase(
            CreateInputAsync(shouldCheckCancellationTokenValue, () => cancellationTokenSource2.Cancel(), cancellationTokenSource2.Token),
            cancellationTokenSource2.Token);

        static async IAsyncEnumerable<int> CreateInputAsync(
            bool shouldCheckCancellationToken,
            [InstantHandle] Action? cancelBeforeLastItem = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Task CheckCancellationTokenAsync() => shouldCheckCancellationToken ? Task.Delay(TimeSpan.Zero, cancellationToken) : Task.CompletedTask;

            await CheckCancellationTokenAsync();
            yield return Value1;

            await CheckCancellationTokenAsync();
            yield return Value2;

            cancelBeforeLastItem?.Invoke();
            await CheckCancellationTokenAsync();
            yield return Value3;
        }

        static async Task ExecuteRegularTestCaseAsync<T>(IAsyncEnumerable<T> input, List<T> expectedResult, CancellationToken cancellationToken)
        {
            var actualResult = await input.EnumerateToListAsync(cancellationToken);
            Assert.That(actualResult, Is.EqualTo(expectedResult) & Is.TypeOf<List<T>>());
        }

        static void ExecuteCancellationTestCase<T>(IAsyncEnumerable<T> input, CancellationToken cancellationToken)
            => Assert.That(
                async () => await input.EnumerateToListAsync(cancellationToken),
                Throws.InstanceOf<OperationCanceledException>()
                    .With
                    .Property(nameof(OperationCanceledException.CancellationToken))
                    .EqualTo(cancellationToken));
    }

    [Test]
    public void TestEnumerateToArrayAsyncWhenInvalidArgumentThenThrows()
    {
        ExecuteTestCase<int>();
        ExecuteTestCase<string>();
        ExecuteTestCase<object>();

        static void ExecuteTestCase<T>()
        {
            Assert.That(() => ((IAsyncEnumerable<T>?)null)!.EnumerateToArrayAsync(CancellationToken.None), Throws.ArgumentNullException);

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Assert.That(() => ((IAsyncEnumerable<T>?)null)!.EnumerateToArrayAsync(cancellationToken), Throws.ArgumentNullException);
        }
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task TestEnumerateToArrayAsyncWhenValidArgumentsThenSucceedsAsync(bool shouldCheckCancellationTokenValue)
    {
        const int Value1 = -13;
        const int Value2 = -17;
        const int Value3 = 29;

        using var cancellationTokenSource1 = new CancellationTokenSource();

        await ExecuteRegularTestCaseAsync(
            CreateInputAsync(shouldCheckCancellationTokenValue),
            new[] { Value1, Value2, Value3 },
            cancellationTokenSource1.Token);

        using var cancellationTokenSource2 = new CancellationTokenSource();

        ExecuteCancellationTestCase(
            CreateInputAsync(shouldCheckCancellationTokenValue, () => cancellationTokenSource2.Cancel(), cancellationTokenSource2.Token),
            cancellationTokenSource2.Token);

        static async IAsyncEnumerable<int> CreateInputAsync(
            bool shouldCheckCancellationToken,
            [InstantHandle] Action? cancelBeforeLastItem = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Task CheckCancellationTokenAsync() => shouldCheckCancellationToken ? Task.Delay(TimeSpan.Zero, cancellationToken) : Task.CompletedTask;

            await CheckCancellationTokenAsync();
            yield return Value1;

            await CheckCancellationTokenAsync();
            yield return Value2;

            cancelBeforeLastItem?.Invoke();
            await CheckCancellationTokenAsync();
            yield return Value3;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        static async Task ExecuteRegularTestCaseAsync<T>(IAsyncEnumerable<T> input, T[] expectedResult, CancellationToken cancellationToken)
        {
            var actualResult = await input.EnumerateToArrayAsync(cancellationToken);
            Assert.That(actualResult, Is.EqualTo(expectedResult) & Is.TypeOf<T[]>());
        }

        static void ExecuteCancellationTestCase<T>(IAsyncEnumerable<T> input, CancellationToken cancellationToken)
            => Assert.That(
                async () => await input.EnumerateToArrayAsync(cancellationToken),
                Throws.InstanceOf<OperationCanceledException>()
                    .With
                    .Property(nameof(OperationCanceledException.CancellationToken))
                    .EqualTo(cancellationToken));
    }

    private static void InvokeTestSetItems<T, TCollection>(Func<T[]> createItems1, Func<T[]> createItems2)
        where TCollection : ICollection<T>, new()
    {
        var collection = new TCollection();

        Assert.That(collection, Is.Not.Null);
        Assert.That(createItems1, Is.Not.Null);
        Assert.That(createItems2, Is.Not.Null);

        var items1 = createItems1().AssertNotNull();
        var items2 = createItems2().AssertNotNull();

        collection.SetItems(items1);
        Assert.That(collection, Is.EqualTo(items1));

        collection.SetItems(items2);
        Assert.That(collection, Is.EqualTo(items2));

        collection.SetItems(Array.Empty<T>());
        Assert.That(collection, Is.Empty);
    }

    private sealed class ToUIStringForStringCollectionTestCases : TestCasesBase
    {
        protected override IEnumerable<TestCaseData> GetCases()
        {
            yield return new TestCaseData(null, "<null>").SetDescription("Null collection of strings");

            yield return new TestCaseData(
                    new[] { null, "", "Hello", "Class \"MyClass\"" },
                    @"null, """", ""Hello"", ""Class """"MyClass""""""")
                .SetDescription("Collection containing various string values");
        }
    }

    private sealed class ToUIStringForNullableCollectionTestCases : TestCasesBase
    {
        protected override IEnumerable<TestCaseData> GetCases()
        {
            yield return new TestCaseData(null, "<null>").SetDescription("Null collection of nullable integers");

            yield return
                new TestCaseData(new int?[] { null, 42 }, @"null, 42")
                    .SetDescription("Collection of nullable integers containing various values");
        }
    }
}