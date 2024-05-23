using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

internal abstract class TypedConstraintTestsBase<[MeansImplicitUse] TConstraint, T> : ConstraintTestsBase<TConstraint>
    where TConstraint : TypedMemberConstraintBase<T>, new()
{
    protected abstract IEnumerable<T> GetTypedValidValues();

    protected abstract IEnumerable<T> GetTypedInvalidValues();

    protected abstract ValidationErrorDetails GetTypedInvalidValueErrorDetails(T invalidValue);

    protected sealed override IEnumerable<object?> GetValidValues() => GetTypedValidValues().Cast<object?>();

    protected sealed override IEnumerable<object?> GetInvalidValues() => GetTypedInvalidValues().Cast<object?>();

    protected override ValidationErrorDetails GetInvalidValueErrorDetails(object? invalidValue)
    {
        var valueType = typeof(T);

        switch (invalidValue)
        {
            case null when valueType.IsClass || valueType.IsInterface || valueType.IsNullableValueType():
                return GetTypedInvalidValueErrorDetails(default!);

            case T castValue:
                return GetTypedInvalidValueErrorDetails(castValue);
        }

        var errorMessage = $"Unexpected value or value type: {FormatValue(invalidValue)}. Expected value type: {valueType.GetFullName().ToUIString()}.";
        Assert.Fail(errorMessage);
        throw new InvalidOperationException(errorMessage);

        string FormatValue(object? value)
        {
            var valuePart = value switch
            {
                null => OmnifactotumRepresentationConstants.NullValueRepresentation,
                string s => s.ToUIString(),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString().AvoidNull()
            };

            var typePart = value?.GetType() switch
            {
                null => string.Empty,
                var type => $"\x0020({type.GetQualifiedName()})"
            };

            return valuePart + typePart;
        }
    }
}