using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests.Internal
{
    internal static class SimpleTestCase
    {
        public static SimpleTestCase<TInput> Create<TInput>(TInput input, IResolveConstraint constraint) => new(input, constraint);
    }
}