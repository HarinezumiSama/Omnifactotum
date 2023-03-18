using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(ComparableObjectBase))]
internal sealed class ComparableObjectBaseTests
{
#pragma warning disable CS1718 // (Comparison made to same variable) - Testing the overloaded operators
    [Test]
    [SuppressMessage("ReSharper", "EqualExpressionComparison")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    [SuppressMessage("ReSharper", "Custom.ComparingExpressionToNullCanBeConvertedToPatternMatching")]
    public void TestObjectMethods()
    {
        const int Value1 = 23;
        const int Value2 = 29;

        Assert.That(Value1, Is.Not.EqualTo(Value2));

        var object1A = new TestDescendant(Value1);
        var object1B = new TestDescendant(Value1);
        var object2 = new TestDescendant(Value2);
        var otherTypeObject = new OtherTestDescendant();

        NUnitFactotum.AssertEquality(object1A, null, AssertEqualityExpectation.NotEqual);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(object1A, null!), Is.Positive);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(null!, object1A), Is.Negative);
        Assert.That(() => Comparer.Default.Compare(object1A, null), Is.Positive);
        Assert.That(() => Comparer.Default.Compare(null, object1A), Is.Negative);
        Assert.That(() => object1A.CompareTo(null), Is.Positive);
        Assert.That(() => ((IComparable)object1A).CompareTo(null), Is.Positive);
        Assert.That(() => object1A == null, Is.False);
        Assert.That(() => object1A <= null, Is.False);
        Assert.That(() => object1A < null, Is.False);
        Assert.That(() => object1A >= null, Is.True);
        Assert.That(() => object1A > null, Is.True);
        Assert.That(() => null == object1A, Is.False);
        Assert.That(() => null <= object1A, Is.True);
        Assert.That(() => null < object1A, Is.True);
        Assert.That(() => null >= object1A, Is.False);
        Assert.That(() => null > object1A, Is.False);

        Assert.That(
            () => ((IComparable)object1A).CompareTo(new object()),
            Throws.ArgumentException.With.Message.EqualTo(@"Incompatible comparand type: ""System.Object"". (Parameter 'obj')"));

        AssertSelfEquality<TestDescendant>(null);
        AssertSelfEquality(object1A);
        AssertSelfEquality(object1B);
        AssertSelfEquality(object2);
        AssertSelfEquality(otherTypeObject);

        AssertEquality(object1A, object1B);

