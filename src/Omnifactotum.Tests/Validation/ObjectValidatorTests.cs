using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.Validation;

[TestFixture(TestOf = typeof(ObjectValidator))]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
internal sealed partial class ObjectValidatorTests
{
    private const string ValidationResultPropertyName = nameof(ObjectValidationException.ValidationResult);

    [SetUp]
    public void SetUp() => ClearLastMemberContexts();

    [TearDown]
    public void TearDown() => ClearLastMemberContexts();

    [Test]
    public void TestValidateWhenArgumentIsNull()
    {
        Assert.That(() => ObjectValidator.Validate<SimpleData>(null!), Throws.ArgumentNullException);
        Assert.That(() => ObjectValidator.Validate<SimpleData>(null!, "expression-2ca70053810c4ac6ad1e0c1fc65dd289"), Throws.ArgumentNullException);
    }

    [Test]
    public void TestValidateWhenValidationSucceeded()
    {
        var data1 = new ComplexData
        {
            Data = new SimpleData { StartDate = new DateTime(2023, 11, 13, 15, 17, 19, DateTimeKind.Utc), NullableValue = 0, Value = "A" },
#if NET7_0_OR_GREATER
            GenericMemberConstraintAttributeData = new SimpleData
            {
                StartDate = new DateTime(2023, 11, 13, 15, 17, 23, DateTimeKind.Utc),
                NullableValue = 0,
                Value = "A"
            },
#endif
            NonEmptyValue = "B",
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C1" } },
            AnotherSimpleDataArray = new[] { new AnotherSimpleData { Value = "C2" } },
#if NET7_0_OR_GREATER
            GenericMemberItemConstraintAnotherSimpleDataArray = Array.Empty<AnotherSimpleData>(),
#endif
            AnotherSimpleDataCollection = new Collection<AnotherSimpleData>(),
            AnotherSimpleDataCustomEnumerable = new CustomEnumerable(),
            AnotherSimpleDataCustomGenericEnumerable = new CustomGenericEnumerable<AnotherSimpleData>(),
            AnotherSimpleDataCustomGenericEnumerableObject = new CustomGenericEnumerable<AnotherSimpleData>(),
            AnotherSimpleDataCustomGenericList = new CustomGenericList<AnotherSimpleData>(),
            AnotherSimpleDataCustomList = new CustomList(),
            AnotherSimpleDataCustomReadOnlyList = new CustomReadOnlyList<AnotherSimpleData>(),
            AnotherSimpleDataImmutableList = ImmutableList<AnotherSimpleData>.Empty,
            ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
            ImmutableStrings = ImmutableArray.Create("E"),
            NullableImmutableStrings = ImmutableArray<string>.Empty,
            AnotherNullableImmutableStrings = ImmutableArray<string>.Empty,
            SingleBaseData = new AnotherSimpleData { Value = "Q" }
        };

        EnsureTestValidationSucceeded(data1);
        EnsureTestValidationSucceeded(data1.AsArray());
        EnsureTestValidationSucceeded(ImmutableArray.Create(data1));

        var data2 = new ComplexData
        {
            Data = new SimpleData { StartDate = new DateTime(2023, 7, 13, 15, 17, 19, DateTimeKind.Utc), NullableValue = 0, Value = "A" },
#if NET7_0_OR_GREATER
            GenericMemberConstraintAttributeData = new SimpleData
            {
                StartDate = new DateTime(2023, 7, 13, 15, 17, 23, DateTimeKind.Utc),
                NullableValue = 0,
                Value = "A"
            },
#endif
            NonEmptyValue = "B",
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C1" } },
            AnotherSimpleDataArray = new[] { new AnotherSimpleData { Value = "C2" } },
#if NET7_0_OR_GREATER
            GenericMemberItemConstraintAnotherSimpleDataArray = new[] { new AnotherSimpleData { Value = "C3" } },
#endif
            AnotherSimpleDataCollection = new Collection<AnotherSimpleData> { new() { Value = "C4" } },
            AnotherSimpleDataCustomEnumerable = new CustomEnumerable(new AnotherSimpleData { Value = "C5" }),
            AnotherSimpleDataCustomGenericEnumerable = new CustomGenericEnumerable<AnotherSimpleData>(new AnotherSimpleData { Value = "C6" }),
            AnotherSimpleDataCustomGenericEnumerableObject = new CustomGenericEnumerable<AnotherSimpleData>(new AnotherSimpleData { Value = "C7" }),
            AnotherSimpleDataCustomGenericList = new CustomGenericList<AnotherSimpleData>(new AnotherSimpleData { Value = "C8" }),
            AnotherSimpleDataCustomList = new CustomList(new AnotherSimpleData { Value = "C9" }),
            AnotherSimpleDataCustomReadOnlyList = new CustomReadOnlyList<AnotherSimpleData>(new AnotherSimpleData { Value = "C10" }),
            AnotherSimpleDataImmutableList = ImmutableList.Create(new AnotherSimpleData { Value = "C11" }),
            ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
            ImmutableStrings = ImmutableArray<string>.Empty,
            NullableImmutableStrings = ImmutableArray.Create("F1"),
            AnotherNullableImmutableStrings = ImmutableArray.Create("F2")
        };

        EnsureTestValidationSucceeded(data2);
        EnsureTestValidationSucceeded(data2.AsArray());
        EnsureTestValidationSucceeded(ImmutableArray.Create(data2));
    }

