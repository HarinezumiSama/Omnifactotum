using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumMethodBaseExtensions))]
internal sealed class OmnifactotumMethodBaseExtensionsTests
{
    private const MethodBase? NullMethod = null;

    private readonly MethodBaseTestHelper _testHelper;

    public OmnifactotumMethodBaseExtensionsTests() => _testHelper = MethodBaseTestHelper.Instance;

    [Test]
    public void TestGetFullNameWhenInvalidArgumentThenThrows()
        => Assert.That(() => NullMethod!.GetFullName(), Throws.ArgumentNullException);

    [Test]
    public void TestGetFullNameWhenConstructorIsPassedThenSucceeds()
    {
        var expected = AsInvariant(
            $@"{nameof(Omnifactotum)}.{nameof(Tests)}.{nameof(ExtensionMethods)}.{
                nameof(MethodBaseTestHelper)}.{_testHelper.InstanceConstructorName}");

        var actual = _testHelper.InstanceConstructorMethod.GetFullName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetFullNameWhenGenericMethodDefinitionIsPassedThenSucceeds()
    {
        var expected = $@"{nameof(Omnifactotum)}.{nameof(Tests)}.{nameof(ExtensionMethods)}.{
            nameof(MethodBaseTestHelper)}.{_testHelper.MethodName}<TReference, TValue>";

        var actual = _testHelper.GenericMethodDefinition.GetFullName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetFullNameWhenGenericMethodIsPassedThenSucceeds()
    {
        var expected =
            AsInvariant(
                $@"{nameof(Omnifactotum)}.{nameof(Tests)}.{nameof(ExtensionMethods)}.{nameof(MethodBaseTestHelper)}.{
                    _testHelper.MethodName}")
            + @"<"
            + @"System.Collections.Generic.List<System.Collections.Generic.Dictionary<System.Attribute, System.Exception>>"
            + @", System.ConsoleKeyInfo"
            + @">";

        var actual = _testHelper.GenericMethod.GetFullName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetQualifiedNameWhenInvalidArgumentThenThrows()
        => Assert.That(() => NullMethod!.GetQualifiedName(), Throws.ArgumentNullException);

    [Test]
    public void TestGetQualifiedNameWhenConstructorIsPassedThenSucceeds()
    {
        var expected = AsInvariant($@"{nameof(MethodBaseTestHelper)}.{_testHelper.InstanceConstructorName}");

        var actual = _testHelper.InstanceConstructorMethod.GetQualifiedName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetQualifiedNameWhenGenericMethodDefinitionIsPassedThenSucceeds()
    {
        var expected = AsInvariant($@"{nameof(MethodBaseTestHelper)}.{_testHelper.MethodName}<TReference, TValue>");

        var actual = _testHelper.GenericMethodDefinition.GetQualifiedName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetQualifiedNameWhenGenericMethodIsPassedThenSucceeds()
    {
        var expected =
            AsInvariant($@"{nameof(MethodBaseTestHelper)}.{_testHelper.MethodName}")
            + @"<List<Dictionary<Attribute, Exception>>, ConsoleKeyInfo>";

        var actual = _testHelper.GenericMethod.GetQualifiedName();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetSignatureWhenInvalidArgumentThenThrows()
        => Assert.That(() => NullMethod!.GetSignature(), Throws.ArgumentNullException);

    [Test]
    public void TestGetSignatureWhenConstructorIsPassedThenSucceeds()
    {
        var expected = AsInvariant($@"{nameof(MethodBaseTestHelper)}.{_testHelper.InstanceConstructorName}()");

        var actual = _testHelper.InstanceConstructorMethod.GetSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetSignatureWhenGenericMethodDefinitionIsPassedThenSucceeds()
    {
        var expected =
            AsInvariant(
                $@"{nameof(MethodBase)} {nameof(MethodBaseTestHelper)}.{
                    _testHelper.MethodName}<TReference, TValue>(TReference, ref TReference, out TReference")
            + @", TValue, ref TValue, out TValue, TValue?, ref TValue?, out TValue?"
            + @", TReference[], ref TReference[], out TReference[], TValue[], ref TValue[], out TValue[]"
            + @", TValue?[], ref TValue?[], out TValue?[], string, ref string, out string"
            + @", long, ref long, out long, long?, ref long?, out long?, long*, ref long*, out long*"
            + @", string[], ref string[], out string[], long[], ref long[], out long[]"
            + @", long?[], ref long?[], out long?[], long*[], ref long*[], out long*[])";

        var actual = _testHelper.GenericMethodDefinition.GetSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetSignatureWhenGenericMethodIsPassedThenSucceeds()
    {
        const string ReferenceType =
            nameof(List<object>) + "<" + nameof(Dictionary<object, object>)
            + "<" + nameof(Attribute) + ", " + nameof(Exception) + ">>";

        const string ValueType = nameof(ConsoleKeyInfo);

        var expected =
            AsInvariant(
                $@"{nameof(MethodBase)} {nameof(MethodBaseTestHelper)}.{
                    _testHelper.MethodName}<{ReferenceType}, {ValueType}>(")
            + AsInvariant($@"{ReferenceType}, ref {ReferenceType}, out {ReferenceType}")
            + AsInvariant($@", {ValueType}, ref {ValueType}, out {ValueType}")
            + AsInvariant($@", {ValueType}?, ref {ValueType}?, out {ValueType}?")
            + AsInvariant($@", {ReferenceType}[], ref {ReferenceType}[], out {ReferenceType}[]")
            + AsInvariant($@", {ValueType}[], ref {ValueType}[], out {ValueType}[]")
            + AsInvariant($@", {ValueType}?[], ref {ValueType}?[], out {ValueType}?[]")
            + @", string, ref string, out string, long, ref long, out long, long?, ref long?, out long?"
            + @", long*, ref long*, out long*, string[], ref string[], out string[]"
            + @", long[], ref long[], out long[], long?[], ref long?[], out long?[]"
            + @", long*[], ref long*[], out long*[])";

        var actual = _testHelper.GenericMethod.GetSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetFullSignatureWhenInvalidArgumentThenThrows()
        => Assert.That(() => NullMethod!.GetFullSignature(), Throws.ArgumentNullException);

    [Test]
    public void TestGetFullSignatureWhenConstructorIsPassedThenSucceeds()
    {
        var expected = AsInvariant(
            $@"{nameof(Omnifactotum)}.{nameof(Tests)}.{nameof(ExtensionMethods)}.{
                nameof(MethodBaseTestHelper)}.{_testHelper.InstanceConstructorName}()");

        var actual = _testHelper.InstanceConstructorMethod.GetFullSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetFullSignatureWhenGenericMethodDefinitionIsPassedThenSucceeds()
    {
        var expected = AsInvariant(
                $@"{nameof(System)}.{nameof(System.Reflection)}.{nameof(MethodBase)} {
                    nameof(Omnifactotum)}.{nameof(Tests)}.{nameof(ExtensionMethods)}.{
                        nameof(MethodBaseTestHelper)}.{
                            _testHelper.MethodName}<TReference, TValue>(TReference, ref TReference, out TReference")
            + @", TValue, ref TValue, out TValue"
            + @", System.Nullable<TValue>, ref System.Nullable<TValue>, out System.Nullable<TValue>"
            + @", TReference[], ref TReference[], out TReference[], TValue[], ref TValue[], out TValue[]"
            + @", System.Nullable<TValue>[], ref System.Nullable<TValue>[], out System.Nullable<TValue>[]"
            + @", System.String, ref System.String, out System.String"
            + @", System.Int64, ref System.Int64, out System.Int64"
            + @", System.Nullable<System.Int64>, ref System.Nullable<System.Int64>, out System.Nullable<System.Int64>"
            + @", System.Int64*, ref System.Int64*, out System.Int64*"
            + @", System.String[], ref System.String[], out System.String[]"
            + @", System.Int64[], ref System.Int64[], out System.Int64[]"
            + @", System.Nullable<System.Int64>[], ref System.Nullable<System.Int64>[], out System.Nullable<System.Int64>[]"
            + @", System.Int64*[], ref System.Int64*[], out System.Int64*[])";

        var actual = _testHelper.GenericMethodDefinition.GetFullSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestGetFullSignatureWhenGenericMethodIsPassedThenSucceeds()
    {
        const string ReferenceType =
            nameof(System) + "." + nameof(System.Collections) + "." + nameof(System.Collections.Generic)
            + "." + nameof(List<object>) + "<"
            + nameof(System) + "." + nameof(System.Collections) + "." + nameof(System.Collections.Generic)
            + "." + nameof(Dictionary<object, object>)
            + "<" + nameof(System) + "." + nameof(Attribute) + ", "
            + nameof(System) + "." + nameof(Exception) + ">>";

        const string ValueType = nameof(System) + "." + nameof(ConsoleKeyInfo);

        var expected =
            AsInvariant(
                $@"{nameof(System)}.{nameof(System.Reflection)}.{nameof(MethodBase)} {nameof(Omnifactotum)}.{
                    nameof(Tests)}.{nameof(ExtensionMethods)}.{nameof(MethodBaseTestHelper)}.{
                        _testHelper.MethodName}<{ReferenceType}, {ValueType}>(")
            + AsInvariant($@"{ReferenceType}, ref {ReferenceType}, out {ReferenceType}")
            + AsInvariant($@", {ValueType}, ref {ValueType}, out {ValueType}")
            + AsInvariant($@", System.Nullable<{ValueType}>, ref System.Nullable<{ValueType}>, out System.Nullable<{ValueType}>")
            + AsInvariant($@", {ReferenceType}[], ref {ReferenceType}[], out {ReferenceType}[]")
            + AsInvariant($@", {ValueType}[], ref {ValueType}[], out {ValueType}[]")
            + AsInvariant(
                $@", System.Nullable<{ValueType}>[], ref System.Nullable<{ValueType}>[], out System.Nullable<{ValueType}>[]")
            + @", System.String, ref System.String, out System.String"
            + @", System.Int64, ref System.Int64, out System.Int64"
            + @", System.Nullable<System.Int64>, ref System.Nullable<System.Int64>, out System.Nullable<System.Int64>"
            + @", System.Int64*, ref System.Int64*, out System.Int64*"
            + @", System.String[], ref System.String[], out System.String[]"
            + @", System.Int64[], ref System.Int64[], out System.Int64[]"
            + @", System.Nullable<System.Int64>[], ref System.Nullable<System.Int64>[], out System.Nullable<System.Int64>[]"
            + @", System.Int64*[], ref System.Int64*[], out System.Int64*[])";

        var actual = _testHelper.GenericMethod.GetFullSignature();
        Assert.That(actual, Is.EqualTo(expected));
    }
}