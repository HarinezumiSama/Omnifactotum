using System;
using System.Collections.Immutable;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Abstractions;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

internal abstract class ValueContainerBaseTestsBase<TContainer, TValue>
    where TContainer : class, IValueContainer<TValue>
    where TValue : IEquatable<TValue>
{
    protected readonly TValue Value;
    protected readonly TValue AnotherValue;
    protected readonly ImmutableArray<TValue> Values;

    protected ValueContainerBaseTestsBase(TValue value, TValue anotherValue)
    {
        Assert.That(new[] { value, anotherValue }, Is.Unique);
        Assert.That(value, Is.Not.EqualTo(default(TValue)));
        Assert.That(value, Is.Not.Null);
        Assert.That(anotherValue, Is.Not.EqualTo(default(TValue)));
        Assert.That(anotherValue, Is.Not.Null);
        Assert.That(anotherValue, Is.Not.EqualTo(value));

        Value = value;
        AnotherValue = anotherValue;
        Values = new[] { value, anotherValue }.ToImmutableArray();
    }

    [Test]
    public void TestSupportedInterfaces() => Assert.That(typeof(IValueContainer<TValue>).IsAssignableFrom(typeof(TContainer)), Is.True);

    [Test]
    public virtual void TestPropertyAccess()
        => NUnitFactotum.For<IValueContainer<TValue>>.AssertReadableWritable(obj => obj.Value, PropertyAccessMode.ReadWrite);

    [Test]
    public void TestConstructionWithValue()
    {
        foreach (var value in Values)
        {
            var container = CreateContainer(value);
            AssertConstructionWithValueTestCase(container, value);
        }
    }

    [Test]
    public abstract void TestConstructionWithValueIncludingDefaultValue();

    [Test]
    public void TestValue()
    {
        var container = CreateContainer(Value);
        Assert.That(GetContainerValue(container), CreateValueEqualityConstraint(Value));

        SetContainerValue(container, AnotherValue);
        Assert.That(GetContainerValue(container), CreateValueEqualityConstraint(AnotherValue));
    }

    [Test]
    public void TestValueViaInterface()
    {
        IValueContainer<TValue> container = CreateContainer(Value);
        Assert.That(container.Value, CreateValueEqualityConstraint(Value));

        container.Value = AnotherValue;
        Assert.That(container.Value, CreateValueEqualityConstraint(AnotherValue));
    }

    [Test]
    public void TestEquality()
    {
        var container1 = CreateContainer(Value);
        var container2 = CreateContainer(Value);
        var containerAnother = CreateContainer(AnotherValue);

        NUnitFactotum.AssertEquality(container1, container1, AssertEqualityExpectation.EqualAndMayBeSame);
        NUnitFactotum.AssertEquality(container1, container2, AssertEqualityExpectation.EqualAndCannotBeSame);
        NUnitFactotum.AssertEquality(container1, containerAnother, AssertEqualityExpectation.NotEqual);
        NUnitFactotum.AssertEquality(container1, null, AssertEqualityExpectation.NotEqual);
    }

    [Test]
    public abstract void TestEqualityIncludingDefaultValue();

    [Test]
    public void TestToString()
    {
        foreach (var value in Values)
        {
            var container = CreateContainer(value);

            Assert.That(
                container.ToString(),
                Is.EqualTo(FormattableStringFactotum.AsInvariant($@"{{ {nameof(IValueContainer<TValue>.Value)} = {value.ToStringSafelyInvariant()} }}")));
        }
    }

    [Test]
    public abstract void TestToStringIncludingDefaultValue();

    protected static IResolveConstraint CreateValueEqualityConstraint(object? value) => typeof(TValue).IsValueType ? Is.EqualTo(value) : Is.SameAs(value);

    protected virtual void AssertConstructionWithValueTestCase(TContainer container, TValue value)
        => Assert.That(GetContainerValue(container), CreateValueEqualityConstraint(value));

    protected abstract TContainer CreateContainer(TValue value);

    protected abstract TValue GetContainerValue(TContainer container);

    protected abstract void SetContainerValue(TContainer container, TValue value);
}