        NUnitFactotum.AssertEquality(object1A, object2, AssertEqualityExpectation.NotEqual);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(object1A, object2), Is.Negative);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(object2, object1A), Is.Positive);
        Assert.That(() => Comparer.Default.Compare(object1A, object2), Is.Negative);
        Assert.That(() => Comparer.Default.Compare(object2, object1A), Is.Positive);
        Assert.That(() => object1A == object2, Is.False);
        Assert.That(() => object1A <= object2, Is.True);
        Assert.That(() => object1A >= object2, Is.False);
        Assert.That(() => object1A != object2, Is.True);

        NUnitFactotum.AssertEquality(object1B, object2, AssertEqualityExpectation.NotEqual);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(object1B, object2), Is.Negative);
        Assert.That(() => Comparer<TestDescendant>.Default.Compare(object2, object1B), Is.Positive);
        Assert.That(() => Comparer.Default.Compare(object1B, object2), Is.Negative);
        Assert.That(() => Comparer.Default.Compare(object2, object1B), Is.Positive);
        Assert.That(() => object1B == object2, Is.False);
        Assert.That(() => object1B <= object2, Is.True);
        Assert.That(() => object1B >= object2, Is.False);
        Assert.That(() => object1B != object2, Is.True);

        const string IncompatibleTypeMessage1 =
            @"Incompatible comparand types: ""Omnifactotum.Tests.ComparableObjectBaseTests.OtherTestDescendant"""
            + @" and ""Omnifactotum.Tests.ComparableObjectBaseTests.TestDescendant"".";

        Assert.That(() => otherTypeObject.Equals(object1A), Is.False);
        Assert.That(() => otherTypeObject.CompareTo(object1A), Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));
        Assert.That(() => ((IComparable)otherTypeObject).CompareTo(object1A), Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));
        Assert.That(() => otherTypeObject == object1A, Is.False);
        Assert.That(() => otherTypeObject != object1A, Is.True);
        Assert.That(() => otherTypeObject < object1A, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));
        Assert.That(() => otherTypeObject <= object1A, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));
        Assert.That(() => otherTypeObject > object1A, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));
        Assert.That(() => otherTypeObject >= object1A, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage1));

        const string IncompatibleTypeMessage2 =
            @"Incompatible comparand types: ""Omnifactotum.Tests.ComparableObjectBaseTests.TestDescendant"""
            + @" and ""Omnifactotum.Tests.ComparableObjectBaseTests.OtherTestDescendant"".";

        Assert.That(() => object1A.Equals(otherTypeObject), Is.False);
        Assert.That(() => object1A.CompareTo(otherTypeObject), Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));
        Assert.That(() => ((IComparable)object1A).CompareTo(otherTypeObject), Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));
        Assert.That(() => object1A == otherTypeObject, Is.False);
        Assert.That(() => object1A != otherTypeObject, Is.True);
        Assert.That(() => object1A < otherTypeObject, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));
        Assert.That(() => object1A <= otherTypeObject, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));
        Assert.That(() => object1A > otherTypeObject, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));
        Assert.That(() => object1A >= otherTypeObject, Throws.ArgumentException.With.Message.EqualTo(IncompatibleTypeMessage2));

        //// ReSharper disable SuspiciousTypeConversion.Global
        Assert.That(() => ((object)otherTypeObject).Equals(object1A), Is.False);
        Assert.That(() => ((object)object1A).Equals(otherTypeObject), Is.False);
        //// ReSharper restore SuspiciousTypeConversion.Global

        Assert.That(() => otherTypeObject == object1A, Is.False);
        Assert.That(() => object1A == otherTypeObject, Is.False);
        Assert.That(() => otherTypeObject != object1A, Is.True);
        Assert.That(() => object1A != otherTypeObject, Is.True);

        static void AssertSelfEquality<T>(T? obj)
            where T : ComparableObjectBase
            => AssertEquality(obj, obj);

        static void AssertEquality<T>(T? value1, T? value2)
            where T : ComparableObjectBase
        {
            NUnitFactotum.AssertEquality(value1, value2, AssertEqualityExpectation.EqualAndMayBeSame);

            Assert.That(() => Comparer<T>.Default.Compare(value1!, value2!), Is.Zero);
            Assert.That(() => Comparer.Default.Compare(value1, value2), Is.Zero);

            if (value1 is not null)
            {
                Assert.That(() => value1.CompareTo(value2), Is.Zero);
                Assert.That(() => ((IComparable)value1).CompareTo(value2), Is.Zero);
            }

            if (value2 is not null)
            {
                Assert.That(() => value2.CompareTo(value1), Is.Zero);
                Assert.That(() => ((IComparable)value2).CompareTo(value1), Is.Zero);
            }

            Assert.That(() => value1 == value2, Is.True);
            Assert.That(() => value1 <= value2, Is.True);
            Assert.That(() => value1 >= value2, Is.True);
            Assert.That(() => value1 != value2, Is.False);
            Assert.That(() => value1 < value2, Is.False);
            Assert.That(() => value1 > value2, Is.False);
        }
    }
#pragma warning restore CS1718

    private sealed class TestDescendant : ComparableObjectBase
    {
        internal TestDescendant(int value) => Value = value;

        private int Value { get; }

        protected override int GetHashCodeInternal() => Value.GetHashCode();

        protected override int CompareToInternal(ComparableObjectBase other)
        {
            var castOther = other.AssertCast().To<TestDescendant>();
            return Comparer<int>.Default.Compare(Value, castOther.Value);
        }
    }

    private sealed class OtherTestDescendant : ComparableObjectBase
    {
        protected override int GetHashCodeInternal() => 0;

        protected override int CompareToInternal(ComparableObjectBase other) => throw new InvalidOperationException();
    }
}