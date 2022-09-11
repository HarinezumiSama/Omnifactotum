using System.Collections.Generic;
using System.Linq;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    internal abstract class TypedConstraintTestsBase<TConstraint, T> : ConstraintTestsBase<TConstraint>
        where TConstraint : TypedMemberConstraintBase<T>, new()
    {
        protected abstract IEnumerable<T> GetTypedValidValues();

        protected abstract IEnumerable<T> GetTypedInvalidValues();

        protected sealed override IEnumerable<object?> GetValidValues() => GetTypedValidValues().Cast<object?>();

        protected sealed override IEnumerable<object?> GetInvalidValues() => GetTypedInvalidValues().Cast<object?>();
    }
}