﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

#if !NET7_0_OR_GREATER
using System.Collections.ObjectModel;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumDictionaryExtensions))]
internal sealed class OmnifactotumDictionaryExtensionsTests
{
    private const string Key1 = "1";
    private const string Key2 = "2";
    private const Dictionary<string, Version>? NullDictionary = null;

    private static readonly Version Value1 = new(1, 0);
    private static readonly Version ExplicitValue2 = new(2, 34);

    private Dictionary<string, Version>? _dictionary;
    private int _initialCount;

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        Assert.That(new[] { Key1, Key2 }, Is.Unique);
        Assert.That(new[] { Value1, ExplicitValue2 }, Is.Unique);
    }

    [TearDown]
    public void TearDown() => _dictionary = null;

    [Test]
    public void TestGetOrCreateValueNegative()
    {
        //// ReSharper disable AssignNullToNotNullAttribute
        Assert.That(() => NullDictionary!.GetOrCreateValue(Key1), Throws.TypeOf<ArgumentNullException>());

        RecreateDictionary();
        Assert.That(() => _dictionary.AssertNotNull().GetOrCreateValue(null!), Throws.TypeOf<ArgumentNullException>());
        AssertInitialDictionaryState();

        RecreateDictionary();
        Assert.That(() => _dictionary.AssertNotNull().GetOrCreateValue(Key1, null!), Throws.TypeOf<ArgumentNullException>());
        AssertInitialDictionaryState();

        //// ReSharper restore AssignNullToNotNullAttribute
    }

    [Test]
    public void TestGetOrCreateValue()
    {
        RecreateDictionary();
        Assert.That(_dictionary!.GetOrCreateValue(Key1), Is.EqualTo(Value1));
        AssertInitialDictionaryState();

        RecreateDictionary();
        Assert.That(_dictionary!.GetOrCreateValue(Key2), Is.EqualTo(new Version()));
        AssertValueCreatedDictionaryState(new Version());

        RecreateDictionary();
        Assert.That(_dictionary!.GetOrCreateValue(Key2, _ => null!), Is.Null);
        AssertValueCreatedDictionaryState(null);

        RecreateDictionary();
        Assert.That(_dictionary!.GetOrCreateValue(Key2, _ => ExplicitValue2), Is.EqualTo(ExplicitValue2));
        AssertValueCreatedDictionaryState(ExplicitValue2);
    }

#if !NET7_0_OR_GREATER
    [Test]
    public void TestAsReadOnly()
    {
        RecreateDictionary();
        var readOnlyDictionary = _dictionary!.AsReadOnly();

        Assert.That(readOnlyDictionary, Is.InstanceOf<ReadOnlyDictionary<string, Version>>());
    }
#endif

    private void RecreateDictionary()
    {
        _dictionary = new Dictionary<string, Version>
        {
            { Key1, Value1 }
        };

        _initialCount = _dictionary.Count;

        AssertInitialDictionaryState();
    }

    private void AssertInitialDictionaryState()
    {
        Assert.That(_initialCount, Is.EqualTo(1));

        Assert.That(_dictionary!.Count, Is.EqualTo(_initialCount));
        Assert.That(_dictionary.ContainsKey(Key1), Is.True);
        Assert.That(_dictionary.ContainsKey(Key2), Is.False);
    }

    private void AssertValueCreatedDictionaryState([CanBeNull] Version? expectedValue)
    {
        Assert.That(_dictionary!.Count, Is.EqualTo(_initialCount + 1));
        Assert.That(_dictionary[Key1], Is.EqualTo(Value1));
        Assert.That(_dictionary[Key2], Is.EqualTo(expectedValue));
    }
}