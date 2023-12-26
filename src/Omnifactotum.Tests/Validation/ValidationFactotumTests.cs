using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

[TestFixture(TestOf = typeof(ValidationFactotum))]
[SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
[SuppressMessage("ReSharper", "ClassCanBeSealed.Local")]
internal sealed class ValidationFactotumTests
{
    private const string InvalidConstraintTypeExceptionMessageEnding =
        """
        is not a valid constraint type (must be an instantiatable class that implements "Omnifactotum.Validation.Constraints.IMemberConstraint"). (Parameter 'constraintType')
        """;

    [Test]
    [TestCase(typeof(NotNullConstraint))]
    [TestCase(typeof(NotNullConstraint<object>))]
    [TestCase(typeof(NotNullConstraint<string>))]
    [TestCase(typeof(NotNullOrEmptyCollectionConstraint))]
    [TestCase(typeof(NotNullOrEmptyCollectionConstraint<object>))]
    [TestCase(typeof(NotNullOrEmptyCollectionConstraint<string>))]
    [TestCase(typeof(NotBlankStringConstraint))]
    [TestCase(typeof(NotNullOrEmptyStringConstraint))]
    [TestCase(typeof(EnumValueDefinedConstraint<AssertEqualityExpectation>))]
    [TestCase(typeof(PrivateConstructorMemberConstraint))]
    [TestCase(typeof(PrivateParameterlessConstructorAndOtherConstructorMemberConstraint))]
    [TestCase(typeof(PrivateProtectedConstructorMemberConstraint))]
    [TestCase(typeof(ProtectedConstructorMemberConstraint))]
    [TestCase(typeof(ProtectedInternalConstructorMemberConstraint))]
    [TestCase(typeof(InternalConstructorMemberConstraint))]
    [TestCase(typeof(PublicConstructorMemberConstraint))]
    [TestCase(typeof(PublicParameterlessConstructorAndOtherConstructorsMemberConstraint))]
    public void TestValidateAndRegisterMemberConstraintTypeAndCreateMemberConstraintWhenValidArgumentThenSucceeds(Type constraintType)
    {
        Assert.That(() => constraintType.ValidateAndRegisterMemberConstraintType(), Is.SameAs(constraintType));

        // Can be called multiple times
        Assert.That(() => constraintType.ValidateAndRegisterMemberConstraintType(), Is.SameAs(constraintType));

        Assert.That(() => ValidationFactotum.CreateMemberConstraint(constraintType), Is.TypeOf(constraintType));
    }

    [Test]
    [TestCase(
        typeof(string),
        $"""
        "System.String" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    [TestCase(
        typeof(IMemberConstraint),
        $"""
        "Omnifactotum.Validation.Constraints.IMemberConstraint" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    [TestCase(
        typeof(NotNullConstraint<>),
        $"""
        "Omnifactotum.Validation.Constraints.NotNullConstraint<T>" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    [TestCase(
        typeof(ITestMemberConstraint),
        $"""
        "Omnifactotum.Tests.Validation.ValidationFactotumTests.ITestMemberConstraint" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    [TestCase(
        typeof(TestMemberConstraintBase),
        $"""
        "Omnifactotum.Tests.Validation.ValidationFactotumTests.TestMemberConstraintBase" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    [TestCase(
        typeof(NoParameterlessConstructorMemberConstraint),
        """
        The constraint type "Omnifactotum.Tests.Validation.ValidationFactotumTests.NoParameterlessConstructorMemberConstraint" must have a parameterless constructor. (Parameter 'constraintType')
        """)]
    [TestCase(
        typeof(ValueTypeMemberConstraint),
        $"""
        "Omnifactotum.Tests.Validation.ValidationFactotumTests.ValueTypeMemberConstraint" {InvalidConstraintTypeExceptionMessageEnding}
        """)]
    public void TestValidateAndRegisterMemberConstraintTypeAndCreateMemberConstraintWhenInvalidArgumentThenThrows(
        Type constraintType,
        string expectedExceptionMessage)
    {
        Assert.That(
            () => constraintType.ValidateAndRegisterMemberConstraintType(),
            Throws.ArgumentException.With.Message.EqualTo(expectedExceptionMessage));

        Assert.That(
            () => ValidationFactotum.CreateMemberConstraint(constraintType),
            Throws.ArgumentException.With.Message.EqualTo(expectedExceptionMessage));
    }

    private struct ValueTypeMemberConstraint : IMemberConstraint
    {
        void IMemberConstraint.Validate(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, object? value)
            => throw new NotSupportedException();
    }

    private interface ITestMemberConstraint : IMemberConstraint
    {
        // No own members
    }

    private abstract class TestMemberConstraintBase : IMemberConstraint
    {
        [UsedImplicitly]
        protected TestMemberConstraintBase()
        {
            // Nothing to do
        }

        void IMemberConstraint.Validate(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, object? value)
            => throw new NotSupportedException();
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private class NoParameterlessConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        private NoParameterlessConstructorMemberConstraint(byte dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        private protected NoParameterlessConstructorMemberConstraint(short dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        protected NoParameterlessConstructorMemberConstraint(int dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        internal NoParameterlessConstructorMemberConstraint(long dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        protected internal NoParameterlessConstructorMemberConstraint(float dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        public NoParameterlessConstructorMemberConstraint(double dummyValue) => throw new NotSupportedException();
    }

    private class PrivateConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        private PrivateConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private class PrivateParameterlessConstructorAndOtherConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        private PrivateParameterlessConstructorAndOtherConstructorMemberConstraint()
        {
            // Nothing to do
        }

        [UsedImplicitly]
        public PrivateParameterlessConstructorAndOtherConstructorMemberConstraint(int dummyValue) => throw new NotSupportedException();
    }

    private class PrivateProtectedConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        private protected PrivateProtectedConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    private class ProtectedConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        protected ProtectedConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    private class ProtectedInternalConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        protected internal ProtectedInternalConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    private class InternalConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        internal InternalConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    private class PublicConstructorMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        public PublicConstructorMemberConstraint()
        {
            // Nothing to do
        }
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private class PublicParameterlessConstructorAndOtherConstructorsMemberConstraint : TestMemberConstraintBase
    {
        [UsedImplicitly]
        private PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(byte dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        private protected PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(short dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        protected PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(int dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        internal PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(long dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        protected internal PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(float dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        public PublicParameterlessConstructorAndOtherConstructorsMemberConstraint(double dummyValue) => throw new NotSupportedException();

        [UsedImplicitly]
        public PublicParameterlessConstructorAndOtherConstructorsMemberConstraint()
        {
            // Nothing to do
        }
    }
}