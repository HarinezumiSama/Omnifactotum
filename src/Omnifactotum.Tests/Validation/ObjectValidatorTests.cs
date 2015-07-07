using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation
{
    [TestFixture]
    public sealed class ObjectValidatorTests
    {
        #region Constants and Fields

        private static readonly string ValidationResultPropertyName =
            Factotum.For<ObjectValidationException>.GetPropertyName(obj => obj.ValidationResult);

        #endregion

        #region Tests

        [Test]
        public void TestValidateWhenValidationSucceeded()
        {
            var data1 = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
                NonEmptyValue = "B",
                MultipleDatas = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "B" } },
                SingleBaseData = new AnotherSimpleData { Value = "Q" }
            };

            EnsureTestValidationSucceeded(data1);
            EnsureTestValidationSucceeded(data1.AsArray());

            var data2 = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
                NonEmptyValue = "B",
                MultipleDatas = new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "B" } }
            };

            EnsureTestValidationSucceeded(data2);
            EnsureTestValidationSucceeded(data2.AsArray());
        }

        [Test]
        public void TestValidateWhenValidationFailed()
        {
            var data = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.Now },
                NonEmptyValue = string.Empty,
                MultipleDatas =
                    new BaseAnotherSimpleData[] { new AnotherSimpleData { Value = "C" }, new AnotherSimpleData() },
                SingleBaseData = new AnotherSimpleData()
            };

            var validationResult = ObjectValidator.Validate(data);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(6));
            Assert.That(validationResult.IsObjectValid, Is.False);

            var validationException = validationResult.GetException();
            Assert.That(validationException, Is.Not.Null & Is.TypeOf<ObjectValidationException>());
            Assert.That(validationException.EnsureNotNull().ValidationResult, Is.SameAs(validationResult));

            Assert.That(
                validationResult.EnsureSucceeded,
                Throws
                    .TypeOf<ObjectValidationException>()
                    .With
                    .Property(ValidationResultPropertyName)
                    .SameAs(validationResult));

            var actualNotNullErrorExpressions = validationResult
                .Errors
                .Where(obj => obj.FailedConstraintType == typeof(NotNullConstraint))
                .Select(obj => obj.Context.Expression.ToString())
                .ToArray();

            var expectedNotNullErrorExpressions =
                new[]
                {
                    MakeExpressionString("{0}.Data.Value"),
                    MakeExpressionString("{0}.Data.NullableValue"),
                    MakeExpressionString("Convert({0}.MultipleDatas[1]).Value"),
                    MakeExpressionString("Convert({0}.SingleBaseData).Value")
                };

            Assert.That(actualNotNullErrorExpressions, Is.EquivalentTo(expectedNotNullErrorExpressions));

            var notEmptyError =
                validationResult.Errors.Single(
                    obj => obj.FailedConstraintType == typeof(NotNullOrEmptyStringConstraint));

            Assert.That(
                notEmptyError.Context.Expression.ToString(),
                Is.EqualTo(MakeExpressionString("{0}.NonEmptyValue")));

            var utcDateError =
                validationResult.Errors.Single(obj => obj.FailedConstraintType == typeof(UtcDateConstraint));

            Assert.That(
                utcDateError.Context.Expression.ToString(),
                Is.EqualTo(MakeExpressionString("{0}.Data.StartDate")));

            var lambdaExpression = utcDateError.Context.CreateLambdaExpression();
            Assert.That(lambdaExpression, Is.Not.Null);
            Assert.That(() => lambdaExpression.Compile().Invoke(data), Is.EqualTo(data.Data.StartDate));
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
                    { "x", new SimpleContainer<int?> { ContainedValue = null } },
                }
            };

            var validationResult = ObjectValidator.Validate(mapContainer);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(2));
            Assert.That(validationResult.IsObjectValid, Is.False);

            var validationException = validationResult.GetException();
            Assert.That(validationException, Is.Not.Null & Is.TypeOf<ObjectValidationException>());
            Assert.That(validationException.EnsureNotNull().ValidationResult, Is.SameAs(validationResult));

            var notNullOrEmptyError =
                validationResult.Errors.Single(
                    obj => obj.FailedConstraintType == typeof(NotNullOrEmptyStringConstraint));

            Assert.That(
                notNullOrEmptyError.Context.Expression.ToString(),
                Is.EqualTo(MakeExpressionString("Convert(Convert({0}.Properties).Cast().First()).Key")));

            var emptyKey =
                (string)notNullOrEmptyError.Context.CreateLambdaExpression("key").Compile().Invoke(mapContainer);

            Assert.That(emptyKey, Is.EqualTo(string.Empty));

            var notNullError =
                validationResult.Errors.Single(obj => obj.FailedConstraintType == typeof(NotNullConstraint));

            Assert.That(
                notNullError.Context.Expression.ToString(),
                Is.EqualTo(
                    MakeExpressionString(
                        "Convert(Convert({0}.Properties).Cast().Skip(2).First()).Value.ContainedValue")));

            var nullContainedValue =
                (int?)notNullError.Context.CreateLambdaExpression("value").Compile().Invoke(mapContainer);

            Assert.That(nullContainedValue.HasValue, Is.False);
        }

        #endregion

        #region Private Methods

        private static string MakeExpressionString(string propertyPathTemplate)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                propertyPathTemplate,
                ObjectValidator.RootObjectParameterName);
        }

        private static void EnsureTestValidationSucceeded<T>(T data)
        {
            var validationResult = ObjectValidator.Validate(data);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
            Assert.That(validationResult.IsObjectValid, Is.True);
            Assert.That(validationResult.GetException(), Is.Null);
            Assert.That(validationResult.EnsureSucceeded, Throws.Nothing);
        }

        #endregion

        #region SimpleData Class

        public sealed class SimpleData
        {
            #region Constants and Fields

            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate",
                Justification = "OK in the unit test.")]
            [MemberConstraint(typeof(UtcDateConstraint))]
            public DateTime StartDate;

            #endregion

            #region Public Properties

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
            public string Value
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
            public string this[int index]
            {
                get
                {
                    return null;
                }
            }

            #endregion
        }

        #endregion

        #region BaseAnotherSimpleData Class

        public abstract class BaseAnotherSimpleData
        {
            //// No members
        }

        #endregion

        #region AnotherSimpleData Class

        public sealed class AnotherSimpleData : BaseAnotherSimpleData
        {
            #region Public Properties

            [MemberConstraint(typeof(NotNullConstraint))]
            public string Value
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region ComplexData Class

        public sealed class ComplexData
        {
            #region Public Properties

            [UsedImplicitly]
            public string Value
            {
                get;
                set;
            }

            [MemberConstraint(typeof(NotNullConstraint))]
            public SimpleData Data
            {
                [UsedImplicitly]
                internal get;

                set;
            }

            [MemberConstraint(typeof(NotNullConstraint))]
            [MemberItemConstraint(typeof(NotNullConstraint))]
            public BaseAnotherSimpleData[] MultipleDatas
            {
                [UsedImplicitly]
                get;

                set;
            }

            [ValidatableMember]
            public BaseAnotherSimpleData SingleBaseData
            {
                [UsedImplicitly]
                get;

                set;
            }

            [MemberConstraint(typeof(NotNullConstraint))]
            [MemberConstraint(typeof(NotNullOrEmptyStringConstraint))]
            public string NonEmptyValue
            {
                [UsedImplicitly]
                private get;

                set;
            }

            #endregion
        }

        #endregion

        #region SimpleContainer Class

        public sealed class SimpleContainer<T>
        {
            #region Public Properties

            [MemberConstraint(typeof(NotNullConstraint))]
            public T ContainedValue
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region MapContainerPropertiesPairConstraint Class

        public sealed class MapContainerPropertiesPairConstraint
            : KeyValuePairConstraintBase<string, SimpleContainer<int?>>
        {
            #region Constructors

            public MapContainerPropertiesPairConstraint()
                : base(typeof(NotNullOrEmptyStringConstraint), typeof(NotNullConstraint<SimpleContainer<int?>>))
            {
                // Nothing to do
            }

            #endregion
        }

        #endregion

        #region UtcDateConstraint Class

        private sealed class UtcDateConstraint : MemberConstraintBase
        {
            protected override void ValidateValue(
                ObjectValidatorContext validatorContext,
                MemberConstraintValidationContext context,
                object value)
            {
                var dateTime = (DateTime)value.AssertNotNull();
                if (dateTime.Kind == DateTimeKind.Utc)
                {
                    return;
                }

                AddDefaultError(validatorContext, context);
            }
        }

        #endregion

        #region NeverCalledConstraint Class

        private sealed class NeverCalledConstraint : MemberConstraintBase
        {
            protected override void ValidateValue(
                ObjectValidatorContext validatorContext,
                MemberConstraintValidationContext context,
                object value)
            {
                const string Message = "This constraint is not supposed to be called ever.";

                Assert.Fail(Message);
                throw new InvalidOperationException(Message);
            }
        }

        #endregion

        #region MapContainer Class

        private sealed class MapContainer
        {
            #region Public Properties

            [MemberConstraint(typeof(NotNullConstraint))]
            [MemberItemConstraint(typeof(MapContainerPropertiesPairConstraint))]
            public IEnumerable<KeyValuePair<string, SimpleContainer<int?>>> Properties
            {
                [UsedImplicitly]
                get;

                set;
            }

            #endregion
        }

        #endregion
    }
}