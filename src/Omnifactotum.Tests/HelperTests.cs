﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Tests.Properties;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed unsafe class HelperTests
    {
        #region Constants and Fields

        private static readonly MethodInfo ToPropertyStringMethodDefinition =
            new Func<object, ToPropertyStringOptions, string>(Helper.ToPropertyString)
                .Method
                .GetGenericMethodDefinition();

        #endregion

        #region Tests

        [Test]
        [TestCaseSource(typeof(ToPropertyStringCases))]
        public void TestToPropertyString(
            Type objectType,
            object obj,
            ToPropertyStringOptions options,
            string expectedString)
        {
            var method = ToPropertyStringMethodDefinition.MakeGenericMethod(objectType);

            var actualString = (string)method.Invoke(null, new[] { obj, options });
            Assert.That(actualString, Is.EqualTo(expectedString));
        }

        #endregion

        #region RecursiveNode Class

        private sealed class RecursiveNode
        {
            #region Public Properties

            public string Value
            {
                [UsedImplicitly]
                get;

                set;
            }

            public RecursiveNode Parent
            {
                [UsedImplicitly]
                get;

                set;
            }

            #endregion
        }

        #endregion

        #region PointerContainer Class

        private sealed class PointerContainer
        {
            #region Public Properties

            public string Value
            {
                [UsedImplicitly]
                get;

                set;
            }

            public int* IntPointer
            {
                [UsedImplicitly]
                get;

                set;
            }

            public IntPtr IntPtr
            {
                [UsedImplicitly]
                get;

                set;
            }

            #endregion
        }

        #endregion

        #region ToPropertyStringCases Class

        private sealed class ToPropertyStringCases : TestCasesBase
        {
            private const int PointerAddress = 0x12EF3478;

            private static readonly PointerContainer PointerContainer = new PointerContainer
            {
                Value = "SomePointer",
                IntPointer = (int*)PointerAddress,
                IntPtr = new IntPtr(PointerAddress)
            };

            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(
                    typeof(string),
                    null,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "String :: <null>")
                    .SetName("Null string, all flags");

                yield return new TestCaseData(
                    typeof(RecursiveNode),
                    null,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "HelperTests.RecursiveNode :: <null>")
                    .SetName("Null RecursiveNode, all flags");

                yield return new TestCaseData(
                    typeof(int),
                    15789632,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "Int32 :: 15789632")
                    .SetName("Int32, all flags");

                {
                    const int IntValue = 35781632;

                    yield return
                        new TestCaseData(
                            typeof(int),
                            IntValue,
                            new ToPropertyStringOptions(),
                            IntValue.ToString(CultureInfo.InvariantCulture))
                            .SetName("Int32, default options");
                }

                {
                    const int IntValue = -45781632;

                    yield return
                        new TestCaseData(typeof(int), IntValue, null, IntValue.ToString(CultureInfo.InvariantCulture))
                            .SetName("Int32, null options");
                }

                {
                    var pointerString = string.Format(Helper.PointerStringFormat, PointerAddress);

                    yield return
                        new TestCaseData(
                            typeof(PointerContainer),
                            PointerContainer,
                            new ToPropertyStringOptions().SetAllFlags(true),
                            string.Format(Resources.ExpectedPointerContainerToPropertyStringTemplate, pointerString))
                            .SetName("PointerContainer, all flags");
                }

                yield return
                    new TestCaseData(
                        typeof(object),
                        VirtualTreeNode.Create(new DateTime(2011, 12, 31, 13, 59, 58, 321)),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedVirtualTreeNodeWithDateTimeToPropertyString)
                        .SetName("VirtualTreeNode with DateTime, all flags");

                yield return
                    new TestCaseData(
                        typeof(object),
                        VirtualTreeNode.Create(
                            new DateTimeOffset(2011, 12, 31, 13, 59, 58, 321, TimeSpan.FromHours(-2d))),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedVirtualTreeNodeWithDateTimeOffsetToPropertyString)
                        .SetName("VirtualTreeNode with DateTimeOffset, all flags");

                var keyTuple = Tuple.Create(GetType().ToString(), (Array)new[] { 1, 2, 5 });
                var valueTuple = Tuple.Create((object)keyTuple, ToString());
                var kvp = new KeyValuePair<Tuple<string, Array>, Tuple<object, string>>(keyTuple, valueTuple);

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedComplexObjectAllFlagsToPropertyString)
                        .SetName("Complex object (KeyValuePair), all flags");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions(),
                        Resources.ExpectedComplexObjectDefaultOptionsToPropertyString)
                        .SetName("Complex object (KeyValuePair), default options");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, MaxCollectionItemCount = 1 },
                        Resources.ExpectedComplexObjectMaxOneItemToPropertyString)
                        .SetName("Complex object (KeyValuePair), complex properties and max 1 item from collection");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, RenderMemberType = true },
                        Resources.ExpectedComplexObjectWithMemberTypeToPropertyString)
                        .SetName("Complex object (KeyValuePair), complex properties and member types");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, RenderActualType = true },
                        Resources.ExpectedComplexObjectWithActualTypeToPropertyString)
                        .SetName("Complex object (KeyValuePair), complex properties and actual types");

                var rootNode = new RecursiveNode { Value = "Root" };
                var childNode = new RecursiveNode { Value = "Child", Parent = rootNode };
                rootNode.Parent = childNode;
                var grandChildNode = new RecursiveNode { Value = "Grandchild", Parent = childNode };
                var nodes = new[] { rootNode, childNode, grandChildNode };

                yield return
                    new TestCaseData(
                        typeof(RecursiveNode[]),
                        nodes,
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedComplexObjectWithCyclesAllFlagsToPropertyString)
                        .SetName("Complex object (RecursiveNode[]) with cyclic dependency, all flags");

                yield return
                    new TestCaseData(
                        typeof(RecursiveNode[]),
                        nodes,
                        new ToPropertyStringOptions { RenderRootActualType = true, RenderComplexProperties = true },
                        Resources.ExpectedComplexObjectWithCyclesWithComplexPropertiesToPropertyString)
                        .SetName("Complex object (RecursiveNode[]) with cyclic dependency, complex properties");

                yield return
                    new TestCaseData(
                        typeof(RecursiveNode[]),
                        nodes,
                        new ToPropertyStringOptions { RenderComplexProperties = true, MaxRecursionLevel = 2 },
                        Resources.ExpectedMaxRecursionToPropertyString)
                        .SetName(
                            "Complex object (RecursiveNode[]) with cyclic dependency, all flags, with max recursion");

                yield return
                    new TestCaseData(
                        typeof(Delegate),
                        new Func<string>(typeof(object).ToString),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        "Func<String> :: System.Func`1[System.String]")
                        .SetName("Delegate");
            }
        }

        #endregion
    }
}