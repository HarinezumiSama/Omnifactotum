﻿using System;
using System.Collections.Generic;
#if !NET40
using System.Collections.ObjectModel;
#endif
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    internal sealed class OmnifactotumDictionaryExtensionsTests
    {
        private const int Key1 = 1;
        private const int Key2 = 2;
        private const Dictionary<int?, Version> NullDictionary = null;

        private static readonly Version Value1 = new Version(1, 0);
        private static readonly Version ExplicitValue2 = new Version(2, 34);

        private Dictionary<int?, Version> _dictionary;
        private int _initialCount;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Assert.That(new[] { Key1, Key2 }, Is.Unique);
            Assert.That(new[] { Value1, ExplicitValue2 }, Is.Unique);
        }

        [TearDown]
        public void TearDown()
        {
            _dictionary = null;
        }

        [Test]
        public void TestGetValueOrDefaultNegative()
        {
            //// ReSharper disable AssignNullToNotNullAttribute
            Assert.That(() => NullDictionary.GetValueOrDefault(Key1), Throws.TypeOf<ArgumentNullException>());

            RecreateDictionary();
            Assert.That(() => _dictionary.GetValueOrDefault(null), Throws.TypeOf<ArgumentNullException>());
            AssertInitialDictionaryState();

            //// ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void TestGetValueOrDefault()
        {
            RecreateDictionary();
#if NETCOREAPP2_1
            Assert.That(OmnifactotumDictionaryExtensions.GetValueOrDefault(_dictionary, Key1), Is.EqualTo(Value1));
#else
            Assert.That(_dictionary.GetValueOrDefault(Key1), Is.EqualTo(Value1));
#endif
            AssertInitialDictionaryState();

            RecreateDictionary();
#if NETCOREAPP2_1
            Assert.That(OmnifactotumDictionaryExtensions.GetValueOrDefault(_dictionary, Key2), Is.EqualTo(null));
#else
            Assert.That(_dictionary.GetValueOrDefault(Key2), Is.EqualTo(null));
#endif
            AssertInitialDictionaryState();

            RecreateDictionary();
#if NETCOREAPP2_1
            Assert.That(OmnifactotumDictionaryExtensions.GetValueOrDefault(_dictionary, Key2, null), Is.EqualTo(null));
#else
            Assert.That(_dictionary.GetValueOrDefault(Key2, null), Is.EqualTo(null));
#endif
            AssertInitialDictionaryState();

            RecreateDictionary();
#if NETCOREAPP2_1
            Assert.That(OmnifactotumDictionaryExtensions.GetValueOrDefault(_dictionary, Key2, ExplicitValue2), Is.EqualTo(ExplicitValue2));
#else
            Assert.That(_dictionary.GetValueOrDefault(Key2, ExplicitValue2), Is.EqualTo(ExplicitValue2));
#endif
            AssertInitialDictionaryState();
        }

        [Test]
        public void TestGetOrCreateValueNegative()
        {
            //// ReSharper disable AssignNullToNotNullAttribute
            Assert.That(() => NullDictionary.GetOrCreateValue(Key1), Throws.TypeOf<ArgumentNullException>());

            RecreateDictionary();
            Assert.That(() => _dictionary.GetOrCreateValue(null), Throws.TypeOf<ArgumentNullException>());
            AssertInitialDictionaryState();

            RecreateDictionary();
            Assert.That(() => _dictionary.GetOrCreateValue(Key1, null), Throws.TypeOf<ArgumentNullException>());
            AssertInitialDictionaryState();

            //// ReSharper restore AssignNullToNotNullAttribute
        }

        [Test]
        public void TestGetOrCreateValue()
        {
            RecreateDictionary();
            Assert.That(_dictionary.GetOrCreateValue(Key1), Is.EqualTo(Value1));
            AssertInitialDictionaryState();

            RecreateDictionary();
            Assert.That(_dictionary.GetOrCreateValue(Key2), Is.EqualTo(new Version()));
            AssertValueCreatedDictionaryState(new Version());

            RecreateDictionary();
            Assert.That(_dictionary.GetOrCreateValue(Key2, key => null), Is.EqualTo(null));
            AssertValueCreatedDictionaryState(null);

            RecreateDictionary();
            Assert.That(_dictionary.GetOrCreateValue(Key2, key => ExplicitValue2), Is.EqualTo(ExplicitValue2));
            AssertValueCreatedDictionaryState(ExplicitValue2);
        }

        [Test]
        public void TestAsReadOnly()
        {
            RecreateDictionary();
            var readOnlyDictionary = _dictionary.AsReadOnly();

            Assert.That(readOnlyDictionary, Is.InstanceOf<ReadOnlyDictionary<int?, Version>>());
        }

        private void RecreateDictionary()
        {
            _dictionary = new Dictionary<int?, Version>
            {
                { Key1, Value1 }
            };

            _initialCount = _dictionary.Count;

            AssertInitialDictionaryState();
        }

        private void AssertInitialDictionaryState()
        {
            Assert.That(_initialCount, Is.EqualTo(1));

            Assert.That(_dictionary.Count, Is.EqualTo(_initialCount));
            Assert.That(_dictionary.ContainsKey(Key1), Is.True);
            Assert.That(_dictionary.ContainsKey(Key2), Is.False);
        }

        private void AssertValueCreatedDictionaryState([CanBeNull] Version expectedValue)
        {
            Assert.That(_dictionary.Count, Is.EqualTo(_initialCount + 1));
            Assert.That(_dictionary[Key1], Is.EqualTo(Value1));
            Assert.That(_dictionary[Key2], Is.EqualTo(expectedValue));
        }
    }
}