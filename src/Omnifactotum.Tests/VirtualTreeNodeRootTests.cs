using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(VirtualTreeNodeRoot<>))]
    internal sealed class VirtualTreeNodeRootTests : VirtualTreeNodeRootTestsBase
    {
        protected override VirtualTreeNodeRoot<T> CreateTestee<T>() => new();

        protected override VirtualTreeNodeRoot<T> CreateTestee<T>(IReadOnlyCollection<VirtualTreeNode<T>> children) => new(children);
    }
}