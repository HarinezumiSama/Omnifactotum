using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if NET7_0_OR_GREATER
using System.Globalization;
using System.Numerics;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Represents the internal helper for the <see cref="ObjectValidator"/>.
/// </summary>
internal static class ValidationFactotum
{
    private static readonly Dictionary<Type, Func<IMemberConstraint>> MemberConstraintFactoryMap = new();

    private static readonly MethodInfo IsDefaultImmutableArrayMethodDefinition =
        ((Expression<Func<ImmutableArray<object>, bool>>)(obj => IsDefaultImmutableArray(obj)))
        .GetLastMethod()
        .EnsureNotNull()
        .GetGenericMethodDefinition();

    /// <summary>
    ///     Converts the type of the expression, if needed.
    /// </summary>
    /// <param name="expression">
    ///     The expression to convert.
    /// </param>
    /// <param name="valueType">
    ///     The type of the value.
    /// </param>
    /// <returns>
    ///     An original expression, if conversion was not needed; otherwise, a converted expression.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static Expression ConvertTypeAuto([NotNull] Expression expression, [NotNull] Type valueType)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        if (valueType is null)
        {
            throw new ArgumentNullException(nameof(valueType));
        }

        var expressionType = expression.Type.GetCollectionElementTypeOrDefault() ?? expression.Type;

        return expressionType == valueType ? expression : Expression.Convert(expression, valueType);
    }

    /// <summary>
    ///     Converts the type of the expression, if needed.
    /// </summary>
    /// <param name="expression">
    ///     The expression to convert.
    /// </param>
    /// <param name="value">
    ///     The value to which type to convert the expression.
    /// </param>
    /// <returns>
    ///     An original expression, if conversion was not needed; otherwise, a converted expression.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static Expression ConvertTypeAuto([NotNull] Expression expression, [CanBeNull] object? value)
        => value is null ? expression.EnsureNotNull() : ConvertTypeAuto(expression, value.GetType());

    /// <summary>
    ///     Ensures that the specified constraint type is a valid member constraint type.
    /// </summary>
    /// <param name="constraintType">
    ///     The type of the constraint to check.
    /// </param>
    /// <returns>
    ///     The input constraint type, if it is a valid member constraint type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="constraintType"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     The specified constraint type is not a valid member constraint type.
    /// </exception>
    [NotNull]
    public static Type ValidateAndRegisterMemberConstraintType([NotNull] this Type constraintType)
    {
        if (constraintType is null)
        {
            throw new ArgumentNullException(nameof(constraintType));
        }

        lock (MemberConstraintFactoryMap)
        {
            if (MemberConstraintFactoryMap.ContainsKey(constraintType))
            {
                return constraintType;
            }

            var compatibleType = typeof(IMemberConstraint);

            if (!constraintType.IsClass || constraintType.IsAbstract || constraintType.IsGenericTypeDefinition
                || !compatibleType.IsAssignableFrom(constraintType))
            {
                throw new ArgumentException(
                    $"{constraintType.GetFullName().ToUIString()} is not a valid constraint type (must be an instantiatable class that implements {
                        compatibleType.GetFullName().ToUIString()}).",
                    nameof(constraintType));
            }

            var constructorInfo = constraintType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                Type.DefaultBinder,
                Type.EmptyTypes,
                Array.Empty<ParameterModifier>());

            if (constructorInfo is null)
            {
                throw new ArgumentException(
                    $"The constraint type {constraintType.GetFullName().ToUIString()} must have a parameterless constructor.",
                    nameof(constraintType));
            }

            var createInstanceExpression =
                (Expression<Func<IMemberConstraint>>)Expression.Lambda(
                    Expression.Convert(Expression.New(constructorInfo), typeof(IMemberConstraint)));

            var createInstance = createInstanceExpression.Compile();
            MemberConstraintFactoryMap.Add(constraintType, createInstance);
        }

        return constraintType;
    }

    public static IMemberConstraint CreateMemberConstraint([NotNull] Type constraintType)
    {
        constraintType.ValidateAndRegisterMemberConstraintType();

        lock (MemberConstraintFactoryMap)
        {
            var createInstance = MemberConstraintFactoryMap[constraintType];
            return createInstance();
        }
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsDefaultImmutableArray([NotNull] object value)
    {
        var type = value.GetType();

        var isImmutableArray = type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(ImmutableArray<>);
        if (!isImmutableArray)
        {
            return false;
        }

        var elementType = type.GetGenericArguments().Single();
        var method = IsDefaultImmutableArrayMethodDefinition.MakeGenericMethod(elementType);

        var result = (bool)method.Invoke(null, new[] { value }).EnsureNotNull();
        return result;
    }

    public static string? TryFormatSimpleValue<TValue>(TValue value)
    {
        return value switch
        {
            null => OmnifactotumRepresentationConstants.NullValueRepresentation,
            string s => s.ToUIString(),
            Uri uri => uri.ToUIString(),
            DateTime dt => dt.ToPreciseFixedString(),
            DateTimeOffset dto => dto.ToPreciseFixedString(),
            TimeSpan ts => ts.ToPreciseFixedString(),
            IReadOnlyCollection<string?> strings => $"[{strings.ToUIString()}]",
            ICollection<string?> strings => $"[{strings.ToUIString()}]",
            _ when value.GetType() is { IsEnum: true } enumType && !enumType.IsDefined(typeof(FlagsAttribute), false)
                => Enum.IsDefined(enumType, value)
                    ? AsInvariant($"{value:D} ({value:G})")
                    : AsInvariant($"{value:D}"),
#if NET7_0_OR_GREATER
            IFormattable formattable
                when value.GetType() is { IsValueType: true } valueType
                && valueType.GetInterfaces()
                    .Any(
                        type => type.IsConstructedGenericType
                            && type.GetGenericTypeDefinition() == typeof(INumberBase<>)
                            && type.GetGenericArguments().Single() == valueType)
                => formattable.ToString(null, CultureInfo.InvariantCulture),
#endif
            _ => null
        };
    }

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    private static bool IsDefaultImmutableArray<T>(ImmutableArray<T> array) => array.IsDefault;
}