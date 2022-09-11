using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(VirtualTreeNodeRoot))]
    internal sealed class VirtualTreeNodeRootHelperTests : VirtualTreeNodeRootTestsBase
    {
        protected override VirtualTreeNodeRoot<T> CreateTestee<T>() => VirtualTreeNodeRoot.Create<T>();

        protected override VirtualTreeNodeRoot<T> CreateTestee<T>(IReadOnlyCollection<VirtualTreeNode<T>> children) => VirtualTreeNodeRoot.Create(children);
    }
}