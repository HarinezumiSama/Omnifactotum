using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class DirectedGraphTests
    {
        [Test]
        [Category(TestCategory.Positive)]
        public void TestObjectConnection()
        {
            var nodeA = DirectedGraphNode.Create("A");
            var nodeB = DirectedGraphNode.Create("B");
            var nodeC = DirectedGraphNode.Create("C");
            var nodeD = DirectedGraphNode.Create("D");
            var nodeE = DirectedGraphNode.Create("E");

            nodeA.Heads.Add(nodeB);
            nodeA.Heads.Add(nodeC);
            nodeA.Heads.Add(nodeD);

            AssertStrictNodeRelation(nodeA, nodeB);
            AssertStrictNodeRelation(nodeA, nodeC);
            AssertStrictNodeRelation(nodeA, nodeD);

            Assert.That(nodeA.Graph, Is.Null);
            Assert.That(nodeB.Graph, Is.Null);
            Assert.That(nodeC.Graph, Is.Null);
            Assert.That(nodeD.Graph, Is.Null);
            Assert.That(nodeE.Graph, Is.Null);

            var graph = new DirectedGraph<string>(new[] { nodeA, nodeB });
            Assert.That(graph.Count, Is.EqualTo(4));
            Assert.That(graph, Is.EquivalentTo(new[] { nodeA, nodeB, nodeC, nodeD }));

            AssertBelongsToGraph(nodeA, graph, true);
            AssertBelongsToGraph(nodeB, graph, true);
            AssertBelongsToGraph(nodeC, graph, true);
            AssertBelongsToGraph(nodeD, graph, true);
            AssertBelongsToGraph(nodeE, graph, false);

            nodeA.Heads.Add(nodeE);
            AssertStrictNodeRelation(nodeA, nodeE);
            AssertBelongsToGraph(nodeE, graph, true);
            Assert.That(nodeE.Graph, Is.SameAs(graph));
            Assert.That(graph, Is.EquivalentTo(new[] { nodeA, nodeB, nodeC, nodeD, nodeE }));

            nodeB.Heads.Add(nodeD);
            AssertStrictNodeRelation(nodeB, nodeD);

            nodeC.Heads.Add(nodeD);
            nodeC.Heads.Add(nodeE);

            AssertStrictNodeRelation(nodeC, nodeD);
            AssertStrictNodeRelation(nodeC, nodeE);

            nodeD.Heads.Add(nodeE);
            AssertStrictNodeRelation(nodeD, nodeE);

            Assert.That(graph.Count, Is.EqualTo(5));

            AssertNodesNotRelated(nodeB, nodeC);
            AssertNodesNotRelated(nodeB, nodeE);

            //// Checking connections after a node is removed from the graph

            Assert.That(graph.Remove(nodeD), Is.True);

            AssertBelongsToGraph(nodeD, graph, false);
            Assert.That(nodeD.Graph, Is.Null);

            AssertNodesNotRelated(nodeD, nodeA);
            AssertNodesNotRelated(nodeD, nodeB);
            AssertNodesNotRelated(nodeD, nodeC);
            AssertNodesNotRelated(nodeD, nodeE);

            Assert.That(graph.Remove(nodeD), Is.False);

            //// Checking connections after a node is removed from other node's head/tail

            Assert.That(nodeA.Heads.Remove(nodeC), Is.True);

            AssertBelongsToGraph(nodeA, graph, true);
            AssertBelongsToGraph(nodeB, graph, true);
            AssertBelongsToGraph(nodeC, graph, true);
            AssertBelongsToGraph(nodeE, graph, true);

            AssertBelongsToGraph(nodeD, graph, false);
            Assert.That(nodeD.Graph, Is.Null);

            AssertNodesNotRelated(nodeA, nodeC);
            AssertNodesNotRelated(nodeD, nodeA);
            AssertNodesNotRelated(nodeD, nodeB);
            AssertNodesNotRelated(nodeD, nodeC);
            AssertNodesNotRelated(nodeD, nodeE);

            AssertStrictNodeRelation(nodeA, nodeB);
            AssertStrictNodeRelation(nodeA, nodeE);
            AssertStrictNodeRelation(nodeC, nodeE);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestSortTopologicallyBasic()
        {
            var nodeA = DirectedGraphNode.Create("A");
            var nodeB = DirectedGraphNode.Create("B");
            var nodeC = DirectedGraphNode.Create("C");
            var nodeD = DirectedGraphNode.Create("D");
            var nodeE = DirectedGraphNode.Create("E");

            nodeA.Heads.Add(nodeB);
            nodeA.Heads.Add(nodeC);
            nodeA.Heads.Add(nodeD);
            nodeA.Heads.Add(nodeE);
            nodeB.Heads.Add(nodeD);
            nodeC.Heads.Add(nodeD);
            nodeC.Heads.Add(nodeE);
            nodeD.Heads.Add(nodeE);

            var graph = new DirectedGraph<string>(new[] { nodeA });

            Action assertGraphState =
                () =>
                {
                    Assert.That(graph.Count, Is.EqualTo(5));

                    AssertStrictNodeRelation(nodeA, nodeB);
                    AssertStrictNodeRelation(nodeA, nodeC);
                    AssertStrictNodeRelation(nodeA, nodeD);
                    AssertStrictNodeRelation(nodeA, nodeE);
                    AssertStrictNodeRelation(nodeB, nodeD);
                    AssertStrictNodeRelation(nodeC, nodeD);
                    AssertStrictNodeRelation(nodeC, nodeE);
                    AssertStrictNodeRelation(nodeD, nodeE);

                    AssertNodesNotRelated(nodeB, nodeC);
                    AssertNodesNotRelated(nodeB, nodeE);
                };

            assertGraphState();

            var sortedNodes = graph.SortTopologically(
                (left, right) => StringComparer.Ordinal.Compare(left.Value, right.Value));
            Assert.That(sortedNodes, Is.Not.Null);
            CollectionAssert.AreEqual(sortedNodes, new[] { nodeA, nodeB, nodeC, nodeD, nodeE });
            assertGraphState();

            var sortedNodesDefault = graph.SortTopologically();
            Assert.That(sortedNodesDefault, Is.Not.Null);
            CollectionAssert.AreEqual(sortedNodesDefault, new[] { nodeA, nodeB, nodeC, nodeD, nodeE });
            assertGraphState();

            var sortedNodesReverseAlpha = graph.SortTopologically(
                (left, right) => -StringComparer.Ordinal.Compare(left.Value, right.Value));
            Assert.That(sortedNodesReverseAlpha, Is.Not.Null);
            CollectionAssert.AreEqual(sortedNodesReverseAlpha, new[] { nodeA, nodeC, nodeB, nodeD, nodeE });
            assertGraphState();
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestSortTopologicallyWithMultipleRoots()
        {
            var graph = new DirectedGraph<string>();

            var nodeA = graph.AddNode("A");
            var nodeB = graph.AddNode("B");
            var nodeC = graph.AddNode("C");
            var nodeD = graph.AddNode("D");
            var nodeE = graph.AddNode("E");

            nodeA.Heads.Add(nodeB);
            nodeC.Heads.Add(nodeD);
            nodeB.Heads.Add(nodeE);
            nodeD.Heads.Add(nodeE);

            Action assertGraphState =
                () =>
                {
                    Assert.That(graph.Count, Is.EqualTo(5));

                    AssertStrictNodeRelation(nodeA, nodeB);
                    AssertStrictNodeRelation(nodeC, nodeD);
                    AssertStrictNodeRelation(nodeB, nodeE);
                    AssertStrictNodeRelation(nodeD, nodeE);

                    AssertNodesNotRelated(nodeA, nodeC);
                    AssertNodesNotRelated(nodeA, nodeD);
                    AssertNodesNotRelated(nodeA, nodeE);

                    AssertNodesNotRelated(nodeB, nodeC);
                    AssertNodesNotRelated(nodeB, nodeD);

                    AssertNodesNotRelated(nodeC, nodeA);
                    AssertNodesNotRelated(nodeC, nodeB);
                    AssertNodesNotRelated(nodeC, nodeE);

                    AssertNodesNotRelated(nodeD, nodeA);
                    AssertNodesNotRelated(nodeD, nodeB);
                };

            assertGraphState();

            var sortedNodesDefault = graph.SortTopologically();
            Assert.That(sortedNodesDefault, Is.Not.Null);
            CollectionAssert.AreEqual(sortedNodesDefault, new[] { nodeA, nodeB, nodeC, nodeD, nodeE });
            assertGraphState();

            var sortedNodesReverseAlpha = graph.SortTopologically(
                (left, right) => -StringComparer.Ordinal.Compare(left.Value, right.Value));
            Assert.That(sortedNodesReverseAlpha, Is.Not.Null);
            CollectionAssert.AreEqual(sortedNodesReverseAlpha, new[] { nodeC, nodeD, nodeA, nodeB, nodeE });
            assertGraphState();
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestSortTopologicallyWithEmptyGraph()
        {
            var graph = new DirectedGraph<string>();
            Assert.That(graph, Is.Empty);
            Assert.That(graph.Count, Is.EqualTo(0));

            var sortedNodes = graph.SortTopologically();
            Assert.That(sortedNodes, Is.Empty);
            Assert.That(graph, Is.Empty);
            Assert.That(graph.Count, Is.EqualTo(0));
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestSortTopologicallyWithSelfCycle()
        {
            var graph = new DirectedGraph<string>();

            var nodeA = graph.AddNode("A");
            nodeA.Heads.Add(nodeA);

            AssertNodeRelation(nodeA, nodeA);

            Assert.That(() => graph.SortTopologically(), Throws.InvalidOperationException);
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestSortTopologicallyWithSimpleCycle()
        {
            var graph = new DirectedGraph<string>();

            var nodeA = graph.AddNode("A");
            var nodeB = graph.AddNode("B");

            nodeA.Heads.Add(nodeB);
            nodeA.Tails.Add(nodeB);

            AssertNodeRelation(nodeA, nodeB);
            AssertNodeRelation(nodeB, nodeA);

            Assert.That(() => graph.SortTopologically(), Throws.InvalidOperationException);
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestSortTopologicallyWithComplexCycle()
        {
            var graph = new DirectedGraph<string>();

            var nodeA = graph.AddNode("A");
            var nodeB = graph.AddNode("B");
            var nodeC = graph.AddNode("C");
            var nodeD = graph.AddNode("D");

            nodeA.Heads.Add(nodeB);
            nodeB.Heads.Add(nodeC);
            nodeC.Heads.Add(nodeD);
            nodeD.Heads.Add(nodeB);

            AssertStrictNodeRelation(nodeA, nodeB);
            AssertStrictNodeRelation(nodeB, nodeC);
            AssertStrictNodeRelation(nodeC, nodeD);
            AssertStrictNodeRelation(nodeD, nodeB);

            Assert.That(() => graph.SortTopologically(), Throws.InvalidOperationException);
        }

        private static void AssertNodeRelation<T>(DirectedGraphNode<T> tail, DirectedGraphNode<T> head)
        {
            Assert.That(head, Is.Not.Null);
            Assert.That(tail, Is.Not.Null);

            Assert.That(tail.Heads, Has.Member(head));
            Assert.That(head.Tails, Has.Member(tail));
        }

        private static void AssertStrictNodeRelation<T>(DirectedGraphNode<T> tail, DirectedGraphNode<T> head)
        {
            AssertNodeRelation(tail, head);

            Assert.That(tail.Tails, Has.No.Member(head));
            Assert.That(head.Heads, Has.No.Member(tail));
        }

        private static void AssertNodesNotRelated<T>(DirectedGraphNode<T> node, DirectedGraphNode<T> notRelatedNode)
        {
            Assert.That(node, Is.Not.Null);
            Assert.That(notRelatedNode, Is.Not.Null);

            Assert.That(node.Heads, Has.No.Member(notRelatedNode));
            Assert.That(node.Tails, Has.No.Member(notRelatedNode));

            Assert.That(notRelatedNode.Heads, Has.No.Member(node));
            Assert.That(notRelatedNode.Tails, Has.No.Member(node));
        }

        private static void AssertBelongsToGraph<T>(
            DirectedGraphNode<T> node,
            DirectedGraph<T> expectedGraph,
            bool belongs)
        {
            Assert.That(node, Is.Not.Null);
            Assert.That(expectedGraph, Is.Not.Null);

            var negateCondition = !belongs;

            Assert.That(node.Graph, AutoNegate(Is.SameAs(expectedGraph), negateCondition));
            Assert.That(expectedGraph, AutoNegate(Has.Member(node), negateCondition));
            Assert.That(expectedGraph.Contains(node), AutoNegate(Is.True, negateCondition));
        }

        private static Constraint AutoNegate(Constraint constraint, bool negateCondition)
        {
            return negateCondition ? !constraint : constraint;
        }
    }
}