using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture(TestOf = typeof(NotNullConstraint))]
    internal sealed class NotNullConstraintTests : ConstraintTestsBase<NotNullConstraint>
    {
        [Ignore("Not applicable.")]
        public override void TestValidateWhenIncorrectValueTypeThenThrows() => throw new NotSupportedException();

        protected override IEnumerable<object> GetValidValues()
        {
            yield return new object();
        }

        protected override IEnumerable<object> GetInvalidValues()
        {
            yield return null;
        }
    }
}