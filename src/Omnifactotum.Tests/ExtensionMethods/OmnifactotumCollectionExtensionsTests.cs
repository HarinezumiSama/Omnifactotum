using System;
using System.Collections;
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
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
    public void TestGetFastCount()
    {
        Assert.That(() => default(string[]).GetFastCount(), Is.Zero);
        Assert.That(() => default(ImmutableArray<Guid>).GetFastCount(), Is.Zero);
        Assert.That(() => default(HashSet<Version>).GetFastCount(), Is.Zero);
        Assert.That(() => default(NonGenericCollection<Exception>).GetFastCount(), Is.Zero);
        Assert.That(() => default(IEnumerable<int>).GetFastCount(), Is.Zero);

        Assert.That(() => new string[0].GetFastCount(), Is.Zero);
        Assert.That(() => ImmutableArray<Guid>.Empty.GetFastCount(), Is.Zero);
        Assert.That(() => new HashSet<Version>().GetFastCount(), Is.Zero);
        Assert.That(() => new NonGenericCollection<Exception>(0).GetFastCount(), Is.Zero);

        Assert.That(() => new[] { "C#", "C++", "Fortran", "Pascal" }.GetFastCount(), Is.EqualTo(4));
        Assert.That(() => ImmutableArray.Create(Guid.Empty).GetFastCount(), Is.EqualTo(1));
        Assert.That(() => new HashSet<Version> { new(1, 2), new(3, 17) }.GetFastCount(), Is.EqualTo(2));
        Assert.That(() => new NonGenericCollection<Exception>(17).GetFastCount(), Is.EqualTo(17));

        Assert.That(() => CreateEnumerable(13, -17, 42).GetFastCount(), Is.Null);
        Assert.That(() => new[] { 13, -17, 42 }.Where(i => i > 0).GetFastCount(), Is.Null);
        Assert.That(() => Enumerable.Range(1, 10).GetFastCount(), Is.Null);

        static IEnumerable<T> CreateEnumerable<T>(params T[] items)
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }

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
        ExecuteTestCase<string, string[]>(Array.Empty<string>(), string.Empty);
        ExecuteTestCase<char, char[]>(new[] { 'A', '/', 'z' }, "A./.z.");
        ExecuteTestCase<int, ImmutableArray<int>>(ImmutableArray.Create(13, 42, 19), "13.42.19.");
        ExecuteTestCase<string, ImmutableArray<string>>(ImmutableArray<string>.Empty, string.Empty);
        ExecuteTestCase<string, ImmutableArray<string>>(default, string.Empty);

        static void ExecuteTestCase<T, TEnumerable>(TEnumerable collection, string expectedResult)
            where TEnumerable : IEnumerable<T>
        {
            var stringBuilder = new StringBuilder();
            collection.DoForEach(value => stringBuilder.Append(value.ToStringSafelyInvariant()).Append('.'));
            Assert.That(stringBuilder.ToString(), Is.EqualTo(expectedResult));
        }
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
        ExecuteTestCase<string, string[]>(Array.Empty<string>(), string.Empty);
        ExecuteTestCase<char, char[]>(new[] { 'A', '/', 'z' }, "A:0./:1.z:2.");
        ExecuteTestCase<int, ImmutableArray<int>>(ImmutableArray.Create(13, 42, 19), "13:0.42:1.19:2.");
        ExecuteTestCase<string, ImmutableArray<string>>(ImmutableArray<string>.Empty, string.Empty);
        ExecuteTestCase<string, ImmutableArray<string>>(default, string.Empty);

        static void ExecuteTestCase<T, TEnumerable>(TEnumerable collection, string expectedResult)
            where TEnumerable : IEnumerable<T>
        {
            var stringBuilder = new StringBuilder();
            collection.DoForEach((value, index) => stringBuilder.Append(value).Append(':').Append(index).Append('.'));
            Assert.That(stringBuilder.ToString(), Is.EqualTo(expectedResult));
        }
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
        await ExecuteTestCaseAsync<string, string[]>(Array.Empty<string>(), string.Empty);
        await ExecuteTestCaseAsync<char, char[]>(new[] { 'A', '/', 'z' }, "A./.z.");
        await ExecuteTestCaseAsync<int, ImmutableArray<int>>(ImmutableArray.Create(13, 42, 19), "13.42.19.");
        await ExecuteTestCaseAsync<string, ImmutableArray<string>>(ImmutableArray<string>.Empty, string.Empty);
        await ExecuteTestCaseAsync<string, ImmutableArray<string>>(default, string.Empty);

        static async Task ExecuteTestCaseAsync<T, TEnumerable>(TEnumerable collection, string expectedResult)
            where TEnumerable : IEnumerable<T>
        {
            var stringBuilder = new StringBuilder();

            await collection.DoForEachAsync(
                async (item, token) =>
                {
                    await Task.Delay(0, token);
                    stringBuilder.Append(item).Append('.');
                });

            Assert.That(stringBuilder.ToString(), Is.EqualTo(expectedResult));
        }
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
        await ExecuteTestCaseAsync<string, string[]>(Array.Empty<string>(), string.Empty);
        await ExecuteTestCaseAsync<char, char[]>(new[] { 'A', '/', 'z' }, "A:0./:1.z:2.");
        await ExecuteTestCaseAsync<int, ImmutableArray<int>>(ImmutableArray.Create(13, 42, 19), "13:0.42:1.19:2.");
        await ExecuteTestCaseAsync<string, ImmutableArray<string>>(ImmutableArray<string>.Empty, string.Empty);
        await ExecuteTestCaseAsync<string, ImmutableArray<string>>(default, string.Empty);

        static async Task ExecuteTestCaseAsync<T, TEnumerable>(TEnumerable collection, string expectedResult)
            where TEnumerable : IEnumerable<T>
        {
            var stringBuilder = new StringBuilder();

            await collection.DoForEachAsync(
                async (item, index, token) =>
                {
                    await Task.Delay(0, token);
                    stringBuilder.Append(item).Append(':').Append(index).Append('.');
                });

            Assert.That(stringBuilder.ToString(), Is.EqualTo(expectedResult));
        }
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
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    public void TestSetItemsWhenInvalidArgumentThenThrows()
    {
        Assert.That(() => default(List<string>)!.SetItems(new[] { string.Empty }), Throws.ArgumentNullException);
        Assert.That(() => new List<string>().SetItems(default(List<string>)!), Throws.ArgumentNullException);

        Assert.That(() => new string[0].SetItems(new[] { string.Empty }), Throws.TypeOf<NotSupportedException>());
        Assert.That(() => new ReadOnlyCollection<string>(new List<string>()).SetItems(new[] { string.Empty }), Throws.TypeOf<NotSupportedException>());
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

        static void InvokeTestSetItems<T, TCollection>(Func<T[]> createItems1, Func<T[]> createItems2)
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
    }

    [Test]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Local")]
    [SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
    public void TestCollectionsEquivalent()
    {
        const int Value1 = 13;
        const int Value2 = 42;
        const int NegativeValue2 = -Value2;

        Assert.That(new[] { Value1, Value2, NegativeValue2 }, Is.Unique);
        Assert.That(Math.Abs(NegativeValue2), Is.EqualTo(Value2));

        ExecuteTestCase(default, default, true);
        ExecuteTestCase(new int[0], new int[0], true);

        ExecuteTestCase(Array.Empty<int>(), default, false);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1 }, false);
        ExecuteTestCase(new[] { Value1, Value1 }, new[] { Value1 }, false);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value1, Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1, Value1, Value2 }, new[] { Value1, Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1, Value2, Value1 }, new[] { Value2, Value1, Value2 }, false);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1, Value2 }, true);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value2, Value1 }, true);
        ExecuteTestCase(new[] { Value2, Value1, NegativeValue2 }, new[] { NegativeValue2, Value1, Value2 }, true);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1, NegativeValue2 }, false, true);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { NegativeValue2, Value1 }, false, true);

        static void ExecuteTestCase(
            int[]? collection,
            int[]? otherCollection,
            bool expectedDefaultResult,
            bool? expectedCustomResult = null)
        {
            InternalExecuteTestCase(collection, otherCollection, expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), otherCollection, expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), otherCollection, expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(collection, ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(collection, ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArray(collection), ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(
                ViaImmutableArrayWithDefault(collection),
                ViaImmutableArrayWithDefault(otherCollection),
                expectedDefaultResult,
                expectedCustomResult);

            InternalExecuteTestCase(ViaList(collection), otherCollection, expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(collection, ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaList(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaList(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaList(collection), ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            static ImmutableArray<int>? ViaImmutableArray(int[]? value)
                => value is null
                    ? null
                    : ImmutableArray.Create(value);

            static ImmutableArray<int>? ViaImmutableArrayWithDefault(int[]? value)
                => value is null
                    ? null
                    : value.Length == 0
                        ? default(ImmutableArray<int>)
                        : ImmutableArray.Create(value);

            static List<int>? ViaList(int[]? value) => value is null ? null : new List<int>(value);
        }

        static void InternalExecuteTestCase(
            IEnumerable<int>? collection,
            IEnumerable<int>? otherCollection,
            bool expectedDefaultResult,
            bool? expectedCustomResult)
        {
            Assert.That(() => collection.CollectionsEquivalent(otherCollection), Is.EqualTo(expectedDefaultResult));
            Assert.That(() => otherCollection.CollectionsEquivalent(collection), Is.EqualTo(expectedDefaultResult));

            Assert.That(() => collection.CollectionsEquivalent(otherCollection, null), Is.EqualTo(expectedDefaultResult));
            Assert.That(() => otherCollection.CollectionsEquivalent(collection, null), Is.EqualTo(expectedDefaultResult));

            Assert.That(
                () => collection.CollectionsEquivalent(otherCollection, AbsValueEqualityComparer.Instance),
                Is.EqualTo(expectedCustomResult ?? expectedDefaultResult));

            Assert.That(
                () => otherCollection.CollectionsEquivalent(collection, AbsValueEqualityComparer.Instance),
                Is.EqualTo(expectedCustomResult ?? expectedDefaultResult));
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
    [SuppressMessage("ReSharper", "ReturnTypeCanBeEnumerable.Local")]
    [SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
    public void TestCollectionsEqual()
    {
        const int Value1 = 13;
        const int Value2 = 42;
        const int NegativeValue2 = -Value2;

        Assert.That(new[] { Value1, Value2, NegativeValue2 }, Is.Unique);
        Assert.That(Math.Abs(NegativeValue2), Is.EqualTo(Value2));

        ExecuteTestCase(default, default, true);
        ExecuteTestCase(new int[0], new int[0], true);

        ExecuteTestCase(Array.Empty<int>(), default, false);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1 }, false);
        ExecuteTestCase(new[] { Value1, Value1 }, new[] { Value1 }, false);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value1, Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1 }, new[] { Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1, Value1, Value2 }, new[] { Value1, Value2, Value2 }, false);
        ExecuteTestCase(new[] { Value1, Value2, Value1 }, new[] { Value2, Value1, Value2 }, false);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1, Value2 }, true);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value2, Value1 }, false);
        ExecuteTestCase(new[] { Value2, Value1, NegativeValue2 }, new[] { NegativeValue2, Value1, Value2 }, false, true);

        ExecuteTestCase(new[] { Value1, Value2 }, new[] { Value1, NegativeValue2 }, false, true);
        ExecuteTestCase(new[] { Value1, Value2 }, new[] { NegativeValue2, Value1 }, false, false);

        static void ExecuteTestCase(
            int[]? collection,
            int[]? otherCollection,
            bool expectedDefaultResult,
            bool? expectedCustomResult = null)
        {
            InternalExecuteTestCase(collection, otherCollection, expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), otherCollection, expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), otherCollection, expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(collection, ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(collection, ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArray(collection), ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(
                ViaImmutableArrayWithDefault(collection),
                ViaImmutableArrayWithDefault(otherCollection),
                expectedDefaultResult,
                expectedCustomResult);

            InternalExecuteTestCase(ViaList(collection), otherCollection, expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(collection, ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaList(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaImmutableArray(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaImmutableArrayWithDefault(collection), ViaList(otherCollection), expectedDefaultResult, expectedCustomResult);

            InternalExecuteTestCase(ViaList(collection), ViaImmutableArray(otherCollection), expectedDefaultResult, expectedCustomResult);
            InternalExecuteTestCase(ViaList(collection), ViaImmutableArrayWithDefault(otherCollection), expectedDefaultResult, expectedCustomResult);

            static ImmutableArray<int>? ViaImmutableArray(int[]? value)
                => value is null
                    ? null
                    : ImmutableArray.Create(value);

            static ImmutableArray<int>? ViaImmutableArrayWithDefault(int[]? value)
                => value is null
                    ? null
                    : value.Length == 0
                        ? default(ImmutableArray<int>)
                        : ImmutableArray.Create(value);

            static List<int>? ViaList(int[]? value) => value is null ? null : new List<int>(value);
        }

        static void InternalExecuteTestCase(
            IEnumerable<int>? collection,
            IEnumerable<int>? otherCollection,
            bool expectedDefaultResult,
            bool? expectedCustomResult)
        {
            Assert.That(() => collection.CollectionsEqual(otherCollection), Is.EqualTo(expectedDefaultResult));
            Assert.That(() => otherCollection.CollectionsEqual(collection), Is.EqualTo(expectedDefaultResult));

            Assert.That(() => collection.CollectionsEqual(otherCollection, null), Is.EqualTo(expectedDefaultResult));
            Assert.That(() => otherCollection.CollectionsEqual(collection, null), Is.EqualTo(expectedDefaultResult));

            Assert.That(
                () => collection.CollectionsEqual(otherCollection, AbsValueEqualityComparer.Instance),
                Is.EqualTo(expectedCustomResult ?? expectedDefaultResult));

            Assert.That(
                () => otherCollection.CollectionsEqual(collection, AbsValueEqualityComparer.Instance),
                Is.EqualTo(expectedCustomResult ?? expectedDefaultResult));
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantCast")]
    public void TestFindDuplicatesWhenInvalidArgumentsThenThrows()
    {
        Assert.That(
            () => default(string[])!.FindDuplicates(Factotum.For<string>.IdentityMethod),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("collection"));

        Assert.That(
            () => default(string[])!.FindDuplicates(Factotum.For<string>.IdentityMethod, null),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("collection"));

        Assert.That(
            () => default(string[])!.FindDuplicates(Factotum.For<string>.IdentityMethod, StringComparer.Ordinal),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("collection"));

        Assert.That(
            () => Array.Empty<string>().FindDuplicates((Func<string, string>)null!),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("keySelector"));

        Assert.That(
            () => Array.Empty<string>().FindDuplicates((Func<string, string>)null!, null),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("keySelector"));

        Assert.That(
            () => Array.Empty<string>().FindDuplicates((Func<string, string>)null!, StringComparer.Ordinal),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("keySelector"));
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestFindDuplicatesWhenValidArgumentsThenSucceeds()
    {
        Assert.That(() => Array.Empty<Version>().FindDuplicates(Factotum.For<Version>.IdentityMethod), Is.Not.Null.And.Empty);
        Assert.That(() => ImmutableArray<Version>.Empty.FindDuplicates(Factotum.For<Version>.IdentityMethod), Is.Not.Null.And.Empty);
        Assert.That(() => default(ImmutableArray<Version>).FindDuplicates(Factotum.For<Version>.IdentityMethod), Is.Not.Null.And.Empty);

        {
            var duplicates = new[] { 1, 2, 1, 3, -7, -1, -7, 19, -7 }.FindDuplicates(Factotum.For<int>.IdentityMethod);
            Assert.That(duplicates, Is.Not.Null);
            Assert.That(duplicates.Count, Is.EqualTo(2));
            Assert.That(duplicates.ContainsKey(1), Is.True);
            Assert.That(duplicates[1], Is.EquivalentTo(Enumerable.Repeat(1, 2)));
            Assert.That(duplicates.ContainsKey(-7), Is.True);
            Assert.That(duplicates[-7], Is.EquivalentTo(Enumerable.Repeat(-7, 3)));
        }

        {
            var duplicates = ImmutableArray.Create(new Version(1, 3), new Version(2, 2), new Version(1, 2)).FindDuplicates(v => v.Major);
            Assert.That(duplicates, Is.Not.Null);
            Assert.That(duplicates.Count, Is.EqualTo(1));
            Assert.That(duplicates.ContainsKey(1), Is.True);
            Assert.That(duplicates[1], Is.EquivalentTo(new[] { new Version(1, 2), new Version(1, 3) }));
        }

        {
            var duplicates = new[] { "Help", "Hi", "HELPER", "human", "Hello", "high" }.FindDuplicates(
                s => s.Substring(0, 2),
                StringComparer.OrdinalIgnoreCase);

            Assert.That(duplicates, Is.Not.Null);
            Assert.That(duplicates.Count, Is.EqualTo(2));
            Assert.That(duplicates.ContainsKey("he"), Is.True);
            Assert.That(duplicates["he"], Is.EquivalentTo(new[] { "HELPER", "Help", "Hello" }));
            Assert.That(duplicates.ContainsKey("hi"), Is.True);
            Assert.That(duplicates["hi"], Is.EquivalentTo(new[] { "high", "Hi" }));
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestDisposeCollectionItemsSafelyForReferenceTypeItems()
    {
        Assert.That(() => default(IEnumerable<ReferenceTypeDisposable>).DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => default(ReferenceTypeDisposable?[]).DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => default(ImmutableArray<ReferenceTypeDisposable>).DisposeCollectionItemsSafely(), Throws.Nothing);

        Assert.That(() => Enumerable.Empty<ReferenceTypeDisposable>().DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => new ReferenceTypeDisposable?[0].DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => ImmutableArray<ReferenceTypeDisposable>.Empty.DisposeCollectionItemsSafely(), Throws.Nothing);

        ExecuteTestCase((i1, i2, i3) => Enumerable.Repeat(i1, 1).Append(i2).Append(i3));
        ExecuteTestCase((i1, i2, i3) => new[] { i1, i2, i3 });
        ExecuteTestCase((i1, i2, i3) => ImmutableArray.Create(i1, i2, i3));

        static void ExecuteTestCase<TEnumerable>(
            Func<ReferenceTypeDisposable?, ReferenceTypeDisposable?, ReferenceTypeDisposable?, TEnumerable> createCollection)
            where TEnumerable : IEnumerable<ReferenceTypeDisposable?>
        {
            var item1 = new ReferenceTypeDisposable();
            var item2 = new ReferenceTypeDisposable();
            var collection = createCollection(item1, null, item2);

            Assert.That(item1.DisposeCallCount, Is.EqualTo(0));
            Assert.That(item2.DisposeCallCount, Is.EqualTo(0));

            Assert.That(() => collection.DisposeCollectionItemsSafely(), Throws.Nothing);

            Assert.That(item1.DisposeCallCount, Is.EqualTo(1));
            Assert.That(item2.DisposeCallCount, Is.EqualTo(1));
        }
    }

    [Test]
    [SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
    [SuppressMessage("Performance", "CA1825:Avoid zero-length array allocations")]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestDisposeCollectionItemsSafelyForValueTypeItems()
    {
        Assert.That(() => default(IEnumerable<ValueTypeDisposable?>).DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => default(ValueTypeDisposable?[]).DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => default(ImmutableArray<ValueTypeDisposable?>).DisposeCollectionItemsSafely(), Throws.Nothing);

        Assert.That(() => Enumerable.Empty<ValueTypeDisposable?>().DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => new ValueTypeDisposable?[0].DisposeCollectionItemsSafely(), Throws.Nothing);
        Assert.That(() => ImmutableArray<ValueTypeDisposable?>.Empty.DisposeCollectionItemsSafely(), Throws.Nothing);

        ExecuteTestCase((i1, i2, i3) => Enumerable.Repeat(i1, 1).Append(i2).Append(i3));
        ExecuteTestCase((i1, i2, i3) => new[] { i1, i2, i3 });
        ExecuteTestCase((i1, i2, i3) => ImmutableArray.Create(i1, i2, i3));

        static void ExecuteTestCase<TEnumerable>(
            Func<ValueTypeDisposable?, ValueTypeDisposable?, ValueTypeDisposable?, TEnumerable> createCollection)
            where TEnumerable : IEnumerable<ValueTypeDisposable?>
        {
            var item1 = new ValueTypeDisposable(new ReferenceTypeDisposable());
            var item2 = new ValueTypeDisposable(new ReferenceTypeDisposable());
            var collection = createCollection(item1, null, item2);

            Assert.That(item1.DisposeCallCount, Is.EqualTo(0));
            Assert.That(item2.DisposeCallCount, Is.EqualTo(0));

            Assert.That(() => collection.DisposeCollectionItemsSafely(), Throws.Nothing);

            Assert.That(item1.DisposeCallCount, Is.EqualTo(1));
            Assert.That(item2.DisposeCallCount, Is.EqualTo(1));
        }
    }

    [Test]
    [TestCaseSource(typeof(ToUIStringForStringCollectionTestCases))]
    public void TestToUIStringForStringCollection(IEnumerable<string?>? values, string expectedResult)
    {
        var actualResult = values.ToUIString();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCaseSource(typeof(ToUIStringForNullableValueTypeCollectionTestCases))]
    public void TestToUIStringForNullableValueTypeCollection(IEnumerable<int?>? values, string expectedResult)
    {
        var actualResult = values.ToUIString();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    public void TestToUIStringForStringKeyValuePairCollection()
    {
        Assert.That(() => default(IEnumerable<KeyValuePair<string, string?>>).ToUIString(), Is.EqualTo("<null>"));
        Assert.That(() => default(IEnumerable<KeyValuePair<string, string>>)!.ToUIString(), Is.EqualTo("<null>"));

        Assert.That(() => default(ImmutableArray<KeyValuePair<string, string?>>).ToUIString(), Is.EqualTo("[]"));
        Assert.That(() => default(ImmutableArray<KeyValuePair<string, string>>).ToUIString(), Is.EqualTo("[]"));

        Assert.That(() => new Dictionary<string, string?>().ToUIString(), Is.EqualTo("[]"));
        Assert.That(() => new Dictionary<string, string>()!.ToUIString(), Is.EqualTo("[]"));

        Assert.That(
            () => new Dictionary<string, string?> { { "Qwe", null }, { "asD", "zXc" }, { "uiOp", string.Empty } }.ToUIString(),
            Is.EqualTo("""[{ "Qwe": null }, { "asD": "zXc" }, { "uiOp": "" }]"""));
    }

#if !NET7_0_OR_GREATER

    [Test]
    public void TestAsReadOnlyNegative()
        => Assert.That(() => default(IList<string>)!.AsReadOnly(), Throws.TypeOf<ArgumentNullException>());

    [Test]
    public void TestAsReadOnly()
    {
        Assert.That(() => default(ImmutableArray<int>).AsReadOnly(), Is.Empty);
        Assert.That(() => ImmutableArray<int>.Empty.AsReadOnly(), Is.Empty);

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

        Assert.That(default(ImmutableArray<int>).Chunk(3), Is.Empty);
        Assert.That(ImmutableArray<int>.Empty.Chunk(3), Is.Empty);

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
        ExecuteTestCase(ImmutableArray<string?>.Empty, Array.Empty<string>());
        ExecuteTestCase(default(ImmutableArray<string?>), Array.Empty<string>());
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
        static void ExecuteTestCase<T>(IEnumerable<T?> input, T[] expectedResult)
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
        ExecuteTestCase(ImmutableArray<int?>.Empty, Array.Empty<int>());
        ExecuteTestCase(default(ImmutableArray<int?>), Array.Empty<int>());
        ExecuteTestCase(new int?[0], Array.Empty<int>());
        ExecuteTestCase(new int?[] { null }, Array.Empty<int>());
        ExecuteTestCase(new int?[] { null, null }, Array.Empty<int>());
        ExecuteTestCase(new int?[] { null, 42, null }, new[] { 42 });
        ExecuteTestCase(new int?[] { 42, null, 17, -11, null, 13, 7 }, new[] { 42, 17, -11, 13, 7 });

        ExecuteTestCase(new char?[0], Array.Empty<char>());
        ExecuteTestCase(new char?[] { null }, Array.Empty<char>());
        ExecuteTestCase(new char?[] { null, null }, Array.Empty<char>());
        ExecuteTestCase(new char?[] { null, 'q', null }, new[] { 'q' });
        ExecuteTestCase(new char?[] { 'w', null, 'z', '•', null, 'é', 'ò' }, new[] { 'w', 'z', '•', 'é', 'ò' });

        //// ReSharper disable once SuggestBaseTypeForParameter
        static void ExecuteTestCase<T>(IEnumerable<T?> input, T[] expectedResult)
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

    [Test]
    public void TestFlattenWhenInvalidArgumentThenThrows()
    {
        Assert.That(() => default(IEnumerable<IEnumerable<object?>>)!.Flatten(), Throws.ArgumentNullException);
        Assert.That(() => default(IEnumerable<IEnumerable<int>>)!.Flatten(), Throws.ArgumentNullException);
        Assert.That(() => default(IEnumerable<IEnumerable<int?>>)!.Flatten(), Throws.ArgumentNullException);
        Assert.That(() => default(IEnumerable<IEnumerable<string?>>)!.Flatten(), Throws.ArgumentNullException);
        Assert.That(() => default(IEnumerable<IEnumerable<string>>)!.Flatten(), Throws.ArgumentNullException);
    }

    [Test]
    public void TestFlattenWhenValidArgumentThenSucceeds()
    {
        // int

        Assert.That(() => default(ImmutableArray<IEnumerable<int>>).Flatten(), Is.EqualTo(Enumerable.Empty<int>()));
        Assert.That(() => ImmutableArray<int[]>.Empty.Flatten(), Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(() => Array.Empty<int[]>().Flatten(), Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(
            () => new[] { Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>() }.Flatten(),
            Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(
            () => new IEnumerable<int>[] { ImmutableArray<int>.Empty, default(ImmutableArray<int>) }.Flatten(),
            Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(
            () => ImmutableArray.Create<IEnumerable<int>>(ImmutableArray<int>.Empty, default(ImmutableArray<int>)).Flatten(),
            Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(
            () => new[] { Array.Empty<int>(), Array.Empty<int>(), Array.Empty<int>() }.Flatten(),
            Is.EqualTo(Enumerable.Empty<int>()));

        Assert.That(
            () => new[] { new[] { 1, 2, 3 }, new[] { 6, 5, 4 }, new[] { 13, 19, -7 } }.Flatten(),
            Is.EqualTo(new[] { 1, 2, 3, 6, 5, 4, 13, 19, -7 }));

        // string

        Assert.That(() => default(ImmutableArray<IEnumerable<string>>).Flatten(), Is.EqualTo(Enumerable.Empty<string>()));
        Assert.That(() => ImmutableArray<string[]>.Empty.Flatten(), Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(() => Array.Empty<string[]>().Flatten(), Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(
            () => new[] { Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>() }.Flatten(),
            Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(
            () => new IEnumerable<string>[] { ImmutableArray<string>.Empty, default(ImmutableArray<string>) }.Flatten(),
            Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(
            () => ImmutableArray.Create<IEnumerable<string>>(ImmutableArray<string>.Empty, default(ImmutableArray<string>)).Flatten(),
            Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(
            () => new[] { Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>() }.Flatten(),
            Is.EqualTo(Enumerable.Empty<string>()));

        Assert.That(
            () => new[] { new[] { 'a', 'c', '?' }, new[] { 'w', 'b', '4' }, new[] { '3', 'X', '!' } }.Flatten(),
            Is.EqualTo(new[] { 'a', 'c', '?', 'w', 'b', '4', '3', 'X', '!' }));
    }

    private sealed class ToUIStringForStringCollectionTestCases : TestCasesBase
    {
        protected override IEnumerable<TestCaseData> GetCases()
        {
            yield return CreateTestCase(null, "<null>").SetDescription("Null collection of strings");

            yield return CreateTestCase(ImmutableArray<string>.Empty, string.Empty).SetDescription("Empty immutable array");
            yield return CreateTestCase(default(ImmutableArray<string>), string.Empty).SetDescription("Default immutable array");

            yield return CreateTestCase(
                    new[] { null, "", "Hello", "Class \"MyClass\"" },
                    "null, \"\", \"Hello\", \"Class \"\"MyClass\"\"\"")
                .SetDescription("Collection containing various string values");

            static TestCaseData CreateTestCase(
                IEnumerable<string?>? values,
                string expectedResult,
                [CallerArgumentExpression(nameof(values))] string? valuesExpression = null)
            {
                var result = new TestCaseData(values, expectedResult);
                if (!string.IsNullOrEmpty(valuesExpression))
                {
                    result.SetArgDisplayNames($"{{ {valuesExpression} }}", $"{{ {expectedResult} }}");
                }

                return result;
            }
        }
    }

    private sealed class ToUIStringForNullableValueTypeCollectionTestCases : TestCasesBase
    {
        protected override IEnumerable<TestCaseData> GetCases()
        {
            yield return CreateTestCase(null, "<null>").SetDescription("Null collection of nullable integers");

            yield return CreateTestCase(ImmutableArray<int?>.Empty, string.Empty).SetDescription("Empty immutable array");
            yield return CreateTestCase(default(ImmutableArray<int?>), string.Empty).SetDescription("Default immutable array");

            yield return CreateTestCase(new int?[] { null, 42 }, "null, 42").SetDescription("Collection of nullable integers containing various values");

            static TestCaseData CreateTestCase(
                IEnumerable<int?>? values,
                string expectedResult,
                [CallerArgumentExpression(nameof(values))] string? valuesExpression = null)
            {
                var result = new TestCaseData(values, expectedResult);
                if (!string.IsNullOrEmpty(valuesExpression))
                {
                    result.SetArgDisplayNames($"{{ {valuesExpression} }}", $"{{ {expectedResult} }}");
                }

                return result;
            }
        }
    }

    private sealed class AbsValueEqualityComparer : IEqualityComparer<int>
    {
        internal static IEqualityComparer<int> Instance { get; } = new AbsValueEqualityComparer();

        public bool Equals(int x, int y) => Math.Abs(x) == Math.Abs(y);

        public int GetHashCode(int obj) => Math.Abs(obj).GetHashCode();
    }

    private sealed class NonGenericCollection<T> : IEnumerable<T>, ICollection
    {
        public NonGenericCollection(int count)
        {
            Factotum.Assert(count >= 0);
            Count = count;
        }

        public int Count { get; }

        public bool IsSynchronized => throw new NotSupportedException();

        public object SyncRoot => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator() => throw new NotSupportedException();

        public void CopyTo(Array array, int index) => throw new NotSupportedException();
    }

    private sealed class ReferenceTypeDisposable : IDisposable
    {
        internal int DisposeCallCount { get; private set; }

        public void Dispose()
        {
            checked
            {
                DisposeCallCount++;
            }
        }
    }

    private readonly struct ValueTypeDisposable : IDisposable
    {
        private readonly ReferenceTypeDisposable _innerDisposable;

        public ValueTypeDisposable(ReferenceTypeDisposable innerDisposable) => _innerDisposable = innerDisposable.AssertNotNull();

        internal int DisposeCallCount => _innerDisposable.AssertNotNull().DisposeCallCount;

        public void Dispose() => _innerDisposable.AssertNotNull().Dispose();
    }
}