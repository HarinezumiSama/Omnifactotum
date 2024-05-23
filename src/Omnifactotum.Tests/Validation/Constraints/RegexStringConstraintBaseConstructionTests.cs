using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(RegexStringConstraintBase))]
#pragma warning restore CS0618 // Type or member is obsolete
internal sealed class RegexStringConstraintBaseConstructionTests
{
    [Test]
    public void TestConstructionWhenValidArgumentsThenSucceeds()
    {
        Assert.That(() => new CustomArgumentsConstraint(string.Empty), Throws.Nothing);

        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.None), Throws.Nothing);
        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.ECMAScript), Throws.Nothing);

        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.None, TimeSpan.FromTicks(1)), Throws.Nothing);
        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(1)), Throws.Nothing);

        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.ECMAScript, TimeSpan.FromTicks(1)), Throws.Nothing);
        Assert.That(() => new CustomArgumentsConstraint(string.Empty, RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(1)), Throws.Nothing);
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    public void TestConstructionWhenInvalidArgumentThenThrows()
    {
        Assert.That(() => new CustomArgumentsConstraint(null!), Throws.ArgumentNullException);
        Assert.That(() => new CustomArgumentsConstraint(null!, RegexOptions.Singleline), Throws.ArgumentNullException);
        Assert.That(() => new CustomArgumentsConstraint(null!, RegexOptions.Singleline, null), Throws.ArgumentNullException);

        Assert.That(() => new CustomArgumentsConstraint("Value", TimeSpan.Zero), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => new CustomArgumentsConstraint("Value", Timeout.InfiniteTimeSpan), Throws.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(() => new CustomArgumentsConstraint("Value", RegexOptions.Singleline, TimeSpan.Zero), Throws.TypeOf<ArgumentOutOfRangeException>());

        Assert.That(
            () => new CustomArgumentsConstraint("Value", RegexOptions.Singleline, Timeout.InfiniteTimeSpan),
            Throws.TypeOf<ArgumentOutOfRangeException>());
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private sealed class CustomArgumentsConstraint : RegexStringConstraintBase
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public CustomArgumentsConstraint(string pattern, RegexOptions options, TimeSpan? timeout)
            : base(pattern, options, timeout)
        {
            // Nothing to do
        }

        public CustomArgumentsConstraint(string pattern, RegexOptions options)
            : base(pattern, options)
        {
            // Nothing to do
        }

        public CustomArgumentsConstraint(string pattern, TimeSpan? timeout)
            : base(pattern, timeout: timeout)
        {
            // Nothing to do
        }

        public CustomArgumentsConstraint(string pattern)
            : base(pattern)
        {
            // Nothing to do
        }
    }
}