using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Tests.Properties;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumGenericObjectExtensions))]
    internal sealed class OmnifactotumGenericObjectExtensionsTests
    {
        private const string NullString = null;
        private const object NullObject = null;

        private static readonly MethodInfo ToPropertyStringWithSpecificOptionsMethodDefinition =
            new Func<object, ToPropertyStringOptions, string>(OmnifactotumGenericObjectExtensions.ToPropertyString)
                .Method
                .GetGenericMethodDefinition();

        private static readonly MethodInfo ToPropertyStringWithDefaultOptionsMethodDefinition =
            new Func<object, string>(OmnifactotumGenericObjectExtensions.ToPropertyString)
                .Method
                .GetGenericMethodDefinition();

        [Test]
        public void TestEnsureNotNullForReferenceTypeSucceeds()
        {
            var emptyString = string.Empty;
            Assert.That(() => emptyString.EnsureNotNull(), Is.SameAs(emptyString));

            var someObject = new object();
            Assert.That(() => someObject.EnsureNotNull(), Is.SameAs(someObject));

            Assert.That(() => NullObject.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestEnsureNotNullForNullableSucceeds()
        {
            int? someValue = 42;
            Assert.That(() => someValue.EnsureNotNull(), Is.EqualTo(someValue.Value));

            Assert.That(() => ((int?)null).EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [SetCulture("ru-RU")]
        public void TestToUIStringSucceeds()
        {
            Assert.That(((int?)null).ToUIString(), Is.EqualTo("null"));
            Assert.That(((int?)1234567).ToUIString(), Is.EqualTo("1234567"));

            Assert.That(((DateTime?)null).ToUIString(), Is.EqualTo("null"));

            Assert.That(
                ((DateTime?)new DateTime(2016, 11, 19, 22, 14, 13)).ToUIString(),
                Is.EqualTo("11/19/2016 22:14:13"));
        }

        [Test]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void TestToUIStringWithFormatAndFormatProviderSucceeds()
        {
            const string NumberFormat = @"N0";

            const string RussianDateFormat = @"dddd', 'd' 'MMMM' 'yyyy' г. 'HH':'mm':'ss";
            const string JapaneseDateFormat = @"yyyy'年'MMMMd'日'dddd' 'HH':'mm':'ss";

            var russianCultureInfo = new CultureInfo(@"ru-RU");
            var japaneseCultureInfo = new CultureInfo(@"ja-JP");

            int? nullableIntegerNull = null;
            int? nullableInteger = 1234567;
            var nullableDateTime = (DateTime?)new DateTime(2016, 11, 19, 22, 14, 13);

            Assert.That(nullableIntegerNull.ToUIString(NumberFormat, russianCultureInfo), Is.EqualTo(@"null"));
            Assert.That(nullableIntegerNull.ToUIString(NumberFormat, japaneseCultureInfo), Is.EqualTo(@"null"));

            Assert.That(nullableInteger.ToUIString(NumberFormat, russianCultureInfo), Is.EqualTo(@"1 234 567"));
            Assert.That(nullableInteger.ToUIString(NumberFormat, japaneseCultureInfo), Is.EqualTo(@"1,234,567"));

            Assert.That(((DateTime?)null).ToUIString(RussianDateFormat, russianCultureInfo), Is.EqualTo(@"null"));
            Assert.That(((DateTime?)null).ToUIString(JapaneseDateFormat, japaneseCultureInfo), Is.EqualTo(@"null"));

            //// ReSharper disable StringLiteralTypo :: False detection (not English)
            Assert.That(
                nullableDateTime.ToUIString(RussianDateFormat, russianCultureInfo),
                Is.EqualTo(@"суббота, 19 ноября 2016 г. 22:14:13"));
            //// ReSharper restore StringLiteralTypo

            Assert.That(
                nullableDateTime.ToUIString(JapaneseDateFormat, japaneseCultureInfo),
                Is.EqualTo(@"2016年11月19日土曜日 22:14:13"));
        }

        [Test]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void TestToUIStringWithFormatProviderSucceeds()
        {
            var russianCultureInfo = new CultureInfo("ru-RU");
            var japaneseCultureInfo = new CultureInfo("ja-JP");

            int? nullableIntegerNull = null;
            int? nullableInteger = 1234567;
            var nullableDateTime = (DateTime?)new DateTime(2016, 11, 19, 22, 14, 23);

            Assert.That(nullableIntegerNull.ToUIString(russianCultureInfo), Is.EqualTo("null"));
            Assert.That(nullableIntegerNull.ToUIString(japaneseCultureInfo), Is.EqualTo("null"));

            Assert.That(nullableInteger.ToUIString(russianCultureInfo), Is.EqualTo("1234567"));
            Assert.That(nullableInteger.ToUIString(japaneseCultureInfo), Is.EqualTo("1234567"));

            Assert.That(((DateTime?)null).ToUIString(russianCultureInfo), Is.EqualTo("null"));
            Assert.That(((DateTime?)null).ToUIString(japaneseCultureInfo), Is.EqualTo("null"));

            Assert.That(
                nullableDateTime.ToUIString(russianCultureInfo),
                Is.EqualTo("19.11.2016 22:14:23"));

            Assert.That(
                nullableDateTime.ToUIString(japaneseCultureInfo),
                Is.EqualTo("2016/11/19 22:14:23"));
        }

        [Test]
        public void TestAsArraySucceeds()
        {
            const int IntValue = 17;
            const string StringValue = "eaacda5096aa41048c86cdbccc27ed03";
            var obj = new object();

            Assert.That(() => IntValue.AsArray(), Is.TypeOf<int[]>().And.EqualTo(new[] { IntValue }));
            Assert.That(() => StringValue.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { StringValue }));
            Assert.That(() => obj.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { obj }));
            Assert.That(() => NullString.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { NullString }));
            Assert.That(() => NullObject.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        public void TestAsListSucceeds()
        {
            const int IntValue = 13;
            const string StringValue = "3037df31af4d426b8edc4469bdf0744c";
            var obj = new object();

            Assert.That(() => IntValue.AsList(), Is.TypeOf<List<int>>().And.EqualTo(new[] { IntValue }));
            Assert.That(() => StringValue.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { StringValue }));
            Assert.That(() => obj.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { obj }));
            Assert.That(() => NullString.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { NullString }));
            Assert.That(() => NullObject.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        public void TestAsCollectionSucceeds()
        {
            const int IntValue = 29;
            const string StringValue = "f00136299c4249e5b5ed8d3eb18bbfb4";
            var obj = new object();

            Assert.That(
                () => IntValue.AsCollection(),
                Is.InstanceOf<IEnumerable<int>>().And.EqualTo(new[] { IntValue }));

            Assert.That(
                () => StringValue.AsCollection(),
                Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { StringValue }));

            Assert.That(() => obj.AsCollection(), Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { obj }));

            Assert.That(
                () => NullString.AsCollection(),
                Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { NullString }));

            Assert.That(
                () => NullObject.AsCollection(),
                Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestAvoidNullWhenDefaultValueProviderIsNullThenThrows()
            => Assert.That(() => new object().AvoidNull(null), Throws.ArgumentNullException);

        [Test]
        public void TestAvoidNullWhenNullValueIsPassedAndDefaultValueProviderReturnsNullThenThrows()
            => Assert.That(() => ((TestClass)null).AvoidNull(() => null), Throws.InvalidOperationException);

        [Test]
        public void TestAvoidNullWhenNonNullValueIsPassedThenSucceedsAndReturnsPassedValue()
        {
            var input = new TestClass();
            Assert.That(() => input.AvoidNull(() => null), Is.SameAs(input));
            Assert.That(() => input.AvoidNull(() => new TestClass()), Is.SameAs(input));
        }

        [Test]
        public void TestAvoidNullWhenNullValueIsPassedThenSucceedsAndReturnsValueProvidedByDefaultValueProvider()
        {
            var output = new TestClass();
            Assert.That(() => ((TestClass)null).AvoidNull(() => output), Is.SameAs(output));
        }

        [Test]
        public void TestGetHashCodeSafelyWithDefaultNullValueHashCodeSucceeds()
        {
            const int IntValue = 17;
            Assert.That(() => IntValue.GetHashCodeSafely(), Is.EqualTo(IntValue.GetHashCode()));

            const string StringValue = "a9fd0a6ce1824e9596b2705611754182";
            Assert.That(() => StringValue.GetHashCodeSafely(), Is.EqualTo(StringValue.GetHashCode()));

            Assert.That(() => ((int?)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((string)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((object)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((TestClass)null).GetHashCodeSafely(), Is.EqualTo(0));
        }

        [Test]
        public void TestGetHashCodeSafelyWithSpecifiedNullValueHashCodeSucceeds()
        {
            const int NullValueHashCode = 1021;

            const int IntValue = 19;
            Assert.That(() => IntValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(IntValue.GetHashCode()));

            const string StringValue = "488066fdd8764ba99dedfc6457751c6e";
            Assert.That(() => StringValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(StringValue.GetHashCode()));

            Assert.That(() => ((int?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((string)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((object)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((TestClass)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
        }

        [Test]
        public void TestIsEqualByContentsToSucceeds()
        {
            //// TODO [HarinezumiSama] Consider MarshalByRefObject when it's a proxy

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

            AssertResultForIsEqualByContentsTo(nodeC1, nodeC1, Is.True);
            AssertResultForIsEqualByContentsTo(nodeC1, null, Is.False);
            AssertResultForIsEqualByContentsTo(nodeC1, nodeC2, Is.True);
            AssertResultForIsEqualByContentsTo(nodeC1, nodeDParentA, Is.False);
            AssertResultForIsEqualByContentsTo(nodeDParentA, nodeDParentB, Is.False);

            AssertResultForIsEqualByContentsTo<object>("13", "13", Is.True);
            AssertResultForIsEqualByContentsTo<object>(17, 17, Is.True);
            AssertResultForIsEqualByContentsTo<object>("19", 19, Is.False);

            const string ContainerValue = "92b6e84bba8e429b9ec8593d2594354e";
            var simpleContainer = new SimpleContainer { Value = ContainerValue };
            var descendantContainer1 = new DescendantContainer1 { Value = ContainerValue };
            var descendantContainer2 = new DescendantContainer2 { Value = ContainerValue };

            AssertResultForIsEqualByContentsTo(simpleContainer, descendantContainer1, Is.False);
            AssertResultForIsEqualByContentsTo<SimpleContainer>(descendantContainer1, descendantContainer2, Is.False);

            var classWithNoFields1 = new ClassWithNoFields();
            var classWithNoFields2 = new ClassWithNoFields();
            AssertResultForIsEqualByContentsTo(classWithNoFields1, classWithNoFields2, Is.True);

            var structureWithNoFields1 = new StructureWithNoFields();
            var structureWithNoFields2 = new StructureWithNoFields();
            AssertResultForIsEqualByContentsTo(structureWithNoFields1, structureWithNoFields2, Is.True);

            AssertResultForIsEqualByContentsTo<object>(classWithNoFields1, structureWithNoFields1, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(ToPropertyStringCases))]
        public void TestToPropertyStringSucceeds(
            Type objectType,
            object obj,
            ToPropertyStringOptions options,
            string expectedString)
        {
            var methodWithSpecificOptions =
                ToPropertyStringWithSpecificOptionsMethodDefinition.MakeGenericMethod(objectType);

            var actualResultWithSpecificOptions =
                (string)methodWithSpecificOptions.Invoke(null, new[] { obj, options });

            Assert.That(actualResultWithSpecificOptions, Is.EqualTo(expectedString));

            if (options is null)
            {
                var methodWithDefaultOptions =
                    ToPropertyStringWithDefaultOptionsMethodDefinition.MakeGenericMethod(objectType);

                var actualResultWithDefaultOptions = (string)methodWithDefaultOptions.Invoke(null, new[] { obj });
                Assert.That(actualResultWithDefaultOptions, Is.EqualTo(expectedString));
            }
        }

        [Test]
        [SetCulture("ru-RU")]
        public void TestToStringSafelyWithDefaultNullValueStringSucceeds()
        {
            Assert.That(() => ((TestClass)null).ToStringSafely(), Is.EqualTo(string.Empty));
            Assert.That(() => new TestClass().ToStringSafely(), Is.EqualTo(typeof(TestClass).FullName));

            Assert.That(() => ((long?)null).ToStringSafely(), Is.EqualTo(string.Empty));
            Assert.That(() => ((long?)123456789).ToStringSafely(), Is.EqualTo("123456789"));

            Assert.That(() => ((DateTime?)null).ToStringSafely(), Is.EqualTo(string.Empty));
            Assert.That(
                () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafely(),
                Is.EqualTo("17.11.2017 21:10:44"));

            Assert.That(() => true.ToStringSafely(), Is.EqualTo(bool.TrueString));
            Assert.That(() => FileMode.OpenOrCreate.ToStringSafely(), Is.EqualTo(nameof(FileMode.OpenOrCreate)));
        }

        [Test]
        [SetCulture("ru-RU")]
        public void TestToStringSafelyWithSpecifiedNullValueStringSucceeds()
        {
            const string NullValueString = "default";

            Assert.That(() => ((TestClass)null).ToStringSafely(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(() => new TestClass().ToStringSafely(NullValueString), Is.EqualTo(typeof(TestClass).FullName));

            Assert.That(() => ((long?)null).ToStringSafely(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(() => ((long?)42).ToStringSafely(NullValueString), Is.EqualTo("42"));

            Assert.That(() => ((DateTime?)null).ToStringSafely(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(
                () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafely(NullValueString),
                Is.EqualTo("17.11.2017 21:10:44"));

            Assert.That(() => true.ToStringSafely(NullValueString), Is.EqualTo(bool.TrueString));
            Assert.That(
                () => FileMode.OpenOrCreate.ToStringSafely(NullValueString),
                Is.EqualTo(nameof(FileMode.OpenOrCreate)));
        }

        [Test]
        [SetCulture("ru-RU")]
        public void TestToStringSafelyInvariantWithDefaultNullValueStringSucceeds()
        {
            Assert.That(() => ((TestClass)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
            Assert.That(() => new TestClass().ToStringSafelyInvariant(), Is.EqualTo(typeof(TestClass).FullName));

            Assert.That(() => ((long?)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
            Assert.That(() => ((long?)123456789).ToStringSafelyInvariant(), Is.EqualTo("123456789"));

            Assert.That(() => ((DateTime?)null).ToStringSafelyInvariant(), Is.EqualTo(string.Empty));
            Assert.That(
                () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafelyInvariant(),
                Is.EqualTo("11/17/2017 21:10:44"));

            Assert.That(() => true.ToStringSafelyInvariant(), Is.EqualTo(bool.TrueString));
            Assert.That(
                () => FileMode.OpenOrCreate.ToStringSafelyInvariant(),
                Is.EqualTo(nameof(FileMode.OpenOrCreate)));
        }

        [Test]
        [SetCulture("ru-RU")]
        public void TestToStringSafelyInvariantWithSpecifiedNullValueStringSucceeds()
        {
            const string NullValueString = "default";

            Assert.That(() => ((TestClass)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(
                () => new TestClass().ToStringSafelyInvariant(NullValueString),
                Is.EqualTo(typeof(TestClass).FullName));

            Assert.That(() => ((long?)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(() => ((long?)42).ToStringSafelyInvariant(NullValueString), Is.EqualTo("42"));

            Assert.That(() => ((DateTime?)null).ToStringSafelyInvariant(NullValueString), Is.EqualTo(NullValueString));
            Assert.That(
                () => ((DateTime?)new DateTime(2017, 11, 17, 21, 10, 44)).ToStringSafelyInvariant(NullValueString),
                Is.EqualTo("11/17/2017 21:10:44"));

            Assert.That(() => true.ToStringSafelyInvariant(NullValueString), Is.EqualTo(bool.TrueString));
            Assert.That(
                () => FileMode.OpenOrCreate.ToStringSafelyInvariant(NullValueString),
                Is.EqualTo(nameof(FileMode.OpenOrCreate)));
        }

        [Test]
        public void TestGetTypeSafelySucceeds()
        {
            Assert.That(() => ((object)null).GetTypeSafely(), Is.EqualTo(typeof(object)));
            Assert.That(() => new object().GetTypeSafely(), Is.EqualTo(typeof(object)));

            Assert.That(() => ((int?)null).GetTypeSafely(), Is.EqualTo(typeof(int?)));
            Assert.That(() => 17.GetTypeSafely(), Is.EqualTo(typeof(int)));
            Assert.That(() => ((int?)17).GetTypeSafely(), Is.EqualTo(typeof(int)));

            Assert.That(() => ((TestClass)null).GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
            Assert.That(() => new TestClass().GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
            Assert.That(() => ((object)new TestClass()).GetTypeSafely(), Is.EqualTo(typeof(TestClass)));
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestMorphForReferenceTypeWithInvalidArgumentsThrows()
        {
            var validInstance = new TestClass();

            Assert.That(() => validInstance.Morph<TestClass, string>(null), Throws.ArgumentNullException);
            Assert.That(() => validInstance.Morph(null, "anyValue"), Throws.ArgumentNullException);
        }

        [Test]
        public void TestMorphForReferenceTypeWithValidArgumentsSucceeds()
        {
            const RecursiveNode NullInput = null;
            const string Value = "value";
            const string DefaultOutput = "default";

            Assert.That(DefaultOutput, Is.Not.EqualTo(Value) & Is.Not.EqualTo(default(string)));

            Assert.That(
                () => NullInput.Morph(node => node.Value, DefaultOutput),
                Is.EqualTo(DefaultOutput));

            Assert.That(
                () => NullInput.Morph(node => node.Value),
                Is.EqualTo(default(string)));

            var input = new RecursiveNode { Value = Value };
            Assert.That(() => input.Morph(node => node.Value, DefaultOutput), Is.EqualTo(Value));
            Assert.That(() => input.Morph(node => node.Value), Is.EqualTo(Value));
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestMorphForNullableValueTypeWithInvalidArgumentsThrows()
        {
            var validInstance = new ConsoleKeyInfo?(new ConsoleKeyInfo());

            Assert.That(() => validInstance.Morph<ConsoleKeyInfo, string>(null), Throws.ArgumentNullException);

            Assert.That(
                () => validInstance.Morph(null, @"anyValue"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void TestMorphForNullableValueTypeWithValidArgumentsSucceeds()
        {
            const ConsoleKey Value = ConsoleKey.A;
            const ConsoleKey DefaultOutput = ConsoleKey.Escape;

            Assert.That(DefaultOutput, Is.Not.EqualTo(Value) & Is.Not.EqualTo(default(ConsoleKey)));

            Assert.That(
                () => ((ConsoleKeyInfo?)null).Morph(node => node.Key, DefaultOutput),
                Is.EqualTo(DefaultOutput));

            Assert.That(
                () => ((ConsoleKeyInfo?)null).Morph(node => node.Key),
                Is.EqualTo(default(ConsoleKey)));

            var input = new ConsoleKeyInfo?(new ConsoleKeyInfo('a', Value, false, false, false));
            Assert.That(() => input.Morph(info => info.Key, DefaultOutput), Is.EqualTo(Value));
            Assert.That(() => input.Morph(info => info.Key), Is.EqualTo(Value));
        }

        private static void AssertResultForIsEqualByContentsTo<T>(T valueA, T valueB, IResolveConstraint constraint)
        {
            Assert.That(valueA.IsEqualByContentsTo(valueB), constraint);
            Assert.That(valueB.IsEqualByContentsTo(valueA), constraint);
        }

        private sealed class ToPropertyStringCases : TestCasesBase
        {
            private const int PointerAddress0 = 0x10EF3478;
            private const int PointerAddress1 = 0x20EF3478;
            private const int PointerAddress2 = 0x30EF3478;

            private static readonly unsafe PointerContainer PointerContainer = new PointerContainer
            {
                Value = "SomePointer",
                IntPointer = (int*)PointerAddress0,
                IntPtr = new IntPtr(PointerAddress1),
                UIntPtr = new UIntPtr(PointerAddress2)
            };

            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(
                    typeof(string),
                    null,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "string :: <null>")
                    .SetDescription("Null string, all flags");

                yield return new TestCaseData(
                    typeof(RecursiveNode),
                    null,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "OmnifactotumGenericObjectExtensionsTests.RecursiveNode :: <null>")
                    .SetDescription("Null RecursiveNode, all flags");

                yield return new TestCaseData(
                    typeof(int),
                    15789632,
                    new ToPropertyStringOptions().SetAllFlags(true),
                    "int :: 15789632")
                    .SetDescription("Int32, all flags");

                {
                    const int IntValue = 35781632;

                    yield return
                        new TestCaseData(
                            typeof(int),
                            IntValue,
                            new ToPropertyStringOptions(),
                            IntValue.ToString(CultureInfo.InvariantCulture))
                            .SetDescription("Int32, default options");
                }

                {
                    const int IntValue = -45781632;

                    yield return
                        new TestCaseData(typeof(int), IntValue, null, IntValue.ToString(CultureInfo.InvariantCulture))
                            .SetDescription("Int32, null options");
                }

                {
                    var pointerString0 = string.Format(
                        OmnifactotumGenericObjectExtensions.PointerStringFormat,
                        PointerAddress0);

                    var pointerString1 = string.Format(
                        OmnifactotumGenericObjectExtensions.PointerStringFormat,
                        PointerAddress1);

                    var pointerString2 = string.Format(
                        OmnifactotumGenericObjectExtensions.PointerStringFormat,
                        PointerAddress2);

                    var expectedPointerContainerToPropertyString = string.Format(
                        Resources.ExpectedPointerContainerToPropertyStringTemplate,
                        pointerString0,
                        pointerString1,
                        pointerString2);

                    yield return
                        new TestCaseData(
                            typeof(PointerContainer),
                            PointerContainer,
                            new ToPropertyStringOptions().SetAllFlags(true),
                            expectedPointerContainerToPropertyString)
                            .SetDescription("PointerContainer, all flags");
                }

                yield return
                    new TestCaseData(
                        typeof(object),
                        VirtualTreeNode.Create(new DateTime(2011, 12, 31, 13, 59, 58, 321)),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedVirtualTreeNodeWithDateTimeToPropertyString)
                        .SetDescription("VirtualTreeNode with DateTime, all flags");

                yield return
                    new TestCaseData(
                        typeof(object),
                        VirtualTreeNode.Create(
                            new DateTimeOffset(2011, 12, 31, 13, 59, 58, 321, TimeSpan.FromHours(-2d))),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedVirtualTreeNodeWithDateTimeOffsetToPropertyString)
                        .SetDescription("VirtualTreeNode with DateTimeOffset, all flags");

                var keyTuple = Tuple.Create(GetType().ToString(), (Array)new[] { 1, 2, 5 });
                var valueTuple = Tuple.Create((object)keyTuple, ToString());
                var kvp = new KeyValuePair<Tuple<string, Array>, Tuple<object, string>>(keyTuple, valueTuple);

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedComplexObjectAllFlagsToPropertyString)
                        .SetDescription("Complex object (KeyValuePair), all flags");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions(),
                        Resources.ExpectedComplexObjectDefaultOptionsToPropertyString)
                        .SetDescription("Complex object (KeyValuePair), default options");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, MaxCollectionItemCount = 1 },
                        Resources.ExpectedComplexObjectMaxOneItemToPropertyString)
                        .SetDescription("Complex object (KeyValuePair), complex properties and max 1 item from collection");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, RenderMemberType = true },
                        Resources.ExpectedComplexObjectWithMemberTypeToPropertyString)
                        .SetDescription("Complex object (KeyValuePair), complex properties and member types");

                yield return
                    new TestCaseData(
                        typeof(KeyValuePair<Tuple<string, Array>, Tuple<object, string>>),
                        kvp,
                        new ToPropertyStringOptions { RenderComplexProperties = true, RenderActualType = true },
                        Resources.ExpectedComplexObjectWithActualTypeToPropertyString)
                        .SetDescription("Complex object (KeyValuePair), complex properties and actual types");

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
                        .SetDescription("Complex object (RecursiveNode[]) with cyclic dependency, all flags");

                yield return
                    new TestCaseData(
                        typeof(RecursiveNode[]),
                        nodes,
                        new ToPropertyStringOptions { RenderRootActualType = true, RenderComplexProperties = true },
                        Resources.ExpectedComplexObjectWithCyclesWithComplexPropertiesToPropertyString)
                        .SetDescription("Complex object (RecursiveNode[]) with cyclic dependency, complex properties");

                yield return
                    new TestCaseData(
                        typeof(RecursiveNode[]),
                        nodes,
                        new ToPropertyStringOptions { RenderComplexProperties = true, MaxRecursionLevel = 2 },
                        Resources.ExpectedMaxRecursionToPropertyString)
                        .SetDescription(
                            "Complex object (RecursiveNode[]) with cyclic dependency, all flags, with max recursion");

                yield return
                    new TestCaseData(
                        typeof(Delegate),
                        new Func<string>(typeof(object).ToString),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        "Func<string> :: System.Func`1[System.String]")
                        .SetDescription("Delegate");

                yield return
                    new TestCaseData(
                        typeof(ClassWithPropertyGetterThrowingException),
                        new ClassWithPropertyGetterThrowingException(),
                        new ToPropertyStringOptions().SetAllFlags(true),
                        Resources.ExpectedClassWithPropertyGetterThrowingExceptionToPropertyString)
                        .SetDescription(nameof(ClassWithPropertyGetterThrowingException));

                {
                    var input = new ClassWithFlagsEnumAndTypeAndAssemblyProperties();

                    var expectedClassWithFlagsEnumAndTypeAndAssemblyPropertiesToPropertyString = string.Format(
                        Resources.ExpectedClassWithFlagsEnumAndTypeAndAssemblyPropertiesToPropertyStringTemplate,
                        input.SomeAssembly.Location,
                        input.SomeAttributes.ToString(),
                        input.SomeType.AssemblyQualifiedName);

                    yield return
                        new TestCaseData(
                            typeof(ClassWithFlagsEnumAndTypeAndAssemblyProperties),
                            input,
                            new ToPropertyStringOptions().SetAllFlags(true),
                            expectedClassWithFlagsEnumAndTypeAndAssemblyPropertiesToPropertyString)
                            .SetDescription(nameof(ClassWithFlagsEnumAndTypeAndAssemblyProperties));
                }
            }
        }

        private sealed class TestClass
        {
            // No own members
        }

        private class SimpleContainer
        {
            public string Value
            {
                [UsedImplicitly]
                get;

                set;
            }
        }

        private sealed class DescendantContainer1 : SimpleContainer
        {
            // No own members
        }

        private sealed class DescendantContainer2 : SimpleContainer
        {
            // No own members
        }

        private sealed class ClassWithNoFields
        {
            // No own members
        }

        private sealed class StructureWithNoFields
        {
            // No own members
        }

        private sealed class RecursiveNode
        {
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
        }

        private sealed class PointerContainer
        {
            public string Value
            {
                [UsedImplicitly]
                get;

                set;
            }

            public unsafe int* IntPointer
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

            public UIntPtr UIntPtr
            {
                [UsedImplicitly]
                get;

                set;
            }
        }

        private sealed class ClassWithPropertyGetterThrowingException
        {
            [UsedImplicitly]
            public int Property1Good => 42;

            [UsedImplicitly]
            public DateTime Property2Bad => throw new NotImplementedException("Never gonna be implemented.");

            [UsedImplicitly]
            public IEnumerable<string> Property3Bad => new ThrowingEnumerable();

            [UsedImplicitly]
            public string Property4Good => "Some value";

            private sealed class ThrowingEnumerable : IEnumerable<string>
            {
                public IEnumerator<string> GetEnumerator() => new ThrowingEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            private sealed class ThrowingEnumerator : IEnumerator<string>
            {
                private int _index = -1;

                public void Dispose()
                {
                    // Nothing to do
                }

                public bool MoveNext()
                {
                    if (_index < -1 || _index >= 2)
                    {
                        return false;
                    }

                    _index++;
                    return true;
                }

                public void Reset() => throw new NotSupportedException();

                public string Current
                {
                    get
                    {
                        switch (_index)
                        {
                            case 0:
                                return "Item0";

                            case 1:
                                throw new NotImplementedException("No item at index 1.");

                            case 2:
                                return "Item2";

                            default:
                                throw new InvalidOperationException("Cannot access items outside range.");
                        }
                    }
                }

                object IEnumerator.Current => Current;
            }
        }

        private sealed class ClassWithFlagsEnumAndTypeAndAssemblyProperties
        {
            [UsedImplicitly]
            public FileAttributes SomeAttributes => FileAttributes.Archive | FileAttributes.Offline;

            [UsedImplicitly]
            public Type SomeType => typeof(TestClass);

            [UsedImplicitly]
            public Assembly SomeAssembly => typeof(ClassWithFlagsEnumAndTypeAndAssemblyProperties).Assembly;
        }
    }
}