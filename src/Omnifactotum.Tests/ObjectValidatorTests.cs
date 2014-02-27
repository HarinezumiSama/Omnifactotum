using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class ObjectValidatorTests
    {
        #region Tests

        [Test]
        public void TestValidateWhenValidationSucceeded()
        {
            var data = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.UtcNow, NullableValue = 0, Value = "A" },
                EmptyValue = "B",
                MultipleDatas = new[] { new AnotherSimpleData { Value = "B" } }
            };

            var validationResult = ObjectValidator.Validate(data);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(0));
            Assert.That(validationResult.IsObjectValid, Is.True);
            Assert.That(validationResult.GetException(), Is.Null);
            Assert.That(() => validationResult.EnsureSucceeded(), Throws.Nothing);
        }

        [Test]
        public void TestValidateWhenValidationFailed()
        {
            var data = new ComplexData
            {
                Data = new SimpleData { StartDate = DateTime.Now },
                EmptyValue = string.Empty,
                MultipleDatas = new[] { new AnotherSimpleData { Value = "C" }, new AnotherSimpleData() }
            };

            var validationResult = ObjectValidator.Validate(data);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(5));
            Assert.That(validationResult.IsObjectValid, Is.False);

            var validationException = validationResult.GetException();
            Assert.That(validationException, Is.Not.Null & Is.TypeOf<ObjectValidationException>());
            Assert.That(validationException.EnsureNotNull().ValidationResult, Is.SameAs(validationResult));

            Assert.That(() => validationResult.EnsureSucceeded(), Throws.TypeOf<ObjectValidationException>());

            var actualNotNullErrorExpressions = validationResult
                .Errors
                .Where(obj => obj.FailedConstraintType == typeof(NotNullConstraint))
                .Select(obj => obj.Context.Expression.ToString())
                .ToArray();

            var expectedNotNullErrorExpressions =
                new[]
                {
                    MakeExpressionString("Data.Value"),
                    MakeExpressionString("Data.NullableValue"),
                    MakeExpressionString("MultipleDatas[1].Value")
                };

            Assert.That(actualNotNullErrorExpressions, Is.EquivalentTo(expectedNotNullErrorExpressions));

            var notEmptyError =
                validationResult.Errors.Single(
                    obj => obj.FailedConstraintType == typeof(NotNullOrEmptyStringConstraint));

            Assert.That(
                notEmptyError.Context.Expression.ToString(),
                Is.EqualTo(MakeExpressionString("EmptyValue")));

            var utcDateError =
                validationResult.Errors.Single(obj => obj.FailedConstraintType == typeof(UtcDateConstraint));

            Assert.That(
                utcDateError.Context.Expression.ToString(),
                Is.EqualTo(MakeExpressionString("Data.StartDate")));
        }

        #endregion

        #region Private Methods

        private static string MakeExpressionString(string propertyPath)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}",
                ObjectValidator.RootObjectParameterName,
                propertyPath);
        }

        #endregion

        #region SimpleData Class

        public sealed class SimpleData
        {
            #region Constants and Fields

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

        #region AnotherSimpleData Class

        public sealed class AnotherSimpleData
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
                private get;

                set;
            }

            [MemberConstraint(typeof(NotNullConstraint))]
            [MemberItemConstraint(typeof(NotNullConstraint))]
            public AnotherSimpleData[] MultipleDatas
            {
                [UsedImplicitly]
                get;

                set;
            }

            [MemberConstraint(typeof(NotNullConstraint))]
            [MemberConstraint(typeof(NotNullOrEmptyStringConstraint))]
            public string EmptyValue
            {
                [UsedImplicitly]
                private get;

                set;
            }

            #endregion
        }

        #endregion

        #region UtcDateConstraint Class

        private sealed class UtcDateConstraint : MemberConstraintBase
        {
            protected override MemberConstraintValidationError ValidateInternal(
                MemberConstraintValidationContext context,
                object value)
            {
                var dateTime = (DateTime)value;
                return dateTime.Kind == DateTimeKind.Utc ? null : CreateDefaultError(context);
            }
        }

        #endregion

        #region NeverCalledConstraint Class

        private sealed class NeverCalledConstraint : MemberConstraintBase
        {
            protected override MemberConstraintValidationError ValidateInternal(
                MemberConstraintValidationContext context,
                object value)
            {
                const string Message = "This constraint is not supposed to be called ever.";

                Assert.Fail(Message);
                throw new InvalidOperationException(Message);
            }
        }

        #endregion
    }
}