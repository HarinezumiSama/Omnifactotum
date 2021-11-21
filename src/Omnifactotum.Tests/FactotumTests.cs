using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Tests.Auxiliary;
using static Omnifactotum.FormattableStringFactotum;

#pragma warning disable CA1822

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(Factotum))]
    internal sealed class FactotumTests
    {
        [Test]
        public void TestDisposeAndNullOfReferenceType()
        {
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            disposableMock.Setup(obj => obj.Dispose()).Verifiable();

            var disposable = disposableMock.Object;
            var disposableCopy = disposable;

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            Assert.That(disposable, Is.Not.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Once);

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            Assert.That(disposable, Is.Not.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Once);
        }

        [Test]
        public void TestDisposeAndNullOfNullableType()
        {
            var disposeCallCount = 0L;
            var disposableStruct = new DisposableStruct();
            disposableStruct.OnDispose += () => disposeCallCount++;

            var disposable = new DisposableStruct?(disposableStruct);
            var disposableCopy = disposable;

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposeCallCount, Is.EqualTo(1));

            Factotum.DisposeAndNull(ref disposableCopy);
            Assert.That(disposableCopy, Is.Null);
            Assert.That(disposable, Is.Not.Null);
            Assert.That(disposeCallCount, Is.EqualTo(1));
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
            Assert.That(() => Factotum.For<int>.Identity(value), Is.EqualTo(value));
            Assert.That(() => Factotum.For<int>.IdentityMethod(value), Is.EqualTo(value));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("TestString")]
        public void TestIdentityReferenceType(string value)
        {
            Assert.That(() => Factotum.Identity(value), Is.SameAs(value));
            Assert.That(() => Factotum.For<string>.Identity(value), Is.SameAs(value));
            Assert.That(() => Factotum.For<string>.IdentityMethod(value), Is.SameAs(value));
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
        public void TestMax()
        {
            Assert.That(() => Factotum.Max(1, 2), Is.EqualTo(2));
            Assert.That(() => Factotum.Max(1, 1), Is.EqualTo(1));

            //// ReSharper disable StringLiteralTypo :: Test value
            Assert.That(() => Factotum.Max("abc", "abcd"), Is.EqualTo("abcd"));
            //// ReSharper restore StringLiteralTypo :: Test value

            Assert.That(
                () => Factotum.Max(TimeSpan.FromMilliseconds(1d), TimeSpan.FromMilliseconds(2d)),
                Is.EqualTo(TimeSpan.FromMilliseconds(2d)));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(() => Factotum.Min(1, 2), Is.EqualTo(1));
            Assert.That(() => Factotum.Min(1, 1), Is.EqualTo(1));

            //// ReSharper disable StringLiteralTypo :: Test value
            Assert.That(() => Factotum.Min("abc", "abcd"), Is.EqualTo("abc"));
            //// ReSharper restore StringLiteralTypo :: Test value

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

        [Test]
        public void TestCreateEmptyCompletedTask()
        {
#pragma warning disable CS0618
            using var task = Factotum.CreateEmptyCompletedTask();
#pragma warning restore CS0618

            Assert.That(task, Is.Not.Null);
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
            Assert.That(task.Exception, Is.Null);
        }

        [Test]
        public void TestCreateEmptyFaultedTask()
        {
            var exception = new InvalidOperationException();

#pragma warning disable CS0618
            using var task = Factotum.CreateEmptyFaultedTask(exception);
#pragma warning restore CS0618

            Assert.That(task, Is.Not.Null);
            Assert.That(task.Status, Is.EqualTo(TaskStatus.Faulted));
            Assert.That(task.Exception.EnsureNotNull().InnerExceptions.Single(), Is.SameAs(exception));
        }

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
                Is.EqualTo(AsInvariant($@"{nameof(FactotumTests)}.{nameof(TestObject)}.{nameof(TestObject.InstanceProperty)}")));

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
#pragma warning disable IDE0002
            //// ReSharper disable once AccessToStaticMemberViaDerivedType :: On purpose
            var name = Factotum.GetQualifiedPropertyName(() => TestObject.StaticProperty);
#pragma warning restore IDE0002

            Assert.That(name, Is.EqualTo($@"{nameof(FactotumTests)}.{nameof(TestObjectBase)}.{nameof(TestObjectBase.StaticProperty)}"));
        }

        private abstract class TestObjectBase
        {
            public static string StaticProperty { get; set; }

            public string InstanceProperty { get; set; }
        }

        [UsedImplicitly]
        private sealed class TestObject : TestObjectBase
        {
            // No members
        }

        private sealed class SetDefaultValuesTestClass
        {
            public const string DefaultStringValue = "DefaultStringValue";
            public const string NonDefaultStringValue = "NonDefaultStringValue";

            [DefaultValue(DefaultStringValue)]
            public static string StaticStringValueWithDefault { get; set; }

            public string StringValueNoDefault { get; set; }

            [DefaultValue(DefaultStringValue)]
            public string StringValueWithDefault { get; internal set; }

            [DefaultValue(DefaultStringValue)]
            //// ReSharper disable once MemberCanBeMadeStatic.Local
            public string ReadOnlyStringValueWithDefault => NonDefaultStringValue;

            [DefaultValue(DefaultStringValue)]
            [UsedImplicitly]
            public string this[int index]
            {
                get => index == 0 ? NonDefaultStringValue : throw new NotSupportedException();

                set => throw new NotSupportedException();
            }
        }
    }
}