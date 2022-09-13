using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;

#pragma warning disable CA1822 //// Mark members as static

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumGenericObjectExtensions))]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local", Justification = "Multiple target frameworks.")]
internal sealed class OmnifactotumGenericObjectExtensionsTests
{
    private const string? NullString = null;
    private const object? NullObject = null;

    [Test]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestEnsureNotNullForReferenceTypeSucceeds()
    {
        var emptyString = string.Empty;
        Assert.That(() => emptyString.EnsureNotNull(), Is.SameAs(emptyString));

        var someObject = new object();
        Assert.That(() => someObject.EnsureNotNull(), Is.SameAs(someObject));

#if NET5_0_OR_GREATER
        const string expectedNullObjectFailureMessage = $"The following expression is null: {{ {nameof(NullObject)} }}. (Parameter 'value')";
#elif NETCOREAPP3_1_OR_GREATER
        const string expectedNullObjectFailureMessage = $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown. (Parameter 'value')";
#else
        var expectedNullObjectFailureMessage =
            $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown.{Environment.NewLine}Parameter name: value";
#endif

        Assert.That(() => NullObject.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(expectedNullObjectFailureMessage));

#if NET5_0_OR_GREATER
        const string expectedExpressionFailureMessage =
            $"The following expression is null: {{ new {nameof(RecursiveNode)}().Parent?.Parent?.Value }}. (Parameter 'value')";
#elif NETCOREAPP3_1_OR_GREATER
        const string expectedExpressionFailureMessage = $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown. (Parameter 'value')";
#else
        var expectedExpressionFailureMessage =
            $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown.{Environment.NewLine}Parameter name: value";
#endif

        Assert.That(
            () => (new RecursiveNode().Parent?.Parent?.Value).EnsureNotNull(),
            Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(expectedExpressionFailureMessage));
    }

    [Test]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public void TestEnsureNotNullForNullableValueTypeSucceeds()
    {
        int? someValue = 42;
        Assert.That(() => someValue.EnsureNotNull(), Is.EqualTo(someValue.Value));

#if NET5_0_OR_GREATER
        const string expectedExpressionFailureMessage =
            $"The following expression is null: {{ (int?)null }}. (Parameter 'value')";
#elif NETCOREAPP3_1_OR_GREATER
        const string expectedExpressionFailureMessage = $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown. (Parameter 'value')";
#else
        var expectedExpressionFailureMessage =
            $"Exception of type '{nameof(System)}.{nameof(ArgumentNullException)}' was thrown.{Environment.NewLine}Parameter name: value";
#endif

        Assert.That(() => ((int?)null).EnsureNotNull(), Throws.TypeOf<ArgumentNullException>().With.Message.EqualTo(expectedExpressionFailureMessage));
    }

    [Test]
    [SetCulture("ru-RU")]
    public void TestToUIStringSucceeds()
    {
        Assert.That(((int?)null).ToUIString(), Is.EqualTo("null"));
        Assert.That(((int?)1234567).ToUIString(), Is.EqualTo("1234567"));

        Assert.That(((DateTime?)null).ToUIString(), Is.EqualTo("null"));

        Assert.That(
            ((DateTime?)new DateTime(2016, 11, 19, 22, 14, 13)).ToUIString(),
            Is.EqualTo("11/19/2016 22:14:13"));
    }

