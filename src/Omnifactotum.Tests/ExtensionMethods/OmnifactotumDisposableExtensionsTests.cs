using System;
using System.IO;
using Moq;
using NUnit.Framework;
using Omnifactotum.Tests.Auxiliary;

namespace Omnifactotum.Tests.ExtensionMethods
{
    //// ReSharper disable AssignNullToNotNullAttribute - Intentionally for tests
    [TestFixture]
    internal sealed class OmnifactotumDisposableExtensionsTests
    {
        [Test]
        public void TestDisposeSafelyOfReferenceTypeNullInstance()
        {
            ((Stream)null).DisposeSafely();
        }

        [Test]
        public void TestDisposeSafelyOfReferenceTypeRealInstance()
        {
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            disposableMock.Setup(obj => obj.Dispose()).Verifiable();

            var disposable = disposableMock.Object;

            disposable.DisposeSafely();
            Assert.That(disposable, Is.Not.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Exactly(1));

            disposable.DisposeSafely();
            Assert.That(disposable, Is.Not.Null);
            disposableMock.Verify(obj => obj.Dispose(), Times.Exactly(2));
        }

        [Test]
        public void TestDisposeSafelyOfNullableTypeNullValue()
        {
            ((DisposableStruct?)null).DisposeSafely();
        }

        [Test]
        public void TestDisposeSafelyOfNullableTypeRealValue()
        {
            var disposeCallCount = 0L;
            var disposableStruct = new DisposableStruct();
            disposableStruct.OnDispose += () => disposeCallCount++;

            var nullable = new DisposableStruct?(disposableStruct);

            nullable.DisposeSafely();
            Assert.That(disposeCallCount, Is.EqualTo(1));

            nullable.DisposeSafely();
            Assert.That(disposeCallCount, Is.EqualTo(2));
        }
    }
}