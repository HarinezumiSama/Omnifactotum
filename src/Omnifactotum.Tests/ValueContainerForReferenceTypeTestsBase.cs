using System;
using System.Collections.Immutable;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Abstractions;
using Omnifactotum.NUnit;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests
{
    internal abstract class ValueContainerForReferenceTypeTestsBase<TValue> : ValueContainerTestsBase<TValue>
        where TValue : class, IEquatable<TValue>
    {
        private readonly ImmutableArray<TValue?> _allValues;

        protected ValueContainerForReferenceTypeTestsBase(TValue value, TValue anotherValue)
            : base(value, anotherValue)
        {
            Assert.That(Value, Is.Not.EqualTo(default(TValue)));
            Assert.That(Value, Is.Not.EqualTo(default(TValue?)));
            Assert.That(Value, Is.Not.Null);

            Assert.That(AnotherValue, Is.Not.EqualTo(default(TValue)));
            Assert.That(AnotherValue, Is.Not.EqualTo(default(TValue?)));
            Assert.That(AnotherValue, Is.Not.Null);

            var defaultValue = default(TValue?);
            Assert.That(defaultValue, Is.Null);

            _allValues = Values.Cast<TValue?>().Concat(new[] { defaultValue }).ToImmutableArray();
        }

        [Test]
        public override void TestConstructionWithValueIncludingDefaultValue()
        {
            foreach (var value in _allValues)
            {
                var container = new ValueContainer<TValue?>(value);
                Assert.That(container.Value, CreateValueEqualityConstraint(value));

                IValueContainer<TValue?> interfacedContainer = new ValueContainer<TValue?>(value);
                Assert.That(interfacedContainer.Value, CreateValueEqualityConstraint(value));
            }
        }

        [Test]
        public override void TestEqualityIncludingDefaultValue()
        {
            var container = new ValueContainer<TValue?>(Value);
            var containerDefault1 = new ValueContainer<TValue?>(default);
            var containerDefault2 = new ValueContainer<TValue?>(default);

            NUnitFactotum.AssertEquality(containerDefault1, containerDefault2, AssertEqualityExpectation.EqualAndCannotBeSame);
            NUnitFactotum.AssertEquality(container, containerDefault1, AssertEqualityExpectation.NotEqual);
        }

        [Test]
        public override void TestToStringIncludingDefaultValue()
        {
            foreach (var value in _allValues)
            {
                var container = new ValueContainer<TValue?>(value);

                Assert.That(
                    container.ToString(),
                    Is.EqualTo(AsInvariant($@"{{ {nameof(ValueContainer<TValue>.Value)} = {value.ToStringSafelyInvariant()} }}")));
            }
        }
    }
}