    [Test]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public void TestToUIStringWithFormatAndFormatProviderSucceeds()
    {
        const string NumberFormat = @"N0";

        //// ReSharper disable StringLiteralTypo :: Date/time format specifiers
        const string RussianDateFormat = @"dddd', 'd' 'MMMM' 'yyyy' г. 'HH':'mm':'ss";
        const string JapaneseDateFormat = @"yyyy'年'MMMMd'日'dddd' 'HH':'mm':'ss";
        //// ReSharper restore StringLiteralTypo

        var russianCultureInfo = new CultureInfo(@"ru-RU");
        var japaneseCultureInfo = new CultureInfo(@"ja-JP");

        int? nullableIntegerNull = null;
        int? nullableInteger = 1234567;
        var nullableDateTime = (DateTime?)new DateTime(2016, 11, 19, 22, 14, 13);

        Assert.That(nullableIntegerNull.ToUIString(NumberFormat, russianCultureInfo), Is.EqualTo(@"null"));
        Assert.That(nullableIntegerNull.ToUIString(NumberFormat, japaneseCultureInfo), Is.EqualTo(@"null"));

        Assert.That(nullableInteger.ToUIString(NumberFormat, russianCultureInfo), Is.EqualTo(@"1 234 567"));
        Assert.That(nullableInteger.ToUIString(NumberFormat, japaneseCultureInfo), Is.EqualTo(@"1,234,567"));

        Assert.That(((DateTime?)null).ToUIString(RussianDateFormat, russianCultureInfo), Is.EqualTo(@"null"));
        Assert.That(((DateTime?)null).ToUIString(JapaneseDateFormat, japaneseCultureInfo), Is.EqualTo(@"null"));

        //// ReSharper disable StringLiteralTypo :: False detection (not English)
        Assert.That(
            nullableDateTime.ToUIString(RussianDateFormat, russianCultureInfo),
            Is.EqualTo(@"суббота, 19 ноября 2016 г. 22:14:13"));
        //// ReSharper restore StringLiteralTypo

        Assert.That(
            nullableDateTime.ToUIString(JapaneseDateFormat, japaneseCultureInfo),
            Is.EqualTo(@"2016年11月19日土曜日 22:14:13"));
    }

    [Test]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public void TestToUIStringWithFormatProviderSucceeds()
    {
        var russianCultureInfo = new CultureInfo("ru-RU");
        var japaneseCultureInfo = new CultureInfo("ja-JP");

        int? nullableIntegerNull = null;
        int? nullableInteger = 1234567;
        var nullableDateTime = (DateTime?)new DateTime(2016, 11, 19, 22, 14, 23);

        Assert.That(nullableIntegerNull.ToUIString(russianCultureInfo), Is.EqualTo("null"));
        Assert.That(nullableIntegerNull.ToUIString(japaneseCultureInfo), Is.EqualTo("null"));

        Assert.That(nullableInteger.ToUIString(russianCultureInfo), Is.EqualTo("1234567"));
        Assert.That(nullableInteger.ToUIString(japaneseCultureInfo), Is.EqualTo("1234567"));

        Assert.That(((DateTime?)null).ToUIString(russianCultureInfo), Is.EqualTo("null"));
        Assert.That(((DateTime?)null).ToUIString(japaneseCultureInfo), Is.EqualTo("null"));

        Assert.That(
            nullableDateTime.ToUIString(russianCultureInfo),
            Is.EqualTo("19.11.2016 22:14:23"));

        Assert.That(
            nullableDateTime.ToUIString(japaneseCultureInfo),
            Is.EqualTo("2016/11/19 22:14:23"));
    }

    [SuppressMessage("Microsoft.CodeAnalysis.CSharp", "RS1024")]
    [Test]
    public void TestGetObjectReferenceDescription()
    {
        Assert.That(() => default(TestClass).GetObjectReferenceDescription(), Is.EqualTo("null"));

        var obj1 = new TestClass();
        var hashCode1 = RuntimeHelpers.GetHashCode(obj1);

        Assert.That(
            () => obj1.GetObjectReferenceDescription(),
            Is.EqualTo(
                $@"Omnifactotum.Tests.ExtensionMethods.OmnifactotumGenericObjectExtensionsTests.TestClass:0x{hashCode1:X8}"));

        var obj2 = new object();
        var hashCode2 = RuntimeHelpers.GetHashCode(obj2);
        Assert.That(() => obj2.GetObjectReferenceDescription(), Is.EqualTo($@"System.Object:0x{hashCode2:X8}"));

        var obj3 = new string('w', 17);
        var hashCode3 = RuntimeHelpers.GetHashCode(obj3);
        Assert.That(() => obj3.GetObjectReferenceDescription(), Is.EqualTo($@"System.String:0x{hashCode3:X8}"));
    }

