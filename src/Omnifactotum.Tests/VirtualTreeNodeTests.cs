using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(VirtualTreeNode<>))]
internal sealed class VirtualTreeNodeTests : VirtualTreeNodeTestsBase
{
    protected override VirtualTreeNode<T> CreateTestee<T>(T value) => new(value);

    protected override VirtualTreeNode<T> CreateTestee<T>(T value, IReadOnlyCollection<VirtualTreeNode<T>> children) => new(value, children);
}