    [Test]
    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeEvident")]
    public void TestValidateWhenValidationFailed()
    {
        var dataContainer = new SimpleContainer<ComplexData>
        {
            ContainedValue = new ComplexData
            {
                Data = new SimpleData { StartDate = new DateTime(2023, 12, 29, 11, 59, 43).AsKind(DateTimeKind.Local) },
#if NET7_0_OR_GREATER
                GenericMemberConstraintAttributeData = null,
#endif
                NonEmptyValue = string.Empty,
                MultipleDataItems = new BaseAnotherSimpleData[]
                {
                    new AnotherSimpleData { Value = "C1" },
                    new AnotherSimpleData()
                },
                AnotherSimpleDataArray = new[]
                {
                    new AnotherSimpleData { Value = "C2" },
                    new AnotherSimpleData()
                },
#if NET7_0_OR_GREATER
                GenericMemberItemConstraintAnotherSimpleDataArray = new[] { null!, new AnotherSimpleData { Value = "C3" } },
#endif
                AnotherSimpleDataCollection = new Collection<AnotherSimpleData>
                {
                    new AnotherSimpleData { Value = "C4" },
                    new AnotherSimpleData()
                },
                AnotherSimpleDataCustomEnumerable = new CustomEnumerable(
                    new AnotherSimpleData(),
                    new AnotherSimpleData { Value = "C5" }),
                AnotherSimpleDataCustomGenericEnumerable = new CustomGenericEnumerable<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C6" },
                    new AnotherSimpleData { Value = "C7" },
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomGenericEnumerableObject = new CustomGenericEnumerable<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C7-O" },
                    new AnotherSimpleData { Value = "C7-O" },
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomGenericList = new CustomGenericList<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C8" },
                    new AnotherSimpleData(),
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomList = new CustomList(
                    new AnotherSimpleData { Value = "C9" },
                    new AnotherSimpleData(),
                    new AnotherSimpleData { Value = "C10" },
                    null,
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomReadOnlyList = new CustomReadOnlyList<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C11" },
                    new AnotherSimpleData { Value = "C12" },
                    new AnotherSimpleData { Value = "C13" },
                    new AnotherSimpleData()),
                AnotherSimpleDataImmutableList = ImmutableList.Create(
                    new AnotherSimpleData { Value = "C14" },
                    new AnotherSimpleData { Value = "C15" },
                    new AnotherSimpleData()),
                ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
                AnotherNullableImmutableStrings = ImmutableArray.Create("E", null!, "F"),
                SingleBaseData = new AnotherSimpleData()
            }
        };

        var validationResult = ObjectValidator.Validate(dataContainer);

#if NET5_0_OR_GREATER
        const string InstanceExpression = nameof(dataContainer);
#else
        const string InstanceExpression = ObjectValidator.DefaultRootObjectParameterName;
#endif

        var expectedTypeToExpressionsMap = new Dictionary<Type, string[]>
        {
            {
                typeof(NotNullConstraint),
                new[]
                {
                    $"{InstanceExpression}.ContainedValue.Data.Value",
                    $"{InstanceExpression}.ContainedValue.Data.NullableValue",
#if NET7_0_OR_GREATER
                    $"{InstanceExpression}.ContainedValue.GenericMemberConstraintAttributeData",
                    $"{InstanceExpression}.ContainedValue.GenericMemberItemConstraintAnotherSimpleDataArray[0]",
#endif
                    $"{InstanceExpression}.ContainedValue.NullableImmutableStrings",
                    $"{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]",
                    $"Convert({InstanceExpression}.ContainedValue.MultipleDataItems[1], AnotherSimpleData).Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataArray[1].Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCollection.Item[1].Value",
                    $"Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomEnumerable.Cast().First(), AnotherSimpleData).Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericEnumerable.Skip(2).First().Value",
                    $"Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericEnumerableObject, IEnumerable`1).Skip(2).First().Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[1].Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[2].Value",
                    $"Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[1], AnotherSimpleData).Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[3]",
                    $"Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[4], AnotherSimpleData).Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomReadOnlyList.Item[3].Value",
                    $"{InstanceExpression}.ContainedValue.AnotherSimpleDataImmutableList.Item[2].Value",
                    $"Convert({InstanceExpression}.ContainedValue.SingleBaseData, AnotherSimpleData).Value"
                }
            },
            {
                typeof(NotNullConstraint<string>),
                new[]
                {
                    $"{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]"
                }
            },
#if NET7_0_OR_GREATER
            {
                typeof(NotNullConstraint<SimpleData>),
                new[]
                {
                    $"{InstanceExpression}.ContainedValue.GenericMemberConstraintAttributeData"
                }
            },
#endif
            {
                typeof(NotNullAndNotEmptyStringConstraint),
                new[] { $"{InstanceExpression}.ContainedValue.NonEmptyValue" }
            },
            {
                typeof(UtcDateConstraint),
                new[] { $"{InstanceExpression}.ContainedValue.Data.StartDate" }
            },
            {
                typeof(UtcDateTypedConstraint),
                new[] { $"{InstanceExpression}.ContainedValue.Data.StartDate" }
            }
        };

        Assert.That(validationResult, Is.Not.Null);
        Assert.That(() => validationResult.IsObjectValid, Is.False);

        var validationException = validationResult.GetException().AssertNotNull();
        Assert.That(validationException, Is.TypeOf<ObjectValidationException>());
        Assert.That(() => validationException.ValidationResult, Is.SameAs(validationResult));
        Assert.That(() => validationException.Message, Is.EqualTo(validationResult.FailureMessage));

        Assert.That(
            () => validationResult.EnsureSucceeded(),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .SameAs(validationResult));

        var groupedErrorMap = validationResult.Errors
            .GroupBy(error => error.FailedConstraintType)
            .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(error => error.Context.Expression.ToString()).ToArray());

