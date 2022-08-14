#nullable enable

using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    internal abstract class VirtualTreeNodeTestsBase
    {
        [Test]
        public void TestCreationWithValueOnly()
        {
            const int Value = 17;

            var node = CreateTestee(Value);
            Assert.That(node, Is.Not.Null);
            Assert.That(() => node.Parent, Is.Null);
            Assert.That(() => node.Value, Is.EqualTo(Value));
            Assert.That(() => node.Children, Is.Not.Null.And.Empty);
        }

        [Test]
        public void TestCreationWithValueAndChildren()
        {
            const int Value = 19;
            const int ChildValue0 = 23;
            const int ChildValue1 = 29;
            const int ChildValue2 = 31;

            Assert.That(new[] { Value, ChildValue0, ChildValue1, ChildValue2 }, Is.Unique);

            var node = CreateTestee(
                Value,
                new[] { CreateTestee(ChildValue0), CreateTestee(ChildValue1), CreateTestee(ChildValue2) });

            Assert.That(node, Is.Not.Null);
            Assert.That(() => node.Parent, Is.Null);
            Assert.That(() => node.Value, Is.EqualTo(Value));
            Assert.That(() => node.Children, Is.Not.Null);
            Assert.That(() => node.Children.Count, Is.EqualTo(3));

            Assert.That(() => node.Children[0], Is.Not.Null);
            Assert.That(() => node.Children[0].Value, Is.EqualTo(ChildValue0));
            Assert.That(() => node.Children[0].Parent, Is.SameAs(node));
            Assert.That(() => node.Children[0].Children, Is.Not.Null.And.Empty);

            Assert.That(() => node.Children[1], Is.Not.Null);
            Assert.That(() => node.Children[1].Value, Is.EqualTo(ChildValue1));
            Assert.That(() => node.Children[1].Parent, Is.SameAs(node));
            Assert.That(() => node.Children[1].Children, Is.Not.Null.And.Empty);

            Assert.That(() => node.Children[2], Is.Not.Null);
            Assert.That(() => node.Children[2].Value, Is.EqualTo(ChildValue2));
            Assert.That(() => node.Children[2].Parent, Is.SameAs(node));
            Assert.That(() => node.Children[2].Children, Is.Not.Null.And.Empty);
        }

        [Test]
        public void TestToString()
        {
            const int Value1 = 37;

            var node1 = CreateTestee(Value1);
            Assert.That(node1, Is.Not.Null);
            Assert.That(() => node1.ToString(), Is.EqualTo("VirtualTreeNode<int>: Children.Count = 0, Value = 37"));

            const int Value2 = 41;
            const int ChildValue0 = 7;
            const int ChildValue1 = 11;
            const int ChildValue2 = 13;

            var node2 = CreateTestee(Value2, new[] { CreateTestee(ChildValue0), CreateTestee(ChildValue1), CreateTestee(ChildValue2) });
            Assert.That(node2, Is.Not.Null);
            Assert.That(() => node2.ToString(), Is.EqualTo("VirtualTreeNode<int>: Children.Count = 3, Value = 41"));
        }

        protected abstract VirtualTreeNode<T> CreateTestee<T>(T value);

        protected abstract VirtualTreeNode<T> CreateTestee<T>(T value, IReadOnlyCollection<VirtualTreeNode<T>> children);
    }
}