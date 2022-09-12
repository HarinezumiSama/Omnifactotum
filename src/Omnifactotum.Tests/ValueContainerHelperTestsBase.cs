using System;

namespace Omnifactotum.Tests;

internal abstract class ValueContainerHelperTestsBase<TValue> : ValueContainerTestsBase<TValue>
    where TValue : IEquatable<TValue>
{
    protected ValueContainerHelperTestsBase(TValue value, TValue anotherValue)
        : base(value, anotherValue)
    {
        // Nothing to do
    }

    protected sealed override ValueContainer<TValue> CreateContainer(TValue value) => ValueContainer.Create(value);
}