        Assert.That(() => groupedErrorMap.Keys, Is.EquivalentTo(expectedTypeToExpressionsMap.Keys));

        foreach (var pair in expectedTypeToExpressionsMap)
        {
            Assert.That(
                () => groupedErrorMap[pair.Key],
                Is.EquivalentTo(pair.Value),
                $"Expression list mismatch for the constraint type {pair.Key.GetFullName().ToUIString()}.");
        }

        Assert.That(() => validationResult.Errors.Count, Is.EqualTo(expectedTypeToExpressionsMap.SelectMany(pair => pair.Value).Count()));

        var utcDateErrorExpression = validationResult
            .Errors
            .Single(obj => obj.FailedConstraintType == typeof(UtcDateConstraint))
            .Context
            .CreateLambdaExpression()
            .AssertNotNull();

        Assert.That(() => utcDateErrorExpression.Compile().Invoke(dataContainer), Is.EqualTo(dataContainer.ContainedValue.Data.StartDate));

        var expectedExceptionMessageItems = new List<string>();

        expectedExceptionMessageItems.AddRange(
            new[]
            {
                $"[{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]] The 'string' value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataArray[1].Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCollection.Item[1].Value] The value cannot be null.",
                $"[Convert({InstanceExpression
                }.ContainedValue.AnotherSimpleDataCustomEnumerable.Cast().First(), AnotherSimpleData).Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericEnumerable.Skip(2).First().Value] The value cannot be null.",
                $"[Convert({InstanceExpression
                }.ContainedValue.AnotherSimpleDataCustomGenericEnumerableObject, IEnumerable`1).Skip(2).First().Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[1].Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[2].Value] The value cannot be null.",
                $"[Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[1], AnotherSimpleData).Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[3]] The value cannot be null.",
                $"[Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[4], AnotherSimpleData).Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomReadOnlyList.Item[3].Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.AnotherSimpleDataImmutableList.Item[2].Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.Data.NullableValue] The value cannot be null.",
                $"[{InstanceExpression
                }.ContainedValue.Data.StartDate] Validation of the constraint \"Omnifactotum.Tests.Validation.ObjectValidatorTests.UtcDateConstraint\" failed.",
                $"[{InstanceExpression
                }.ContainedValue.Data.StartDate] Validation of the constraint \"Omnifactotum.Tests.Validation.ObjectValidatorTests.UtcDateTypedConstraint\" failed.",
                $"[{InstanceExpression}.ContainedValue.Data.Value] The value cannot be null."
            });

#if NET7_0_OR_GREATER
        expectedExceptionMessageItems.AddRange(
            new[]
            {
                $"[{InstanceExpression}.ContainedValue.GenericMemberConstraintAttributeData] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.GenericMemberConstraintAttributeData] The 'ObjectValidatorTests.SimpleData' value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.GenericMemberItemConstraintAnotherSimpleDataArray[0]] The value cannot be null.",
            });
#endif

        expectedExceptionMessageItems.AddRange(
            new[]
            {
                $"[Convert({InstanceExpression}.ContainedValue.MultipleDataItems[1], AnotherSimpleData).Value] The value cannot be null.",
                $"[{InstanceExpression}.ContainedValue.NonEmptyValue] The value must not be null or an empty string.",
                $"[{InstanceExpression}.ContainedValue.NullableImmutableStrings] The value cannot be null.",
                $"[Convert({InstanceExpression}.ContainedValue.SingleBaseData, AnotherSimpleData).Value] The value cannot be null.",
            });

        var expectedExceptionMessage = expectedExceptionMessageItems
            .Select((s, i) => AsInvariant($"[{i + 1}/{expectedExceptionMessageItems.Count}] {s}"))
            .Join(Environment.NewLine);

        Assert.That(() => validationException.Message, Is.EqualTo(expectedExceptionMessage));

        AssertLastMemberContexts();

        ClearLastMemberContexts();

        Assert.That(
            () => ObjectValidator.EnsureValid(dataContainer),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .With
                .Property(nameof(ObjectValidationResult.FailureMessage))
                .EqualTo(expectedExceptionMessage));

        AssertLastMemberContexts();
    }

    [Test]
    [TestCase("")]
    [TestCase("\x0020")]
    [TestCase("\t")]
    [TestCase("\t\x0020\t\r\n")]
    public void TestValidateWhenInvalidInstanceExpressionThenThrows(string? explicitInstanceExpression)
    {
        var dataContainer = new SimpleContainer<SimpleData>
        {
            ContainedValue = new SimpleData { StartDate = DateTime.Now }
        };

        Assert.That(
            () => ObjectValidator.Validate(dataContainer, explicitInstanceExpression),
            Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("instanceExpression"));
    }

    [Test]
    [TestCase(null)]
    [TestCase("var_842c9c8e")]
    [TestCase("[var1.var2.var3]")]
    [TestCase("[var:SimpleContainer<SimpleData>]")]
    public void TestValidateWhenValidInstanceExpressionAndValidationFailed(string? explicitInstanceExpression)
    {
        var expectedInstanceExpression = explicitInstanceExpression ?? ObjectValidator.DefaultRootObjectParameterName;

        var dataContainer = new SimpleContainer<SimpleData>
        {
            ContainedValue = new SimpleData { StartDate = DateTime.Now }
        };

        var validationResult = ObjectValidator.Validate(dataContainer, explicitInstanceExpression);

        var expectedTypeToExpressionsMap = new Dictionary<Type, string[]>
        {
            {
                typeof(NotNullConstraint),
                new[]
                {
                    $"{expectedInstanceExpression}.ContainedValue.Value",
                    $"{expectedInstanceExpression}.ContainedValue.NullableValue"
                }
            },
            {
                typeof(UtcDateConstraint),
                new[] { $"{expectedInstanceExpression}.ContainedValue.StartDate" }
            },
            {
                typeof(UtcDateTypedConstraint),
                new[] { $"{expectedInstanceExpression}.ContainedValue.StartDate" }
            }
        };

        Assert.That(validationResult, Is.Not.Null);
        Assert.That(() => validationResult.IsObjectValid, Is.False);

        var validationException = validationResult.GetException().AssertNotNull();
        Assert.That(validationException, Is.TypeOf<ObjectValidationException>());
        Assert.That(() => validationException.ValidationResult, Is.SameAs(validationResult));
        Assert.That(() => validationException.Message, Is.EqualTo(validationResult.FailureMessage));

        Assert.That(
            () => validationResult.EnsureSucceeded(),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .SameAs(validationResult));

        var groupedErrorMap = validationResult.Errors
            .GroupBy(error => error.FailedConstraintType)
            .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(error => error.Context.Expression.ToString()).ToArray());

        Assert.That(() => groupedErrorMap.Keys, Is.EquivalentTo(expectedTypeToExpressionsMap.Keys));

        foreach (var pair in expectedTypeToExpressionsMap)
        {
            Assert.That(
                () => groupedErrorMap[pair.Key],
                Is.EquivalentTo(pair.Value),
                $"Expression list mismatch for the constraint type {pair.Key.GetFullName().ToUIString()}.");
        }

        Assert.That(() => validationResult.Errors.Count, Is.EqualTo(expectedTypeToExpressionsMap.SelectMany(pair => pair.Value).Count()));

        var utcDateErrorExpression = validationResult
            .Errors
            .Single(obj => obj.FailedConstraintType == typeof(UtcDateConstraint))
            .Context
            .CreateLambdaExpression()
            .AssertNotNull();

        Assert.That(() => utcDateErrorExpression.Compile().Invoke(dataContainer), Is.EqualTo(dataContainer.ContainedValue.StartDate));

        Assert.That(
            () => ObjectValidator.EnsureValid(dataContainer, explicitInstanceExpression),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .With
                .Property(nameof(ObjectValidationResult.FailureMessage))
                .EqualTo(validationResult.FailureMessage));
    }

    [Test]
    public void TestValidateDictionaryWhenValidationFailed()
    {
        var mapContainer = new MapContainer
        {
            Properties = new Dictionary<string, SimpleContainer<int?>>
            {
                { string.Empty, new SimpleContainer<int?> { ContainedValue = 0 } },
                { "abc", new SimpleContainer<int?> { ContainedValue = 3 } },
                { "x", new SimpleContainer<int?> { ContainedValue = null } }
            }
        };

        var validationResult = ObjectValidator.Validate(mapContainer);

#if NET5_0_OR_GREATER
        const string InstanceExpression = nameof(mapContainer);
#else
        const string InstanceExpression = ObjectValidator.DefaultRootObjectParameterName;
#endif

        var expectedTypeToExpressionsMap = new Dictionary<Type, string[]>
        {
            {
                typeof(NotNullConstraint),
                new[]
                {
                    $"Convert({InstanceExpression}.Properties.Skip(2).First(), KeyValuePair`2).Value.ContainedValue"
                }
            },
            {
                typeof(NotNullAndNotEmptyStringConstraint),
                new[]
                {
                    $"{InstanceExpression}.Properties.First().Key"
                }
            },
            {
                typeof(NotAbcStringConstraint),
                new[]
                {
                    $"{InstanceExpression}.Properties.Skip(1).First().Key"
                }
            },
            {
                typeof(CustomNotAbcStringConstraint),
                new[]
                {
                    $"{InstanceExpression}.Properties.Skip(1).First().Key"
                }
            }
        };

        Assert.That(validationResult, Is.Not.Null);
        Assert.That(() => validationResult.IsObjectValid, Is.False);

        var validationException = validationResult.GetException().AssertNotNull();
        Assert.That(validationException, Is.TypeOf<ObjectValidationException>());
        Assert.That(() => validationException.ValidationResult, Is.SameAs(validationResult));
        Assert.That(() => validationException.Message, Is.EqualTo(validationResult.FailureMessage));

        Assert.That(
            () => validationResult.EnsureSucceeded(),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .SameAs(validationResult));

        var groupedErrorMap = validationResult.Errors
            .GroupBy(error => error.FailedConstraintType)
            .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(error => error.Context.Expression.ToString()).ToArray());

        Assert.That(() => groupedErrorMap.Keys, Is.EquivalentTo(expectedTypeToExpressionsMap.Keys));

        foreach (var pair in expectedTypeToExpressionsMap)
        {
            Assert.That(
                () => groupedErrorMap[pair.Key],
                Is.EquivalentTo(pair.Value),
                $"Expression list mismatch for the constraint type {pair.Key.GetFullName().ToUIString()}.");
        }

        Assert.That(() => validationResult.Errors.Count, Is.EqualTo(expectedTypeToExpressionsMap.SelectMany(pair => pair.Value).Count()));

        var notAbcStringError = validationResult.Errors.Single(obj => obj.FailedConstraintType == typeof(NotAbcStringConstraint));
        Assert.That(() => (string?)notAbcStringError.Context.CreateLambdaExpression("value").Compile().Invoke(mapContainer), Is.EqualTo("abc"));

        AssertLastMemberContexts();

        ClearLastMemberContexts();

        Assert.That(
            () => ObjectValidator.EnsureValid(mapContainer),
            Throws
                .TypeOf<ObjectValidationException>()
                .With
                .Property(ValidationResultPropertyName)
                .With
                .Property(nameof(ObjectValidationResult.FailureMessage))
                .EqualTo(validationResult.FailureMessage));

        AssertLastMemberContexts();
    }

    private static void ClearLastMemberContexts()
    {
#if NET7_0_OR_GREATER
        ClearLastMemberContext<NotAbcStringConstraint>();
        ClearLastMemberContext<CustomNotAbcStringConstraint>();
        ClearLastMemberContext<UtcDateConstraint>();
        ClearLastMemberContext<UtcDateTypedConstraint>();
#else
        NotAbcStringConstraint.LastMemberContext = null;
        CustomNotAbcStringConstraint.LastMemberContext = null;
        UtcDateConstraint.LastMemberContext = null;
        UtcDateTypedConstraint.LastMemberContext = null;
#endif
    }

    private static void AssertLastMemberContexts()
    {
#if NET7_0_OR_GREATER
        AssertLastMemberContext<NotAbcStringConstraint>();
        AssertLastMemberContext<CustomNotAbcStringConstraint>();
        AssertLastMemberContext<UtcDateConstraint>();
        AssertLastMemberContext<UtcDateTypedConstraint>();
#else
        AssertLastMemberContext(NotAbcStringConstraint.LastMemberContext);
        AssertLastMemberContext(CustomNotAbcStringConstraint.LastMemberContext);
        AssertLastMemberContext(UtcDateConstraint.LastMemberContext);
        AssertLastMemberContext(UtcDateTypedConstraint.LastMemberContext);
#endif
    }

