using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Omnifactotum.NUnit;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumTypeExtensions))]
internal sealed class OmnifactotumTypeExtensionsTests
{
    [Test]
    public void TestGetManifestResourceStreamWhenNullTypeIsPassedThenThrows()
        => Assert.That(() => ((Type?)null)!.GetManifestResourceStream("ValidName"), Throws.ArgumentNullException);

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void TestGetManifestResourceStreamWhenInvalidNameIsPassedThenThrows(string? name)
        => Assert.That(() => GetType().GetManifestResourceStream(name!), Throws.ArgumentException);

    [Test]
    public void TestGetManifestResourceStreamNonExistentResourceNameIsPassedThenReturnsNull()
    {
        using var stream = GetType().GetManifestResourceStream("NonExistentResourceName");
        Assert.That(stream, Is.Null);
    }

    [Test]
    public void TestGetManifestResourceStreamWhenValidResourceNameIsPassedThenReturnsNonNullStream()
    {
        using var stream = GetType().GetManifestResourceStream("OmnifactotumTypeExtensionsTests.TestResource.txt");
        Assert.That(stream, Is.Not.Null);

        using var reader = new StreamReader(stream!);
        var actualValue = reader.ReadToEnd();
        Assert.That(actualValue, Is.EqualTo("Test"));
    }

    [Test]
    public void TestIsNullableValueTypeWhenNullArgumentIsPassedThenThrows()
    {
        Assert.That(() => ((Type?)null)!.IsNullableValueType(), Throws.ArgumentNullException);

#pragma warning disable CS0618
        Assert.That(() => ((Type?)null)!.IsNullable(), Throws.ArgumentNullException);
#pragma warning restore CS0618
    }

    [Test]
    [TestCase(typeof(void), false)]
    [TestCase(typeof(void*), false)]
    [TestCase(typeof(bool), false)]
    [TestCase(typeof(bool?), true)]
    [TestCase(typeof(int), false)]
    [TestCase(typeof(int*), false)]
    [TestCase(typeof(int?), true)]
    [TestCase(typeof(IntPtr), false)]
    [TestCase(typeof(IntPtr*), false)]
    [TestCase(typeof(IntPtr?), true)]
    [TestCase(typeof(DateTime), false)]
    [TestCase(typeof(DateTime?), true)]
    [TestCase(typeof(DateTimeKind), false)]
    [TestCase(typeof(DateTimeKind?), true)]
    [TestCase(typeof(string), false)]
    [TestCase(typeof(object), false)]
    [TestCase(typeof(Action), false)]
    [TestCase(typeof(IDisposable), false)]
    public void TestIsNullableValueTypeWhenValidArgumentIsPassedThenSucceeds(Type type, bool expectedResult)
    {
        Assert.That(type.IsNullableValueType(), Is.EqualTo(expectedResult));

#pragma warning disable CS0618
        Assert.That(type.IsNullable(), Is.EqualTo(expectedResult));
#pragma warning restore CS0618
    }

    [Test]
    public void TestGetCollectionElementTypeOrDefaultWhenInvalidArgumentThenThrows()
        => Assert.That(() => ((Type?)null)!.GetCollectionElementTypeOrDefault(), Throws.ArgumentNullException);

    [Test]
    [TestCase(typeof(object), null)]
    [TestCase(typeof(int), null)]
    [TestCase(typeof(int*), null)]
    [TestCase(typeof(string), typeof(char))]
    [TestCase(typeof(Array), typeof(object))]
    [TestCase(typeof(IEnumerable), typeof(object))]
    [TestCase(typeof(string[]), typeof(string))]
    [TestCase(typeof(char[]), typeof(char))]
    [TestCase(typeof(IEnumerable<char>), typeof(char))]
    [TestCase(typeof(ICollection<char>), typeof(char))]
    [TestCase(typeof(Collection<char>), typeof(char))]
    [TestCase(typeof(ReadOnlyCollection<char>), typeof(char))]
    [TestCase(typeof(IList<char>), typeof(char))]
    [TestCase(typeof(List<char>), typeof(char))]
    [TestCase(typeof(int[]), typeof(int))]
    [TestCase(typeof(IEnumerable<int>), typeof(int))]
    [TestCase(typeof(ICollection<int>), typeof(int))]
    [TestCase(typeof(Collection<int>), typeof(int))]
    [TestCase(typeof(ReadOnlyCollection<int>), typeof(int))]
    [TestCase(typeof(IList<int>), typeof(int))]
    [TestCase(typeof(List<int>), typeof(int))]
    [TestCase(typeof(IDictionary<int, string>), typeof(KeyValuePair<int, string>))]
    [TestCase(typeof(Dictionary<int, string>), typeof(KeyValuePair<int, string>))]
    [TestCase(typeof(HashSet<Type>), typeof(Type))]
    [TestCase(typeof(ISet<Type>), typeof(Type))]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestGetCollectionElementTypeOrDefaultWhenValidArgumentThenSucceeds(Type input, Type? expectedResult)
    {
        Assert.That(() => input.GetCollectionElementTypeOrDefault(), Is.EqualTo(expectedResult));

#pragma warning disable CS0618
        Assert.That(() => input.GetCollectionElementType(), Is.EqualTo(expectedResult));
#pragma warning restore CS0618
    }

