using System;
using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests.Internal
{
    internal sealed class SimpleTestCase<TInput>
    {
        internal SimpleTestCase(TInput input, [NotNull] IResolveConstraint constraint)
        {
            Input = input;
            Constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        public TInput Input { get; }

        [NotNull]
        public IResolveConstraint Constraint { get; }

        public override string ToString() => AsInvariant($@"{{ {nameof(Input)} = {Input}, {nameof(Constraint)} = {Constraint} }}");
    }
}