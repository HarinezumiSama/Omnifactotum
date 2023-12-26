using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

[TestFixture(TestOf = typeof(ObjectValidator))]
internal sealed class ObjectValidatorTests
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
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C" } },
            ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
            ImmutableStrings = ImmutableArray.Create("E"),
            NullableImmutableStrings = ImmutableArray<string>.Empty,
            SingleBaseData = new AnotherSimpleData { Value = "Q" }
        };

        EnsureTestValidationSucceeded(data1);
        EnsureTestValidationSucceeded(data1.AsArray());
        EnsureTestValidationSucceeded(ImmutableArray.Create(data1));

        var data2 = new ComplexData
        {
            Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
            NonEmptyValue = "B",
            MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C" } },
            ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
            ImmutableStrings = ImmutableArray<string>.Empty,
            NullableImmutableStrings = ImmutableArray.Create("F")
        };

        EnsureTestValidationSucceeded(data2);
        EnsureTestValidationSucceeded(data2.AsArray());
        EnsureTestValidationSucceeded(ImmutableArray.Create(data2));
    }

    [Test]
    public void TestValidateWhenValidationFailed()
    {
        var dataContainer = new SimpleContainer<ComplexData>
        {
            ContainedValue = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.Now },
                NonEmptyValue = string.Empty,
                MultipleDataItems = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C" }, new AnotherSimpleData() },
                ImmutableMultipleDataItems = ImmutableArray.Create<BaseAnotherSimpleData>(new AnotherSimpleData { Value = "D" }),
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
                    $"Convert({InstanceExpression}.ContainedValue.MultipleDataItems[1], AnotherSimpleData).Value",
                    $"Convert({InstanceExpression}.ContainedValue.SingleBaseData, AnotherSimpleData).Value"
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

        const string ExpectedExceptionMessage = $"""
            [1/8] [{InstanceExpression}.ContainedValue.Data.StartDate] Validation of the constraint "{nameof(ObjectValidatorTests)}.UtcDateConstraint" failed.
            [2/8] [{InstanceExpression}.ContainedValue.Data.StartDate] Validation of the constraint "{
                nameof(ObjectValidatorTests)}.UtcDateTypedConstraint" failed.
            [3/8] [{InstanceExpression}.ContainedValue.Data.Value] The value cannot be null.
            [4/8] [{InstanceExpression}.ContainedValue.Data.NullableValue] The value cannot be null.
            [5/8] [Convert({InstanceExpression}.ContainedValue.MultipleDataItems[1], AnotherSimpleData).Value] The value cannot be null.
            [6/8] [{InstanceExpression}.ContainedValue.NullableImmutableStrings] The value cannot be null.
            [7/8] [Convert({InstanceExpression}.ContainedValue.SingleBaseData, AnotherSimpleData).Value] The value cannot be null.
            [8/8] [{InstanceExpression}.ContainedValue.NonEmptyValue] The value must not be null or an empty string.
            """;

        Assert.That(() => validationException.Message, Is.EqualTo(ExpectedExceptionMessage));

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
    public void TestDictionaryValidation()
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
                    $"Convert(Convert({InstanceExpression}.Properties, IEnumerable).Cast().Skip(2).First(), KeyValuePair`2).Value.ContainedValue",
                    $"Convert(Convert({InstanceExpression}.Properties, IEnumerable).Cast().Skip(2).First(), KeyValuePair`2).Value.ContainedValue"
                }
            },
            {
                typeof(NotNullOrEmptyStringConstraint),
                new[]
                {
                    $"Convert(Convert({InstanceExpression}.Properties, IEnumerable).Cast().First(), KeyValuePair`2).Key"
                }
            },
            {
                typeof(NotAbcStringConstraint),
                new[]
                {
                    $"Convert(Convert({InstanceExpression}.Properties, IEnumerable).Cast().Skip(1).First(), KeyValuePair`2).Key"
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

    public sealed class SimpleData
    {
        [SuppressMessage(
            "StyleCop.CSharp.MaintainabilityRules",
            "SA1401:FieldsMustBePrivate",
            Justification = "OK in the unit test.")]
        [MemberConstraint(typeof(UtcDateConstraint))]
        [MemberConstraint(typeof(UtcDateTypedConstraint))]
        public DateTime StartDate;

        [UsedImplicitly]
        [MemberConstraint(typeof(NeverCalledConstraint))]
        public int WriteOnlyValue
        {
            // ReSharper disable once ValueParameterNotUsed
            set
            {
                // Nothing to do
            }
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        public string? Value
        {
            [UsedImplicitly]
            private get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        public int? NullableValue
        {
            [UsedImplicitly]
            private get;
            set;
        }

        [MemberConstraint(typeof(NeverCalledConstraint))]
        [UsedImplicitly]
        public string this[int index] => throw new NotSupportedException();
    }

    public abstract class BaseAnotherSimpleData
    {
        //// No members
    }

    public sealed class AnotherSimpleData : BaseAnotherSimpleData
    {
        [MemberConstraint(typeof(NotNullConstraint))]
        public string? Value
        {
            [UsedImplicitly]
            get;
            set;
        }
    }

    public sealed class ComplexData
    {
        [UsedImplicitly]
        public string? Value { get; set; }

        [MemberConstraint(typeof(NotNullConstraint))]
        public SimpleData? Data
        {
            [UsedImplicitly]
            internal get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public BaseAnotherSimpleData[]? MultipleDataItems
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullOrEmptyCollectionConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        public ImmutableArray<BaseAnotherSimpleData> ImmutableMultipleDataItems
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberItemConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint<string>))]
        public ImmutableArray<string> ImmutableStrings
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint))]
        [MemberItemConstraint(typeof(NotNullConstraint<string>))]
        public ImmutableArray<string>? NullableImmutableStrings
        {
            [UsedImplicitly]
            get;
            set;
        }

        [ValidatableMember]
        public BaseAnotherSimpleData? SingleBaseData
        {
            [UsedImplicitly]
            get;
            set;
        }

        [MemberConstraint(typeof(NotNullConstraint))]
        [MemberConstraint(typeof(NotNullOrEmptyStringConstraint))]
        public string? NonEmptyValue
        {
            [UsedImplicitly]
            private get;
            set;
        }
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

    private sealed class NotAbcStringConstraint : TypedMemberConstraintBase<string>
    {
        protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, string value)
        {
            if (value == "abc")
            {
                AddDefaultError(validatorContext, memberContext);
            }
        }
    }

    private sealed class MapContainerPropertiesPairConstraint : KeyValuePairConstraintBase<string, SimpleContainer<int?>>
    {
        public MapContainerPropertiesPairConstraint()
            : base(typeof(NotNullOrEmptyStringConstraint), typeof(NotNullConstraint<SimpleContainer<int?>>))
        {
            // Nothing to do
        }
    }

    private sealed class UtcDateConstraint : MemberConstraintBase
    {
        protected override void ValidateValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            object? value)
        {
            var dateTime = (DateTime)value.AssertNotNull();
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return;
            }

            AddDefaultError(validatorContext, memberContext);
        }
    }

    private sealed class UtcDateTypedConstraint : TypedMemberConstraintBase<DateTime>
    {
        /// <inheritdoc />
        protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                AddDefaultError(validatorContext, memberContext);
            }
        }
    }

    private sealed class NeverCalledConstraint : MemberConstraintBase
    {
        protected override void ValidateValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            object? value)
        {
            const string Message = "This constraint is not supposed to be called ever.";

            Assert.Fail(Message);
            throw new InvalidOperationException(Message);
        }
    }
}