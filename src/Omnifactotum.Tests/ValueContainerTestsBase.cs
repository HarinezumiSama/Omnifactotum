using System;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

internal abstract class ValueContainerTestsBase<TValue> : ValueContainerBaseTestsBase<ValueContainer<TValue>, TValue>
    where TValue : IEquatable<TValue>
{
    protected ValueContainerTestsBase(TValue value, TValue anotherValue)
        : base(value, anotherValue)
    {
        // Nothing to do
    }

    [Test]
    public override void TestPropertyAccess()
    {
        base.TestPropertyAccess();
        NUnitFactotum.For<ValueContainer<TValue>>.AssertReadableWritable(obj => obj.Value, PropertyAccessMode.ReadWrite);
    }

    protected override ValueContainer<TValue> CreateContainer(TValue value) => new(value);

    protected sealed override TValue GetContainerValue(ValueContainer<TValue> container) => container.Value;

    protected sealed override void SetContainerValue(ValueContainer<TValue> container, TValue value) => container.Value = value;

}