    [SuppressMessage("Microsoft.CodeAnalysis.CSharp", "RS1024")]
    [Test]
    public void TestGetShortObjectReferenceDescription()
    {
        Assert.That(() => default(TestClass).GetObjectReferenceDescription(), Is.EqualTo("null"));

        var obj1 = new TestClass();
        var hashCode1 = RuntimeHelpers.GetHashCode(obj1);

        Assert.That(
            () => obj1.GetShortObjectReferenceDescription(),
            Is.EqualTo($@"OmnifactotumGenericObjectExtensionsTests.TestClass:0x{hashCode1:X8}"));

        var obj2 = new object();
        var hashCode2 = RuntimeHelpers.GetHashCode(obj2);
        Assert.That(() => obj2.GetShortObjectReferenceDescription(), Is.EqualTo($@"object:0x{hashCode2:X8}"));

        var obj3 = new string('z', 17);
        var hashCode3 = RuntimeHelpers.GetHashCode(obj3);
        Assert.That(() => obj3.GetShortObjectReferenceDescription(), Is.EqualTo($@"string:0x{hashCode3:X8}"));
    }

    [Test]
    public void TestAsArraySucceeds()
    {
        const int IntValue = 17;
        const string StringValue = "eaacda5096aa41048c86cdbccc27ed03";
        var obj = new object();

        Assert.That(() => IntValue.AsArray(), Is.TypeOf<int[]>().And.EqualTo(new[] { IntValue }));
        Assert.That(() => StringValue.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { StringValue }));
        Assert.That(() => obj.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { obj }));
        Assert.That(() => NullString.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { NullString }));
        Assert.That(() => NullObject.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { NullObject }));
    }

    [Test]
    public void TestAsListSucceeds()
    {
        const int IntValue = 13;
        const string StringValue = "3037df31af4d426b8edc4469bdf0744c";
        var obj = new object();

        Assert.That(() => IntValue.AsList(), Is.TypeOf<List<int>>().And.EqualTo(new[] { IntValue }));
        Assert.That(() => StringValue.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { StringValue }));
        Assert.That(() => obj.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { obj }));
        Assert.That(() => NullString.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { NullString }));
        Assert.That(() => NullObject.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { NullObject }));
    }

    [Test]
    public void TestAsCollectionSucceeds()
    {
        const int IntValue = 29;
        const string StringValue = "f00136299c4249e5b5ed8d3eb18bbfb4";
        var obj = new object();

        Assert.That(
            () => IntValue.AsCollection(),
            Is.InstanceOf<IEnumerable<int>>().And.EqualTo(new[] { IntValue }));

        Assert.That(
            () => StringValue.AsCollection(),
            Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { StringValue }));

        Assert.That(() => obj.AsCollection(), Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { obj }));

        Assert.That(
            () => NullString.AsCollection(),
            Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { NullString }));

        Assert.That(
            () => NullObject.AsCollection(),
            Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { NullObject }));
    }

    [Test]
    [TestCase(int.MinValue)]
    [TestCase(0)]
    [TestCase(17)]
    [TestCase(42)]
    [TestCase(int.MaxValue)]
    public void TestAsNullableWhenInt32Value(int value)
    {
        var nullableIntValue = value.AsNullable();
        Assert.That(nullableIntValue, Is.EqualTo(value));
    }

    [Test]
    public void TestAsNullableWhenDateTimeKindValue([Values] DateTimeKind value)
    {
        var nullableIntValue = value.AsNullable();
        Assert.That(nullableIntValue, Is.EqualTo(value));
    }

    [Test]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void TestAvoidNullWhenDefaultValueProviderIsNullThenThrows()
        => Assert.That(() => new object().AvoidNull(null!), Throws.ArgumentNullException);

    [Test]
    public void TestAvoidNullWhenNullValueIsPassedAndDefaultValueProviderReturnsNullThenThrows()
        => Assert.That(() => ((TestClass?)null).AvoidNull(() => null!), Throws.InvalidOperationException);

    [Test]
    public void TestAvoidNullWhenNonNullValueIsPassedThenSucceedsAndReturnsPassedValue()
    {
        var input = new TestClass();
        Assert.That(() => input.AvoidNull(() => null!), Is.SameAs(input));
        Assert.That(() => input.AvoidNull(() => new TestClass()), Is.SameAs(input));
    }

    [Test]
    public void TestAvoidNullWhenNullValueIsPassedThenSucceedsAndReturnsValueProvidedByDefaultValueProvider()
    {
        var output = new TestClass();
        Assert.That(() => ((TestClass?)null).AvoidNull(() => output), Is.SameAs(output));
    }

    [Test]
    public void TestGetHashCodeSafelyWithDefaultNullValueHashCodeSucceeds()
    {
        const int IntValue = 17;
        Assert.That(() => IntValue.GetHashCodeSafely(), Is.EqualTo(IntValue.GetHashCode()));

        const string StringValue = "a9fd0a6ce1824e9596b2705611754182";
        Assert.That(() => StringValue.GetHashCodeSafely(), Is.EqualTo(StringValue.GetHashCode()));

        Assert.That(() => ((int?)null).GetHashCodeSafely(), Is.EqualTo(0));
        Assert.That(() => ((string?)null).GetHashCodeSafely(), Is.EqualTo(0));
        Assert.That(() => ((object?)null).GetHashCodeSafely(), Is.EqualTo(0));
        Assert.That(() => ((TestClass?)null).GetHashCodeSafely(), Is.EqualTo(0));
    }

    [Test]
    public void TestGetHashCodeSafelyWithSpecifiedNullValueHashCodeSucceeds()
    {
        const int NullValueHashCode = 1021;

        const int IntValue = 19;
        Assert.That(() => IntValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(IntValue.GetHashCode()));

        const string StringValue = "488066fdd8764ba99dedfc6457751c6e";
        Assert.That(() => StringValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(StringValue.GetHashCode()));

        Assert.That(() => ((int?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
        Assert.That(() => ((string?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
        Assert.That(() => ((object?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
        Assert.That(() => ((TestClass?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
    }

    [Test]
    public void TestIsEqualByContentsToSucceeds()
    {
        //// TODO [HarinezumiSama] Consider MarshalByRefObject when it's a proxy

        const string ValueA = "A";
        const string ValueB = "B";
        const string ValueC = "C";
        const string ValueD = "D";

        Assert.That(new[] { ValueA, ValueB, ValueC, ValueD }, Is.Unique);

        var nodeA = new RecursiveNode { Value = ValueA };
        var nodeB = new RecursiveNode { Value = ValueB, Parent = nodeA };
        nodeA.Parent = nodeB;

        Assert.That(nodeA, Is.Not.AssignableTo<IEquatable<RecursiveNode>>());

        var nodeC1 = new RecursiveNode { Value = ValueC, Parent = nodeA };
        var nodeC2 = new RecursiveNode { Value = ValueC, Parent = nodeA };
        var nodeDParentA = new RecursiveNode { Value = ValueD, Parent = nodeA };
        var nodeDParentB = new RecursiveNode { Value = ValueD, Parent = nodeB };

        AssertResultForIsEqualByContentsTo(nodeC1, nodeC1, Is.True);
        AssertResultForIsEqualByContentsTo(nodeC1, default, Is.False);
        AssertResultForIsEqualByContentsTo(nodeC1, nodeC2, Is.True);
        AssertResultForIsEqualByContentsTo(nodeC1, nodeDParentA, Is.False);
        AssertResultForIsEqualByContentsTo(nodeDParentA, nodeDParentB, Is.False);

        AssertResultForIsEqualByContentsTo<object>("13", "13", Is.True);
        AssertResultForIsEqualByContentsTo<object>(17, 17, Is.True);
        AssertResultForIsEqualByContentsTo<object>("19", 19, Is.False);

        const string ContainerValue = "92b6e84bba8e429b9ec8593d2594354e";
        var simpleContainer = new SimpleContainer { Value = ContainerValue };
        var descendantContainer1 = new DescendantContainer1 { Value = ContainerValue };
        var descendantContainer2 = new DescendantContainer2 { Value = ContainerValue };

        AssertResultForIsEqualByContentsTo(simpleContainer, descendantContainer1, Is.False);
        AssertResultForIsEqualByContentsTo<SimpleContainer>(descendantContainer1, descendantContainer2, Is.False);

        var classWithNoFields1 = new ClassWithNoFields();
        var classWithNoFields2 = new ClassWithNoFields();
        AssertResultForIsEqualByContentsTo(classWithNoFields1, classWithNoFields2, Is.True);

        var structureWithNoFields1 = new StructureWithNoFields();
        var structureWithNoFields2 = new StructureWithNoFields();
        AssertResultForIsEqualByContentsTo(structureWithNoFields1, structureWithNoFields2, Is.True);

        AssertResultForIsEqualByContentsTo<object>(classWithNoFields1, structureWithNoFields1, Is.False);
    }

    [Test]
    [SetCulture("ru-RU")]
    public void TestToStringSafelyWithDefaultNullValueStringSucceeds()
    {
        Assert.That(() => ((TestClass?)null).ToStringSafely(), Is.EqualTo(string.Empty));
        Assert.That(() => new TestClass().ToStringSafely(), Is.EqualTo(typeof(TestClass).FullName));

        Assert.That(() => ((long?)null).ToStringSafely(), Is.EqualTo(string.Empty));
        Assert.That(() => ((long?)123456789).ToStringSafely(), Is.EqualTo("123456789"));

        Assert.That(() => ((DateTime?)null).ToStringSafely(), Is.EqualTo(string.Empty));
        Assert.That(
            () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafely(),
            Is.EqualTo("17.11.2017 21:10:44"));

        Assert.That(() => true.ToStringSafely(), Is.EqualTo(bool.TrueString));
        Assert.That(() => FileMode.OpenOrCreate.ToStringSafely(), Is.EqualTo(nameof(FileMode.OpenOrCreate)));
    }

    [Test]
    [SetCulture("ru-RU")]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("default")]
    public void TestToStringSafelyWithSpecifiedNullValueStringSucceeds(string? fallbackResult)
    {
        var expectedResultForNullInput = fallbackResult ?? string.Empty;

        Assert.That(() => ((TestClass?)null).ToStringSafely(fallbackResult), Is.EqualTo(expectedResultForNullInput));
        Assert.That(() => new TestClass().ToStringSafely(fallbackResult), Is.EqualTo(typeof(TestClass).FullName));

        Assert.That(() => ((long?)null).ToStringSafely(fallbackResult), Is.EqualTo(expectedResultForNullInput));
        Assert.That(() => ((long?)42).ToStringSafely(fallbackResult), Is.EqualTo("42"));

        Assert.That(() => ((DateTime?)null).ToStringSafely(fallbackResult), Is.EqualTo(expectedResultForNullInput));

        Assert.That(
            () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafely(expectedResultForNullInput),
            Is.EqualTo("17.11.2017 21:10:44"));

        Assert.That(() => true.ToStringSafely(fallbackResult), Is.EqualTo(bool.TrueString));

        Assert.That(
            () => FileMode.OpenOrCreate.ToStringSafely(fallbackResult),
            Is.EqualTo(nameof(FileMode.OpenOrCreate)));
    }

    [Test]
    [SetCulture("ru-RU")]
    public void TestToStringSafelyInvariantWithDefaultNullValueStringSucceeds()
    {
        Assert.That(() => ((TestClass?)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
        Assert.That(() => new TestClass().ToStringSafelyInvariant(), Is.EqualTo(typeof(TestClass).FullName));

        Assert.That(() => ((long?)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
        Assert.That(() => ((long?)123456789).ToStringSafelyInvariant(), Is.EqualTo("123456789"));

        Assert.That(() => ((DateTime?)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
        Assert.That(
            () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafelyInvariant(),
            Is.EqualTo("11/17/2017 21:10:44"));

        Assert.That(() => true.ToStringSafelyInvariant(), Is.EqualTo(bool.TrueString));
        Assert.That(
            () => FileMode.OpenOrCreate.ToStringSafelyInvariant(),
            Is.EqualTo(nameof(FileMode.OpenOrCreate)));
    }

    [Test]
    [SetCulture("ru-RU")]
    public void TestToStringSafelyInvariantWithSpecifiedNullValueStringSucceeds()
    {
        const string NullValueString = "default";

        Assert.That(() => ((TestClass?)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
        Assert.That(
            () => new TestClass().ToStringSafelyInvariant(NullValueString),
            Is.EqualTo(typeof(TestClass).FullName));

        Assert.That(() => ((long?)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
        Assert.That(() => ((long?)42).ToStringSafelyInvariant(NullValueString), Is.EqualTo("42"));

        Assert.That(() => ((DateTime?)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
        Assert.That(
            () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafelyInvariant(NullValueString),
            Is.EqualTo("11/17/2017 21:10:44"));

        Assert.That(() => true.ToStringSafelyInvariant(NullValueString), Is.EqualTo(bool.TrueString));
        Assert.That(
            () => FileMode.OpenOrCreate.ToStringSafelyInvariant(NullValueString),
            Is.EqualTo(nameof(FileMode.OpenOrCreate)));
    }

    [Test]
    public void TestGetTypeSafelySucceeds()
    {
        Assert.That(() => ((object?)null).GetTypeSafely(), Is.EqualTo(typeof(object)));
        Assert.That(() => new object().GetTypeSafely(), Is.EqualTo(typeof(object)));

        Assert.That(() => ((int?)null).GetTypeSafely(), Is.EqualTo(typeof(int?)));
        Assert.That(() => 17.GetTypeSafely(), Is.EqualTo(typeof(int)));
        Assert.That(() => ((int?)17).GetTypeSafely(), Is.EqualTo(typeof(int)));

        Assert.That(() => ((TestClass?)null).GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
        Assert.That(() => new TestClass().GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
        Assert.That(() => ((object)new TestClass()).GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
    }

    private static void AssertResultForIsEqualByContentsTo<T>(T valueA, T valueB, IResolveConstraint constraint)
    {
        Assert.That(valueA.IsEqualByContentsTo(valueB), constraint);
        Assert.That(valueB.IsEqualByContentsTo(valueA), constraint);
    }

    private sealed class TestClass
    {
        // No own members
    }

    private class SimpleContainer
    {
        public string? Value
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

    private sealed class DescendantContainer1 : SimpleContainer
    {
        // No own members
    }

    private sealed class DescendantContainer2 : SimpleContainer
    {
        // No own members
    }

    private sealed class ClassWithNoFields
    {
        // No own members
    }

    private sealed class StructureWithNoFields
    {
        // No own members
    }

    private sealed class RecursiveNode
    {
        public string? Value
        {
            [UsedImplicitly]
            get;
            set;
        }

        public RecursiveNode? Parent
        {
            [UsedImplicitly]
            get;
            set;
        }
    }
}