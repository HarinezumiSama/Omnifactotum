using System;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(VirtualTreeNodeCollection<>))]
internal sealed class VirtualTreeNodeCollectionTests
{
    [Test]
    public void TestScenario()
    {
        const string ChildValue0 = nameof(ChildValue0);
        const string ChildValue1 = nameof(ChildValue1);
        const string ChildValue2 = nameof(ChildValue2);
        const string ChildValue3 = nameof(ChildValue3);
        const string NewChildValue0 = nameof(NewChildValue0);
        const string AnotherNewChildValue0 = nameof(AnotherNewChildValue0);

        const string AnotherRootChildValue = nameof(AnotherRootChildValue);

        Assert.That(
            new[] { ChildValue0, ChildValue1, ChildValue2, ChildValue3, NewChildValue0, AnotherNewChildValue0 },
            Is.Unique);

        var childNode0 = new VirtualTreeNode<string>(ChildValue0);
        var childNode1 = new VirtualTreeNode<string>(ChildValue1);
        var childNode2 = new VirtualTreeNode<string>(ChildValue2);
        var childNode3 = new VirtualTreeNode<string>(ChildValue3);
        var newChildNode0 = new VirtualTreeNode<string>(NewChildValue0);
        var anotherNewChildNode0 = new VirtualTreeNode<string>(AnotherNewChildValue0);

        var root = new VirtualTreeNodeRoot<string>();
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.Null);
        Assert.That(() => childNode1.Parent, Is.Null);
        Assert.That(() => childNode2.Parent, Is.Null);
        Assert.That(() => childNode3.Parent, Is.Null);
        Assert.That(() => root.Children, Is.Not.Null);
        Assert.That(() => root.Children.Count, Is.Zero);
        Assert.That(() => root.Children.IsReadOnly, Is.False);
        Assert.That(() => root.Children[0], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode1), Is.False);
        Assert.That(() => root.Children.Contains(childNode2), Is.False);
        Assert.That(() => root.Children.Contains(childNode3), Is.False);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode1), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode2), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode3), Is.Negative);
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 0"));

        Assert.That(() => root.Children.Add(null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.AddRange(null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.Contains(null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.Insert(0, null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.IndexOf(null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.Remove(null!), Throws.ArgumentNullException);
        Assert.That(() => root.Children.CopyTo(null!, 0), Throws.ArgumentNullException);

        root.Children.Add(childNode0);
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.Null);
        Assert.That(() => childNode2.Parent, Is.Null);
        Assert.That(() => childNode3.Parent, Is.Null);
        Assert.That(() => root.Children.Count, Is.EqualTo(1));
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.False);
        Assert.That(() => root.Children.Contains(childNode2), Is.False);
        Assert.That(() => root.Children.Contains(childNode3), Is.False);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode2), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode3), Is.Negative);
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 1"));

        Assert.That(() => root.Children[0] = childNode0, Throws.Nothing);
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));

        root.Children.Add(childNode1);
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.Null);
        Assert.That(() => childNode3.Parent, Is.Null);
        Assert.That(() => root.Children.Count, Is.EqualTo(2));
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode1));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.False);
        Assert.That(() => root.Children.Contains(childNode3), Is.False);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode3), Is.Negative);
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 2"));

        Assert.That(
            () => root.Children.Add(childNode0),
            Throws.ArgumentException.With.Message.StartsWith(
                $@"The item {{ VirtualTreeNode<string>: Children.Count = 0, Value = {ChildValue0} }} already belongs to this collection."));

        root.Children.AddRange(new[] { childNode2, childNode3 });
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.SameAs(root));
        Assert.That(() => childNode3.Parent, Is.SameAs(root));
        Assert.That(() => root.Children.Count, Is.EqualTo(4));
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode1));
        Assert.That(() => root.Children[2], Is.SameAs(childNode2));
        Assert.That(() => root.Children[3], Is.SameAs(childNode3));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.True);
        Assert.That(() => root.Children.Contains(childNode3), Is.True);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.EqualTo(2));
        Assert.That(() => root.Children.IndexOf(childNode3), Is.EqualTo(3));
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 4"));

        var childNodesCopy = new VirtualTreeNode<string>[root.Children.Count];
        root.Children.CopyTo(childNodesCopy, 0);
        Assert.That(() => childNodesCopy.Length, Is.EqualTo(4));
        Assert.That(() => childNodesCopy[0], Is.SameAs(childNode0));
        Assert.That(() => childNodesCopy[1], Is.SameAs(childNode1));
        Assert.That(() => childNodesCopy[2], Is.SameAs(childNode2));
        Assert.That(() => childNodesCopy[3], Is.SameAs(childNode3));

        var anotherRootChildNode = new VirtualTreeNode<string>(AnotherRootChildValue);
        var anotherRoot = new VirtualTreeNodeRoot<string>(new[] { anotherRootChildNode });
        Assert.That(anotherRoot, Is.Not.SameAs(root));
        Assert.That(() => anotherRootChildNode.Parent, Is.SameAs(anotherRoot));

        Assert.That(
            () => root.Children.Add(anotherRootChildNode),
            Throws.ArgumentException.With.Message.StartsWith(
                $@"The item {{ VirtualTreeNode<string>: Children.Count = 0, Value = {AnotherRootChildValue} }} already belongs to another collection."));

        root.Children.Insert(0, newChildNode0);
        Assert.That(() => newChildNode0.Parent, Is.SameAs(root));
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.SameAs(root));
        Assert.That(() => childNode3.Parent, Is.SameAs(root));
        Assert.That(() => root.Children.Count, Is.EqualTo(5));
        Assert.That(() => root.Children[0], Is.SameAs(newChildNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode0));
        Assert.That(() => root.Children[2], Is.SameAs(childNode1));
        Assert.That(() => root.Children[3], Is.SameAs(childNode2));
        Assert.That(() => root.Children[4], Is.SameAs(childNode3));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.True);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.True);
        Assert.That(() => root.Children.Contains(childNode3), Is.True);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(2));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.EqualTo(3));
        Assert.That(() => root.Children.IndexOf(childNode3), Is.EqualTo(4));
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 5"));

        var enumerator = ((System.Collections.IEnumerable)root.Children).GetEnumerator();
        Assert.That(() => enumerator.MoveNext(), Is.True);
        Assert.That(() => enumerator.Current, Is.SameAs(newChildNode0));
        Assert.That(() => enumerator.MoveNext(), Is.True);
        Assert.That(() => enumerator.Current, Is.SameAs(childNode0));
        Assert.That(() => enumerator.MoveNext(), Is.True);
        Assert.That(() => enumerator.Current, Is.SameAs(childNode1));
        Assert.That(() => enumerator.MoveNext(), Is.True);
        Assert.That(() => enumerator.Current, Is.SameAs(childNode2));
        Assert.That(() => enumerator.MoveNext(), Is.True);
        Assert.That(() => enumerator.Current, Is.SameAs(childNode3));
        Assert.That(() => enumerator.MoveNext(), Is.False);

        //// ReSharper disable once RedundantCast
        using var genericEnumerator = ((System.Collections.Generic.IEnumerable<VirtualTreeNode<string>>)root.Children).GetEnumerator();
        Assert.That(() => genericEnumerator.MoveNext(), Is.True);
        Assert.That(() => genericEnumerator.Current, Is.SameAs(newChildNode0));
        Assert.That(() => genericEnumerator.MoveNext(), Is.True);
        Assert.That(() => genericEnumerator.Current, Is.SameAs(childNode0));
        Assert.That(() => genericEnumerator.MoveNext(), Is.True);
        Assert.That(() => genericEnumerator.Current, Is.SameAs(childNode1));
        Assert.That(() => genericEnumerator.MoveNext(), Is.True);
        Assert.That(() => genericEnumerator.Current, Is.SameAs(childNode2));
        Assert.That(() => genericEnumerator.MoveNext(), Is.True);
        Assert.That(() => genericEnumerator.Current, Is.SameAs(childNode3));
        Assert.That(() => genericEnumerator.MoveNext(), Is.False);

        Assert.That(() => root.Children[-1] = new VirtualTreeNode<string>(string.Empty), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => root.Children[0] = null!, Throws.ArgumentNullException);
        Assert.That(() => root.Children[1] = null!, Throws.ArgumentNullException);
        Assert.That(() => root.Children[2] = null!, Throws.ArgumentNullException);
        Assert.That(() => root.Children[3] = null!, Throws.ArgumentNullException);
        Assert.That(() => root.Children[4] = null!, Throws.ArgumentNullException);
        Assert.That(() => root.Children[5] = new VirtualTreeNode<string>(string.Empty), Throws.TypeOf<ArgumentOutOfRangeException>());

        root.Children[0] = anotherNewChildNode0;
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.SameAs(root));
        Assert.That(() => childNode3.Parent, Is.SameAs(root));
        Assert.That(() => root.Children.Count, Is.EqualTo(5));
        Assert.That(() => root.Children[0], Is.SameAs(anotherNewChildNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode0));
        Assert.That(() => root.Children[2], Is.SameAs(childNode1));
        Assert.That(() => root.Children[3], Is.SameAs(childNode2));
        Assert.That(() => root.Children[4], Is.SameAs(childNode3));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.True);
        Assert.That(() => root.Children.Contains(childNode3), Is.True);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(2));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.EqualTo(3));
        Assert.That(() => root.Children.IndexOf(childNode3), Is.EqualTo(4));
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 5"));

        Assert.That(() => root.Children.Remove(anotherNewChildNode0), Is.True);
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.SameAs(root));
        Assert.That(() => childNode3.Parent, Is.SameAs(root));
        Assert.That(() => root.Children.Count, Is.EqualTo(4));
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode1));
        Assert.That(() => root.Children[2], Is.SameAs(childNode2));
        Assert.That(() => root.Children[3], Is.SameAs(childNode3));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.True);
        Assert.That(() => root.Children.Contains(childNode3), Is.True);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.EqualTo(2));
        Assert.That(() => root.Children.IndexOf(childNode3), Is.EqualTo(3));
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 4"));

        root.Children.RemoveAt(3);
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.SameAs(root));
        Assert.That(() => childNode1.Parent, Is.SameAs(root));
        Assert.That(() => childNode2.Parent, Is.SameAs(root));
        Assert.That(() => childNode3.Parent, Is.Null);
        Assert.That(() => root.Children.Count, Is.EqualTo(3));
        Assert.That(() => root.Children[0], Is.SameAs(childNode0));
        Assert.That(() => root.Children[1], Is.SameAs(childNode1));
        Assert.That(() => root.Children[2], Is.SameAs(childNode2));
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.True);
        Assert.That(() => root.Children.Contains(childNode1), Is.True);
        Assert.That(() => root.Children.Contains(childNode2), Is.True);
        Assert.That(() => root.Children.Contains(childNode3), Is.False);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.EqualTo(0));
        Assert.That(() => root.Children.IndexOf(childNode1), Is.EqualTo(1));
        Assert.That(() => root.Children.IndexOf(childNode2), Is.EqualTo(2));
        Assert.That(() => root.Children.IndexOf(childNode3), Is.Negative);
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 3"));

        root.Children.Clear();
        Assert.That(() => newChildNode0.Parent, Is.Null);
        Assert.That(() => anotherNewChildNode0.Parent, Is.Null);
        Assert.That(() => childNode0.Parent, Is.Null);
        Assert.That(() => childNode1.Parent, Is.Null);
        Assert.That(() => childNode2.Parent, Is.Null);
        Assert.That(() => childNode3.Parent, Is.Null);
        Assert.That(() => root.Children, Is.Not.Null);
        Assert.That(() => root.Children.Count, Is.Zero);
        Assert.That(() => root.Children.IsReadOnly, Is.False);
        Assert.That(() => root.Children[0], Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => root.Children.Contains(newChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(anotherNewChildNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode0), Is.False);
        Assert.That(() => root.Children.Contains(childNode1), Is.False);
        Assert.That(() => root.Children.Contains(childNode2), Is.False);
        Assert.That(() => root.Children.Contains(childNode3), Is.False);
        Assert.That(() => root.Children.IndexOf(newChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(anotherNewChildNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode0), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode1), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode2), Is.Negative);
        Assert.That(() => root.Children.IndexOf(childNode3), Is.Negative);
        Assert.That(() => root.Children.ToString(), Is.EqualTo(@"VirtualTreeNodeCollection<string>: Count = 0"));
    }
}