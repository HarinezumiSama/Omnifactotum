using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests;

internal abstract class VirtualTreeNodeRootTestsBase
{
    [Test]
    public void TestCreationWithNoParameters()
    {
        var nodeRoot = CreateTestee<int>();
        Assert.That(nodeRoot, Is.Not.Null);
        Assert.That(() => nodeRoot.Children, Is.Not.Null.And.Empty);
    }

    [Test]
    public void TestCreationWithChildren()
    {
        const int ChildValue0 = 23;
        const int ChildValue1 = 29;
        const int ChildValue2 = 31;

        Assert.That(new[] { ChildValue0, ChildValue1, ChildValue2 }, Is.Unique);

        var nodeRoot = CreateTestee(new[] { CreateNode(ChildValue0), CreateNode(ChildValue1), CreateNode(ChildValue2) });

        Assert.That(nodeRoot, Is.Not.Null);
        Assert.That(() => nodeRoot.Children, Is.Not.Null);
        Assert.That(() => nodeRoot.Children.Count, Is.EqualTo(3));

        Assert.That(() => nodeRoot.Children[0], Is.Not.Null);
        Assert.That(() => nodeRoot.Children[0].Value, Is.EqualTo(ChildValue0));
        Assert.That(() => nodeRoot.Children[0].Parent, Is.SameAs(nodeRoot));
        Assert.That(() => nodeRoot.Children[0].Children, Is.Not.Null.And.Empty);

        Assert.That(() => nodeRoot.Children[1], Is.Not.Null);
        Assert.That(() => nodeRoot.Children[1].Value, Is.EqualTo(ChildValue1));
        Assert.That(() => nodeRoot.Children[1].Parent, Is.SameAs(nodeRoot));
        Assert.That(() => nodeRoot.Children[1].Children, Is.Not.Null.And.Empty);

        Assert.That(() => nodeRoot.Children[2], Is.Not.Null);
        Assert.That(() => nodeRoot.Children[2].Value, Is.EqualTo(ChildValue2));
        Assert.That(() => nodeRoot.Children[2].Parent, Is.SameAs(nodeRoot));
        Assert.That(() => nodeRoot.Children[2].Children, Is.Not.Null.And.Empty);
    }

    [Test]
    public void TestToString()
    {
        var nodeNode1 = CreateTestee<int>();
        Assert.That(nodeNode1, Is.Not.Null);
        Assert.That(() => nodeNode1.ToString(), Is.EqualTo("VirtualTreeNodeRoot<int>: Children.Count = 0"));

        const int ChildValue0 = 7;
        const int ChildValue1 = 11;
        const int ChildValue2 = 13;

        var nodeNode2 = CreateTestee(new[] { CreateNode(ChildValue0), CreateNode(ChildValue1), CreateNode(ChildValue2) });
        Assert.That(nodeNode2, Is.Not.Null);
        Assert.That(() => nodeNode2.ToString(), Is.EqualTo("VirtualTreeNodeRoot<int>: Children.Count = 3"));
    }

    protected abstract VirtualTreeNodeRoot<T> CreateTestee<T>();

    protected abstract VirtualTreeNodeRoot<T> CreateTestee<T>(IReadOnlyCollection<VirtualTreeNode<T>> children);

    private static VirtualTreeNode<T> CreateNode<T>(T value, IReadOnlyCollection<VirtualTreeNode<T>>? children = null) => new(value, children);
}