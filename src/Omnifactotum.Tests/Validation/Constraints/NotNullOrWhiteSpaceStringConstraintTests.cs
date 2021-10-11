using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
#pragma warning disable 618
    [TestFixture(TestOf = typeof(NotNullOrWhiteSpaceStringConstraint))]
    internal sealed class NotNullOrWhiteSpaceStringConstraintTests
        : TypedConstraintTestsBase<NotNullOrWhiteSpaceStringConstraint, string>
#pragma warning restore 618
    {
        protected override IEnumerable<string> GetTypedValidValues()
        {
            yield return "\x0020A";
            yield return "A";
        }

        protected override IEnumerable<string> GetTypedInvalidValues()
        {
            yield return null;
            yield return string.Empty;
            yield return "\t\x0020\r\n";
        }
    }
}