#if NET7_0_OR_GREATER
    private static void ClearLastMemberContext<T>()
        where T : class, ILastMemberContextProvider
        => T.LastMemberContext = null;

    private static void AssertLastMemberContext<T>()
        where T : class, ILastMemberContextProvider
        => AssertLastMemberContext(T.LastMemberContext);
#endif

    private static void AssertLastMemberContext(MemberConstraintValidationContext? lastMemberContext)
    {
        if (lastMemberContext is null)
        {
            return;
        }

        var validatorContext = lastMemberContext.ValidatorContext.AssertNotNull();

        Assert.That(() => validatorContext.IsValidationComplete, Is.True);
        Assert.That(() => validatorContext.RecursiveProcessingContext.ItemsBeingProcessed?.Count, Is.Zero);

        var errorCount = validatorContext.Errors.Count;

        Assert.That(
            () => validatorContext.AddError(
                new MemberConstraintValidationError(
                    new MemberConstraintValidationContext(
                        validatorContext,
                        lastMemberContext.Root,
                        lastMemberContext.Container,
                        Expression.Constant(null),
                        lastMemberContext.RootParameterExpression),
                    typeof(NotNullConstraint),
                    "Some valid message.")),
            Throws.InvalidOperationException.With.Message.EqualTo("Object validation is already completed."));

        Assert.That(() => validatorContext.Errors.Count, Is.EqualTo(errorCount));
    }

    private static void EnsureTestValidationSucceeded<T>(T data)
    {
        ClearLastMemberContexts();
        var validationResult1 = ObjectValidator.Validate(data!).AssertNotNull();

        Assert.That(() => validationResult1.GetException(), Is.Null);
        Assert.That(() => validationResult1.FailureMessage, Is.Null);
        Assert.That(() => validationResult1.Errors.Count, Is.EqualTo(0));
        Assert.That(() => validationResult1.IsObjectValid, Is.True);
        Assert.That(() => validationResult1.EnsureSucceeded(), Throws.Nothing);
        AssertLastMemberContexts();

        ClearLastMemberContexts();
        ObjectValidator.EnsureValid(data!);
        AssertLastMemberContexts();

        ClearLastMemberContexts();
        var validationResult2 = ObjectValidator.Validate(data!, "customExpression-5d804913def74aa2b441496a9e92dba6").AssertNotNull();

        Assert.That(() => validationResult2.GetException(), Is.Null);
        Assert.That(() => validationResult2.FailureMessage, Is.Null);
        Assert.That(() => validationResult2.Errors.Count, Is.EqualTo(0));
        Assert.That(() => validationResult2.IsObjectValid, Is.True);
        Assert.That(() => validationResult2.EnsureSucceeded(), Throws.Nothing);
        AssertLastMemberContexts();

        ClearLastMemberContexts();
        ObjectValidator.EnsureValid(data!, "customExpression-5ccb64b2000d4da2be33aCca8e2e42f3");
        AssertLastMemberContexts();
    }
}