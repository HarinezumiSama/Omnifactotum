using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests.Internal;

internal static class SimpleTestCase
{
    [NotNull]
    public static SimpleTestCase<TInput> Create<TInput>(TInput input, [NotNull] IResolveConstraint constraint) => new(input, constraint);
}