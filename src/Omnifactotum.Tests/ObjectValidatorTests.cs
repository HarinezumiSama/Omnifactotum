﻿using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class ObjectValidatorTests
    {
        #region Tests

        [Test]
        public void TestValidate()
        {
            var data = new ComplexData { Data = new SimpleData { StartDate = DateTime.Now } };
            var validationResult = ObjectValidator.Validate(data);

            Assert.That(validationResult, Is.Not.Null);
            Assert.That(validationResult.Errors.Count, Is.EqualTo(3));

            var actualNotNullErrorExpressions = validationResult
                .Errors
                .Where(obj => obj.FailedConstraintType == typeof(NotNullConstraint))
                .Select(obj => obj.Context.Expression.ToString())
                .ToArray();

            var expectedNotNullErrorExpressions =
                new[]
                {
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.Data.Value",
                        ObjectValidator.RootObjectParameterName),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.Data.NullableValue",
                        ObjectValidator.RootObjectParameterName)
                };

            Assert.That(actualNotNullErrorExpressions, Is.EquivalentTo(expectedNotNullErrorExpressions));

            var utcDateError =
                validationResult.Errors.Single(obj => obj.FailedConstraintType == typeof(UtcDateConstraint));

            Assert.That(
                utcDateError.Context.Expression.ToString(),
                Is.EqualTo(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.Data.StartDate",
                        ObjectValidator.RootObjectParameterName)));
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