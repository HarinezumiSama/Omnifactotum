using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

[TestFixture(TestOf = typeof(ObjectValidator))]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
[SuppressMessage("Performance", "CA1822:Mark members as static")]
internal sealed partial class ObjectValidatorTests
{
    private const string ValidationResultPropertyName = nameof(ObjectValidationException.ValidationResult);

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
            Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
            NonEmptyValue = "B",
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C1" } },
            AnotherSimpleDataArray = new[] { new AnotherSimpleData { Value = "C2" } },
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
            Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
            NonEmptyValue = "B",
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C1" } },
            AnotherSimpleDataArray = new[] { new AnotherSimpleData { Value = "C2" } },
            AnotherSimpleDataCollection = new Collection<AnotherSimpleData> { new() { Value = "C3" } },
            AnotherSimpleDataCustomEnumerable = new CustomEnumerable(new AnotherSimpleData { Value = "C4" }),
            AnotherSimpleDataCustomGenericEnumerable = new CustomGenericEnumerable<AnotherSimpleData>(new AnotherSimpleData { Value = "C5" }),
            AnotherSimpleDataCustomGenericEnumerableObject = new CustomGenericEnumerable<AnotherSimpleData>(new AnotherSimpleData { Value = "C5" }),
            AnotherSimpleDataCustomGenericList = new CustomGenericList<AnotherSimpleData>(new AnotherSimpleData { Value = "C6" }),
            AnotherSimpleDataCustomList = new CustomList(new AnotherSimpleData { Value = "C7" }),
            AnotherSimpleDataCustomReadOnlyList = new CustomReadOnlyList<AnotherSimpleData>(new AnotherSimpleData { Value = "C8" }),
            AnotherSimpleDataImmutableList = ImmutableList.Create(new AnotherSimpleData { Value = "C9" }),
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
                AnotherSimpleDataCollection = new Collection<AnotherSimpleData>
                {
                    new AnotherSimpleData { Value = "C3" },
                    new AnotherSimpleData()
                },
                AnotherSimpleDataCustomEnumerable = new CustomEnumerable(
                    new AnotherSimpleData(),
                    new AnotherSimpleData { Value = "C4" }),
                AnotherSimpleDataCustomGenericEnumerable = new CustomGenericEnumerable<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C5" },
                    new AnotherSimpleData { Value = "C6" },
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomGenericEnumerableObject = new CustomGenericEnumerable<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C5-O" },
                    new AnotherSimpleData { Value = "C6-O" },
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomGenericList = new CustomGenericList<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C7" },
                    new AnotherSimpleData(),
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomList = new CustomList(
                    new AnotherSimpleData { Value = "C8" },
                    new AnotherSimpleData(),
                    new AnotherSimpleData { Value = "C9" },
                    null,
                    new AnotherSimpleData()),
                AnotherSimpleDataCustomReadOnlyList = new CustomReadOnlyList<AnotherSimpleData>(
                    new AnotherSimpleData { Value = "C10" },
                    new AnotherSimpleData { Value = "C11" },
                    new AnotherSimpleData { Value = "C12" },
                    new AnotherSimpleData()),
                AnotherSimpleDataImmutableList = ImmutableList.Create(
                    new AnotherSimpleData { Value = "C13" },
                    new AnotherSimpleData { Value = "C14" },
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
            {
                typeof(NotNullOrEmptyStringConstraint),
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

        const string ExpectedExceptionMessage =
            $"""
            [1/22] [{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]] The value cannot be null.
            [2/22] [{InstanceExpression}.ContainedValue.AnotherNullableImmutableStrings.Value.Item[1]] The value cannot be null.
            [3/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataArray[1].Value] The value cannot be null.
            [4/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCollection.Item[1].Value] The value cannot be null.
            [5/22] [Convert({
                InstanceExpression}.ContainedValue.AnotherSimpleDataCustomEnumerable.Cast().First(), AnotherSimpleData).Value] The value cannot be null.
            [6/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericEnumerable.Skip(2).First().Value] The value cannot be null.
            [7/22] [Convert({InstanceExpression
            }.ContainedValue.AnotherSimpleDataCustomGenericEnumerableObject, IEnumerable`1).Skip(2).First().Value] The value cannot be null.
            [8/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[1].Value] The value cannot be null.
            [9/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomGenericList.Item[2].Value] The value cannot be null.
            [10/22] [Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[1], AnotherSimpleData).Value] The value cannot be null.
            [11/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[3]] The value cannot be null.
            [12/22] [Convert({InstanceExpression}.ContainedValue.AnotherSimpleDataCustomList.Item[4], AnotherSimpleData).Value] The value cannot be null.
            [13/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataCustomReadOnlyList.Item[3].Value] The value cannot be null.
            [14/22] [{InstanceExpression}.ContainedValue.AnotherSimpleDataImmutableList.Item[2].Value] The value cannot be null.
            [15/22] [{InstanceExpression}.ContainedValue.Data.NullableValue] The value cannot be null.
            [16/22] [{InstanceExpression}.ContainedValue.Data.StartDate] Validation of the constraint "{
                nameof(ObjectValidatorTests)}.UtcDateConstraint" failed.
            [17/22] [{InstanceExpression}.ContainedValue.Data.StartDate] Validation of the constraint "{
                nameof(ObjectValidatorTests)}.UtcDateTypedConstraint" failed.
            [18/22] [{InstanceExpression}.ContainedValue.Data.Value] The value cannot be null.
            [19/22] [Convert({InstanceExpression}.ContainedValue.MultipleDataItems[1], AnotherSimpleData).Value] The value cannot be null.
            [20/22] [{InstanceExpression}.ContainedValue.NonEmptyValue] The value must not be null or an empty string.
            [21/22] [{InstanceExpression}.ContainedValue.NullableImmutableStrings] The value cannot be null.
            [22/22] [Convert({InstanceExpression}.ContainedValue.SingleBaseData, AnotherSimpleData).Value] The value cannot be null.
            """;

        Assert.That(() => validationException.Message, Is.EqualTo(ExpectedExceptionMessage));
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
                    $"Convert({InstanceExpression}.Properties.Skip(2).First(), KeyValuePair`2).Value.ContainedValue",
                    $"Convert({InstanceExpression}.Properties.Skip(2).First(), KeyValuePair`2).Value.ContainedValue"
                }
            },
            {
                typeof(NotNullOrEmptyStringConstraint),
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
            }
        };

        Assert.That(validationResult, Is.Not.Null);
        Assert.That(() => validationResult.IsObjectValid, Is.False);

        var validationException = validationResult.GetException().AssertNotNull();
        Assert.That(validationException, Is.TypeOf<ObjectValidationException>());
        Assert.That(() => validationException.ValidationResult, Is.SameAs(validationResult));

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
    }

    private static void EnsureTestValidationSucceeded<T>(T data)
    {
        var validationResult1 = ObjectValidator.Validate(data!).AssertNotNull();

        Assert.That(() => validationResult1.GetException(), Is.Null);
        Assert.That(() => validationResult1.Errors.Count, Is.EqualTo(0));
        Assert.That(() => validationResult1.IsObjectValid, Is.True);
        Assert.That(() => validationResult1.EnsureSucceeded(), Throws.Nothing);

        var validationResult2 = ObjectValidator.Validate(data!, "customExpression-5d804913def74aa2b441496a9e92dba6").AssertNotNull();

        Assert.That(() => validationResult2.GetException(), Is.Null);
        Assert.That(() => validationResult2.Errors.Count, Is.EqualTo(0));
        Assert.That(() => validationResult2.IsObjectValid, Is.True);
        Assert.That(() => validationResult2.EnsureSucceeded(), Throws.Nothing);
    }

    public sealed class SimpleContainer<T>
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        public T? ContainedValue
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

    private sealed class MapContainer
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(MapContainerPropertiesPairConstraint))]
        [MemberItemConstraint(
            typeof(KeyValuePairConstraint<string, SimpleContainer<int?>?, NotAbcStringConstraint, NotNullConstraint<SimpleContainer<int?>>>))]
        public IEnumerable<KeyValuePair<string, SimpleContainer<int?>>>? Properties
        {
            [UsedImplicitly]
            get;
            set;
        }
    }
}