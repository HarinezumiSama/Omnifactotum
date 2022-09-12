using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumDelegateExtensions))]
internal sealed class OmnifactotumDelegateExtensionsTests
{
    [Test]
    public void TestGetTypedInvocations()
    {
        var obj = new TestClass();

        Assert.That(() => obj.GetSomeActionInvocations(), Is.Empty);

        obj.SomeAction += Handler1;

        Assert.That(
            () => obj.GetSomeActionInvocations(),
            Is.EqualTo(new EventHandler<EventArgs>[] { Handler1 }));

        obj.SomeAction += Handler2;

        Assert.That(
            () => obj.GetSomeActionInvocations(),
            Is.EqualTo(new EventHandler<EventArgs>[] { Handler1, Handler2 }));

        obj.SomeAction += Handler3;
        obj.SomeAction += Handler1;
        obj.SomeAction += Handler1;
        obj.SomeAction += Handler2;

        Assert.That(
            () => obj.GetSomeActionInvocations(),
            Is.EqualTo(new EventHandler<EventArgs>[] { Handler1, Handler2, Handler3, Handler1, Handler1, Handler2 }));

        obj.SomeAction -= Handler1;

        Assert.That(
            () => obj.GetSomeActionInvocations(),
            Is.EqualTo(new EventHandler<EventArgs>[] { Handler1, Handler2, Handler3, Handler1, Handler2 }));

        static void Handler1(object? sender, EventArgs eventArgs) => throw new NotSupportedException();

        static void Handler2(object? sender, EventArgs eventArgs) => throw new NotSupportedException();

        static void Handler3(object? sender, EventArgs eventArgs) => throw new NotSupportedException();
    }

    private sealed class TestClass
    {
        public event EventHandler<EventArgs>? SomeAction;

        public EventHandler<EventArgs>[] GetSomeActionInvocations() => SomeAction.GetTypedInvocations();
    }
}