    [Test]
    public void TestGetInterfaceMethodImplementationWhenInvalidArgumentsThenThrows()
    {
        var dummyMethodInfo = new Func<int, string>(DummyMethod).Method;

        Assert.That(
            () => default(Type)!.GetInterfaceMethodImplementation(dummyMethodInfo),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("implementationType"));

        Assert.That(
            () => typeof(string).GetInterfaceMethodImplementation(null!),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("interfaceMethod"));

        Assert.That(
            () => typeof(IEnumerable<int>).GetInterfaceMethodImplementation(dummyMethodInfo),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("implementationType")
                .And.Message.StartsWith("""The type "System.Collections.Generic.IEnumerable<System.Int32>" is an interface."""));

        Assert.That(
            () => typeof(string).GetInterfaceMethodImplementation(
                ((Expression<Func<string, string>>)(s => s.Insert(0, string.Empty))).GetLastMethod().EnsureNotNull()),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("interfaceMethod")
                .And.Message.StartsWith(
                    """The method { string string.Insert(int, string) } belongs to the type "System.String" which is not an interface."""));

        Assert.That(
            () => typeof(int).GetInterfaceMethodImplementation(
                typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose)).EnsureNotNull()),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("interfaceMethod")
                .And.Message.StartsWith(
                    """The method { void IDisposable.Dispose() } belongs to the interface "System.IDisposable" which is not implemented by the type "System.Int32"."""));

        Assert.That(
            () => typeof(List<>).GetInterfaceMethodImplementation(typeof(ICollection<>).GetMethod(nameof(ICollection<object>.Clear)).EnsureNotNull()),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("interfaceMethod")
                .And.Message.StartsWith(
                    """The method { void ICollection<T>.Clear() } belongs to the interface "System.Collections.Generic.ICollection<T>" which is not implemented by the type "System.Collections.Generic.List<T>"."""));
    }

    [Test]
    [TestCase(typeof(List<string>), typeof(ICollection<string>), nameof(ICollection<string>.Clear), false)]
    [TestCase(typeof(int[]), typeof(IList), nameof(IList.IndexOf), false)]
    [TestCase(typeof(CancellationTokenSource), typeof(IDisposable), nameof(IDisposable.Dispose), false)]
#if NET7_0_OR_GREATER
    [TestCase(typeof(int), typeof(INumber<int>), nameof(INumber<int>.Sign), true)]
#endif
    public void TestGetInterfaceMethodImplementationWhenValidArgumentsThenSucceeds(
        Type implementationType,
        Type interfaceType,
        string interfaceMethodName,
        bool isStatic)
    {
        var interfaceMethodInfo = interfaceType
            .GetMethods(BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance))
            .SingleOrDefault(info => info.Name == interfaceMethodName)
            .AssertNotNull();

        var foundMethodInfo = implementationType.GetInterfaceMethodImplementation(interfaceMethodInfo);
        Assert.That(foundMethodInfo, Is.Not.Null);
        Assert.That(foundMethodInfo.DeclaringType, Is.EqualTo(implementationType.IsArray ? typeof(Array) : implementationType));
        Assert.That(foundMethodInfo.ReflectedType, Is.EqualTo(implementationType));

        var expectedName = foundMethodInfo.IsPrivate ? interfaceType.FullName + Type.Delimiter + interfaceMethodName : interfaceMethodName;
        Assert.That(foundMethodInfo.Name, Is.EqualTo(expectedName));

        Assert.That(foundMethodInfo.ReturnType, Is.EqualTo(interfaceMethodInfo.ReturnType));
        Assert.That(GetParameterTypes(foundMethodInfo), Is.EqualTo(GetParameterTypes(interfaceMethodInfo)));
    }

    private static Type[] GetParameterTypes(MethodBase methodInfo) => methodInfo.GetParameters().Select(info => info.ParameterType).ToArray();

    private static string DummyMethod(int value) => throw new NotSupportedException();
}