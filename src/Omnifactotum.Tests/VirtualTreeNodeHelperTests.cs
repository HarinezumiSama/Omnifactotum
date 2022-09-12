using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(VirtualTreeNode))]
internal sealed class VirtualTreeNodeHelperTests : VirtualTreeNodeTestsBase
{
    protected override VirtualTreeNode<T> CreateTestee<T>(T value) => VirtualTreeNode.Create(value);

    protected override VirtualTreeNode<T> CreateTestee<T>(T value, IReadOnlyCollection<VirtualTreeNode<T>> children)
        => VirtualTreeNode.Create(value, children);
}