using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Tests.Properties;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public unsafe sealed class FactotumTests
    {
        #region Constants and Fields

        private static readonly MethodInfo ToPropertyStringMethodDefinition =
            new Func<object, ToPropertyStringOptions, string>(Factotum.ToPropertyString)
                .Method
                .GetGenericMethodDefinition();

        #endregion

        #region Tests: General

        [Test]
        public void TestDisposeAndNull()
        {
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            disposableMock.Setup(obj => obj.Dispose()).Verifiable();

            var disposable = disposableMock.Object;
            var disposableCopy = disposable;

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Once);

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Once);
        }

        [Test]
        public void TestExchangeValueType()
        {
            const int OriginalA = 1;
            const int OriginalB = 2;
            Assert.That(OriginalA, Is.Not.EqualTo(OriginalB));

            var a = OriginalA;
            var b = OriginalB;

            Factotum.Exchange(ref a, ref b);
            Assert.That(a, Is.EqualTo(OriginalB));
            Assert.That(b, Is.EqualTo(OriginalA));
        }

        [Test]
        public void TestExchangeReferenceType()
        {
            var originalA = new object();
            var originalB = new object();
            Assert.That(originalA, Is.Not.SameAs(originalB));

            var a = originalA;
            var b = originalB;

            Factotum.Exchange(ref a, ref b);
            Assert.That(a, Is.SameAs(originalB));
            Assert.That(b, Is.SameAs(originalA));
        }

        [Test]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void TestIdentityValueType(int value)
        {
            Assert.That(() => Factotum.Identity(value), Is.EqualTo(value));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("TestString")]
        public void TestIdentityReferenceType(string value)
        {
            Assert.That(() => Factotum.Identity(value), Is.SameAs(value));
        }

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

        [Test]
        public void TestSetDefaultValues()
        {
            const string TestStringValue = "TestStringValue";

            Assert.That(TestStringValue, Is.Not.EqualTo(SetDefaultValuesTestClass.DefaultStringValue));

            Assert.That(
                SetDefaultValuesTestClass.DefaultStringValue,
                Is.Not.EqualTo(SetDefaultValuesTestClass.NonDefaultStringValue));

            SetDefaultValuesTestClass.StaticStringValueWithDefault = TestStringValue;

            var testObject = new SetDefaultValuesTestClass
            {
                StringValueNoDefault = TestStringValue,
                StringValueWithDefault = TestStringValue
            };

            Assert.That(SetDefaultValuesTestClass.StaticStringValueWithDefault, Is.EqualTo(TestStringValue));
            Assert.That(testObject.StringValueNoDefault, Is.EqualTo(TestStringValue));
            Assert.That(testObject.StringValueWithDefault, Is.EqualTo(TestStringValue));
            Assert.That(
                testObject.ReadOnlyStringValueWithDefault,
                Is.EqualTo(SetDefaultValuesTestClass.NonDefaultStringValue));

            var resultTestObject = Factotum.SetDefaultValues(testObject);
            Assert.That(resultTestObject, Is.SameAs(testObject));

            Assert.That(SetDefaultValuesTestClass.StaticStringValueWithDefault, Is.EqualTo(TestStringValue));
            Assert.That(testObject.StringValueNoDefault, Is.EqualTo(TestStringValue));
            Assert.That(testObject.StringValueWithDefault, Is.EqualTo(SetDefaultValuesTestClass.DefaultStringValue));
        }

        [Test]
        public void TestAreEqualByContents()
        {
            const string ValueA = "A";
            const string ValueB = "B";
            const string ValueC = "C";
            const string ValueD = "D";

            Assert.That(new[] { ValueA, ValueB, ValueC, ValueD }, Is.Unique);

            var nodeA = new RecursiveNode { Value = ValueA };
            var nodeB = new RecursiveNode { Value = ValueB, Parent = nodeA };
            nodeA.Parent = nodeB;

            Assert.That(nodeA, Is.Not.AssignableTo<IEquatable<RecursiveNode>>());

            var nodeC1 = new RecursiveNode { Value = ValueC, Parent = nodeA };
            var nodeC2 = new RecursiveNode { Value = ValueC, Parent = nodeA };
            var nodeDParentA = new RecursiveNode { Value = ValueD, Parent = nodeA };
            var nodeDParentB = new RecursiveNode { Value = ValueD, Parent = nodeB };

            Assert.That(Factotum.AreEqualByContents(nodeC1, nodeC1), Is.True);
            Assert.That(Factotum.AreEqualByContents(nodeC1, null), Is.False);
            Assert.That(Factotum.AreEqualByContents(null, nodeC1), Is.False);

            Assert.That(Factotum.AreEqualByContents(nodeC1, nodeC2), Is.True);
            Assert.That(Factotum.AreEqualByContents(nodeC2, nodeC1), Is.True);

            Assert.That(Factotum.AreEqualByContents(nodeC1, nodeDParentA), Is.False);

            Assert.That(Factotum.AreEqualByContents(nodeDParentA, nodeDParentB), Is.False);
        }

        [Test]
        public void TestMax()
        {
            Assert.That(() => Factotum.Max(1, 2), Is.EqualTo(2));
            Assert.That(() => Factotum.Max(1, 1), Is.EqualTo(1));

            Assert.That(() => Factotum.Max("abc", "abcd"), Is.EqualTo("abcd"));

            Assert.That(
                () => Factotum.Max(TimeSpan.FromMilliseconds(1d), TimeSpan.FromMilliseconds(2d)),
                Is.EqualTo(TimeSpan.FromMilliseconds(2d)));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(() => Factotum.Min(1, 2), Is.EqualTo(1));
            Assert.That(() => Factotum.Min(1, 1), Is.EqualTo(1));

            Assert.That(() => Factotum.Min("abc", "abcd"), Is.EqualTo("abc"));

            Assert.That(
                () => Factotum.Min(TimeSpan.FromMilliseconds(1d), TimeSpan.FromMilliseconds(2d)),
                Is.EqualTo(TimeSpan.FromMilliseconds(1d)));
        }

        [Test]
        public void TestGenerateIdNegativeCases()
        {
            Assert.That(
                () => Factotum.GenerateId(int.MaxValue, 0),
                Throws.ArgumentException.With.Message.Contains("There is nothing to generate."));

            Assert.That(
                () => Factotum.GenerateId(Factotum.MinimumGeneratedIdPartSize - 1, IdGenerationModes.Random),
                Throws
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With
                    .Message
                    .Contains("For the specified mode(s), the size must be at least"));

            Assert.That(
                () => Factotum.GenerateId(Factotum.MinimumGeneratedIdPartSize - 1, IdGenerationModes.Unique),
                Throws
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With
                    .Message
                    .Contains("For the specified mode(s), the size must be at least"));

            Assert.That(
                () =>
                    Factotum.GenerateId(
                        Factotum.MinimumGeneratedIdPartSize * 2 - 1,
                        IdGenerationModes.UniqueAndRandom),
                Throws
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With
                    .Message
                    .Contains("For the specified mode(s), the size must be at least"));
        }

        [Test]
        [TestCase(16, IdGenerationModes.Unique)]
        [TestCase(16, IdGenerationModes.Random)]
        [TestCase(32, IdGenerationModes.UniqueAndRandom)]
        [TestCase(307, IdGenerationModes.Unique)]
        [TestCase(307, IdGenerationModes.Random)]
        [TestCase(307, IdGenerationModes.UniqueAndRandom)]
        public void TestGenerateId(int size, IdGenerationModes modes)
        {
            var id = Factotum.GenerateId(size, modes);
            Assert.That(id, Has.Length.EqualTo(size));
            Assert.That(id.Count(b => b == 0), Is.LessThan(Factotum.MinimumGeneratedIdPartSize));
        }

        #endregion

        #region Tests: Properties

        [Test]
        public void TestGetPropertyInfoForInstanceProperty()
        {
            var propertyInfo = Factotum.GetPropertyInfo((TestObject obj) => obj.InstanceProperty);

            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.DeclaringType, Is.EqualTo(typeof(TestObjectBase)));
            Assert.That(propertyInfo.Name, Is.EqualTo("InstanceProperty"));

            var propertyInfoCopy = Factotum.For<TestObject>.GetPropertyInfo(obj => obj.InstanceProperty);
            Assert.That(propertyInfoCopy, Is.SameAs(propertyInfo));
        }

        [Test]
        public void TestGetPropertyNameForInstanceProperty()
        {
            var name = Factotum.GetPropertyName((TestObject obj) => obj.InstanceProperty);

            Assert.That(name, Is.EqualTo("InstanceProperty"));

            var nameCopy = Factotum.For<TestObject>.GetPropertyName(obj => obj.InstanceProperty);
            Assert.That(nameCopy, Is.EqualTo(name));
        }

        [Test]
        public void TestGetQualifiedPropertyNameForInstanceProperty()
        {
            var name = Factotum.GetQualifiedPropertyName((TestObject obj) => obj.InstanceProperty);

            Assert.That(
                name,
                Is.EqualTo(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}.InstanceProperty",
                        typeof(FactotumTests).Name,
                        typeof(TestObject).Name)));

            var nameCopy = Factotum.For<TestObject>.GetQualifiedPropertyName(obj => obj.InstanceProperty);
            Assert.That(nameCopy, Is.EqualTo(name));
        }

        [Test]
        public void TestGetPropertyInfoForStaticProperty()
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType - By design for this specific test case
            var propertyInfo = Factotum.GetPropertyInfo(() => TestObject.StaticProperty);

            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.DeclaringType, Is.EqualTo(typeof(TestObjectBase)));
            Assert.That(propertyInfo.Name, Is.EqualTo("StaticProperty"));
        }

        [Test]
        public void TestGetPropertyNameForStaticProperty()
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType - By design for this specific test case
            var name = Factotum.GetPropertyName(() => TestObject.StaticProperty);

            Assert.That(name, Is.EqualTo("StaticProperty"));
        }

        [Test]
        public void TestGetQualifiedPropertyNameForStaticProperty()
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType - By design for this specific test case
            var name = Factotum.GetQualifiedPropertyName(() => TestObject.StaticProperty);

            Assert.That(
                name,
                Is.EqualTo(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}.StaticProperty",
                        typeof(FactotumTests).Name,
                        typeof(TestObjectBase).Name)));
        }

        #endregion

        #region TestObjectBase Class

        public abstract class TestObjectBase
        {
            #region Public Properties

            public static string StaticProperty
            {
                get;
                set;
            }

            public string InstanceProperty
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region TestObject Class

        public sealed class TestObject : TestObjectBase
        {
            // No members
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
                    "FactotumTests.RecursiveNode :: <null>")
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
                    var pointerString = string.Format(Factotum.PointerStringFormat, PointerAddress);

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

        #region SetDefaultValuesTestClass Class

        private sealed class SetDefaultValuesTestClass
        {
            #region Constants and Fields

            public const string DefaultStringValue = "DefaultStringValue";
            public const string NonDefaultStringValue = "NonDefaultStringValue";

            #endregion

            #region Public Properties

            [DefaultValue(DefaultStringValue)]
            public static string StaticStringValueWithDefault
            {
                get;
                set;
            }

            public string StringValueNoDefault
            {
                get;
                set;
            }

            [DefaultValue(DefaultStringValue)]
            public string StringValueWithDefault
            {
                get;
                internal set;
            }

            [DefaultValue(DefaultStringValue)]
            //// ReSharper disable once MemberCanBeMadeStatic.Local
            public string ReadOnlyStringValueWithDefault
            {
                get
                {
                    return NonDefaultStringValue;
                }
            }

            [DefaultValue(DefaultStringValue)]
            [UsedImplicitly]
            public string this[int index]
            {
                get
                {
                    if (index != 0)
                    {
                        throw new NotSupportedException();
                    }

                    return NonDefaultStringValue;
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            #endregion
        }

        #endregion
    }
}