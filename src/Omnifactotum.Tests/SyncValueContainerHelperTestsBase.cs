using System;

namespace Omnifactotum.Tests
{
    internal abstract class SyncValueContainerHelperTestsBase<TValue> : SyncValueContainerTestsBase<TValue>
        where TValue : IEquatable<TValue>
    {
        protected SyncValueContainerHelperTestsBase(TValue value, TValue anotherValue)
            : base(value, anotherValue)
        {
            // Nothing to do
        }

        protected sealed override SyncValueContainer<TValue> CreateContainer(TValue value) => SyncValueContainer.Create(value);

        protected sealed override SyncValueContainer<TValue> CreateContainer(TValue value, object syncObject) => SyncValueContainer.Create(value, syncObject);
    }
}