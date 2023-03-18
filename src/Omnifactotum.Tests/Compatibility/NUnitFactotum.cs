using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Compatibility (Omnifactotum.NUnit)
namespace Omnifactotum.NUnit;

/// <summary>
///     Provides the helper methods and properties for common use in the NUnit tests.
/// </summary>
internal static class NUnitFactotum
{
    private const string EqualityOperatorMethodName = "op_Equality";
    private const string InequalityOperatorMethodName = "op_Inequality";

    private const BindingFlags OperatorBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

    /// <summary>
    ///     Asserts the readability and writability of the specified property.
    /// </summary>
    /// <typeparam name="TObject">
    ///     The type of the object whose property to check.
    /// </typeparam>
    /// <typeparam name="TProperty">
    ///     The type of the property to check.
    /// </typeparam>
    /// <param name="propertyGetterExpression">
    ///     The lambda expression specifying the property to check,
    ///     in the following form: (SomeType x) => x.Property.
    /// </param>
    /// <param name="expectedAccessMode">
    ///     The expected readability and writability of the specified property.
    /// </param>
    /// <param name="accessorVisibilityAttributes">
    ///     The attribute from <see cref="MethodAttributes.MemberAccessMask"/> specifying the visibility of the
    ///     accessors.
    /// </param>
    public static void AssertReadableWritable<TObject, TProperty>(
        [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
        PropertyAccessMode expectedAccessMode,
        MethodAttributes accessorVisibilityAttributes = MethodAttributes.Public)
    {
        Assert.That(propertyGetterExpression, Is.Not.Null, @"The property expression cannot be null.");
        Assert.That(Enum.IsDefined(typeof(PropertyAccessMode), expectedAccessMode), Is.True, @"Invalid expected access mode.");

        Assert.That(
            (int)(accessorVisibilityAttributes & ~MethodAttributes.MemberAccessMask),
            Is.EqualTo(0),
            () => AsInvariant(
                $@"Invalid accessor visibility. Must match the ""{nameof(MethodAttributes)}{Type.Delimiter}{
                    nameof(MethodAttributes.MemberAccessMask)}"" mask."));

        var propertyInfo = Factotum.For<TObject>.GetPropertyInfo(propertyGetterExpression);
        Assert.That(propertyInfo, Is.Not.Null);

        string GetFullPropertyNameUIString()
            => AsInvariant($@"{propertyInfo.DeclaringType.EnsureNotNull().GetFullName()}{Type.Delimiter}{propertyInfo.Name}")
                .ToUIString();

        var expectedReadability = expectedAccessMode != PropertyAccessMode.WriteOnly;

        var actualReadability = propertyInfo.CanRead
            && AccessAttributesMatch(propertyInfo.GetGetMethod(true).AssertNotNull(), accessorVisibilityAttributes);

        Assert.That(
            actualReadability,
            Is.EqualTo(expectedReadability),
            () => AsInvariant(
                $@"The property {GetFullPropertyNameUIString()} MUST {
                    (expectedReadability ? string.Empty : "NOT ")}be readable (visibility: {
                        GetVisibilityName()})."));

        var expectedWritability = expectedAccessMode != PropertyAccessMode.ReadOnly;

        var actualWritability = propertyInfo.CanWrite
            && AccessAttributesMatch(propertyInfo.GetSetMethod(true).AssertNotNull(), accessorVisibilityAttributes);

        Assert.That(
            actualWritability,
            Is.EqualTo(expectedWritability),
            () => AsInvariant(
                $@"The property {GetFullPropertyNameUIString()} MUST {
                    (expectedWritability ? string.Empty : "NOT ")}be writable (visibility: {
                        Enum.GetName(accessorVisibilityAttributes.GetType(), accessorVisibilityAttributes)})."));

        string GetVisibilityName()
            => Enum.GetName(accessorVisibilityAttributes.GetType(), accessorVisibilityAttributes).EnsureNotNull();
    }

    /// <summary>
    ///     Asserts the equality of two specified values of the same types.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the values.
    /// </typeparam>
    /// <param name="value1">
    ///     The first value to compare.
    /// </param>
    /// <param name="value2">
    ///     The second value to compare.
    /// </param>
    /// <param name="equalityExpectation">
    ///     The equality expectation for the specified values.
    /// </param>
    /// <param name="operatorExpectation">
    ///     The expectation for the overloaded equality and inequality operators.
    /// </param>
    public static void AssertEquality<T>(
        T value1,
        T value2,
        AssertEqualityExpectation equalityExpectation,
        AssertEqualityOperatorExpectation operatorExpectation = AssertEqualityOperatorExpectation.MayDefine)
    {
        if (value1 is not null)
        {
            Assert.That(value1, Is.TypeOf<T>());
        }

        if (value2 is not null)
        {
            Assert.That(value2, Is.TypeOf<T>());
        }

        if (value1 is not null && value2 is not null && equalityExpectation != AssertEqualityExpectation.EqualAndMayBeSame)
        {
            Assert.That(value1, Is.Not.SameAs(value2));
        }

        var equals =
            equalityExpectation switch
            {
                AssertEqualityExpectation.NotEqual => false,
                AssertEqualityExpectation.EqualAndMayBeSame or AssertEqualityExpectation.EqualAndCannotBeSame => true,
                _ => throw equalityExpectation.CreateEnumValueNotImplementedException()
            };

        Assert.That(() => EqualityComparer<T>.Default.Equals(value1, value2), Is.EqualTo(equals));
        Assert.That(() => EqualityComparer<T>.Default.Equals(value2, value1), Is.EqualTo(equals));

        var value1AsObject = (object?)value1;
        var value2AsObject = (object?)value2;

        Assert.That(() => Equals(value1AsObject, value2AsObject), Is.EqualTo(equals));
        Assert.That(() => Equals(value2AsObject, value1AsObject), Is.EqualTo(equals));

        if (value1 is not null)
        {
            Assert.That(() => value1AsObject!.Equals(value2AsObject), Is.EqualTo(equals));
        }

        if (value2 is not null)
        {
            Assert.That(() => value2AsObject!.Equals(value1AsObject), Is.EqualTo(equals));
        }

        if (equals && value1 is not null && value2 is not null)
        {
            Assert.That(
                value2.GetHashCode,
                Is.EqualTo(value1.GetHashCode()),
                @"When the values are equal, their hash codes must also be equal.");
        }

        var type = typeof(T);

        var equalityOperatorMethod = type.GetMethod(EqualityOperatorMethodName, OperatorBindingFlags);
        var inequalityOperatorMethod = type.GetMethod(InequalityOperatorMethodName, OperatorBindingFlags);

        Func<IResolveConstraint>? createOperatorConstraint;
        string? verb;
        switch (operatorExpectation)
        {
            case AssertEqualityOperatorExpectation.MayDefine:
                createOperatorConstraint = null;
                verb = null;
                break;
            case AssertEqualityOperatorExpectation.MustNotDefine:
                createOperatorConstraint = () => Is.Null;
                verb = @"must not";
                break;
            case AssertEqualityOperatorExpectation.MustDefine:
                createOperatorConstraint = () => Is.Not.Null;
                verb = @"must";
                break;
            default:
                throw operatorExpectation.CreateEnumValueNotImplementedException();
        }

        if (createOperatorConstraint is not null)
        {
            Assert.That(verb, Is.Not.Null.And.Not.Empty);

            Assert.That(
                equalityOperatorMethod,
                createOperatorConstraint(),
                AsInvariant($@"Equality operator (==) {verb} be defined for the type {type.GetFullName().ToUIString()}."));

            Assert.That(
                inequalityOperatorMethod,
                createOperatorConstraint(),
                AsInvariant($@"Inequality operator (!=) {verb} be defined for the type {type.GetFullName().ToUIString()}."));
        }

        if (equalityOperatorMethod is not null)
        {
            Assert.That(() => equalityOperatorMethod.Invoke(null, new object?[] { value1, value2 }), Is.EqualTo(equals));
            Assert.That(() => equalityOperatorMethod.Invoke(null, new object?[] { value2, value1 }), Is.EqualTo(equals));
        }

        if (inequalityOperatorMethod is not null)
        {
            Assert.That(() => inequalityOperatorMethod.Invoke(null, new object?[] { value1, value2 }), Is.EqualTo(!equals));
            Assert.That(() => inequalityOperatorMethod.Invoke(null, new object?[] { value2, value1 }), Is.EqualTo(!equals));
        }
    }

    /// <summary>
    ///     Generates the combinatorial test cases using the specified arguments.
    /// </summary>
    /// <param name="processTestCase">
    ///     A reference to a method that may modify a generated <see cref="TestCaseData"/>.
    /// </param>
    /// <param name="arguments">
    ///     The arguments to produce the combinatorial test cases from. Each argument may be a single value or
    ///     a collection of the values.
    /// </param>
    /// <returns>
    ///     A list of the test cases generated.
    /// </returns>
    public static List<TestCaseData> GenerateCombinatorialTestCases(
        [CanBeNull] Action<TestCaseData>? processTestCase,
        [NotNull] params object?[] arguments)
    {
        Assert.That(arguments, Is.Not.Null.And.Not.Empty);

        var result = new List<TestCaseData>();

        //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
        var normalizedArguments = arguments
            .Select(item => item is string ? item.AsCollection() : (item as IEnumerable ?? item.AsCollection()))
            .Select(item => item.Cast<object>().ToArray())
            .ToArray();

        try
        {
            var indices = new int[normalizedArguments.Length];

            var lastArgumentCount = normalizedArguments.Last().Length;
            while (indices.Last() < lastArgumentCount)
            {
                var args = normalizedArguments.Select((item, index) => item[indices[index]]).ToArray();
                var testCase = new TestCaseData(args);
                processTestCase?.Invoke(testCase);

                result.Add(testCase);

                var indexIndex = 0;
                while (indexIndex < normalizedArguments.Length)
                {
                    indices[indexIndex]++;
                    if (indices[indexIndex] < normalizedArguments[indexIndex].Length)
                    {
                        break;
                    }

                    indices[indexIndex] = 0;
                    indexIndex++;
                }

                if (indexIndex >= normalizedArguments.Length)
                {
                    break;
                }
            }
        }
        finally
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            normalizedArguments.OfType<IDisposable>().DoForEach(item => item.DisposeSafely());
        }

        return result;
    }

    /// <summary>
    ///     Generates the combinatorial test cases using the specified arguments.
    /// </summary>
    /// <param name="arguments">
    ///     The arguments to produce the combinatorial test cases from. Each argument may be a single value or
    ///     a collection of the values.
    /// </param>
    /// <returns>
    ///     A list of the test cases generated.
    /// </returns>
    public static List<TestCaseData> GenerateCombinatorialTestCases([NotNull] params object?[] arguments)
        => GenerateCombinatorialTestCases(null, arguments);

    /// <summary>
    ///     Returns the specified value if is not null;
    ///     otherwise, throws <see cref="AssertionException"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The reference type of the value to check.
    /// </typeparam>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET Core 3.0+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The specified value if is not <see langword="null"/>.
    /// </returns>
    /// <exception cref="AssertionException">
    ///     <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    [DebuggerStepThrough]
    [ContractAnnotation("value:null => stop; value:notnull => notnull", true)]
    [NotNull]
    [return: System.Diagnostics.CodeAnalysis.NotNull]
    public static T AssertNotNull<T>(
        [CanBeNull] [System.Diagnostics.CodeAnalysis.NotNull] this T? value,
        [CallerArgumentExpression("value")]
        string? valueExpression = null)
        where T : class
    {
        Assert.That(value, Is.Not.Null, GetAssertNotNullFailureMessage(valueExpression));
        return value.EnsureNotNull();
    }

    /// <summary>
    ///     Returns the value which underlies the specified nullable value, if it is not <see langword="null"/>
    ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <see langword="true"/>);
    ///     otherwise, throws <see cref="AssertionException"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type which underlies the nullable type of the value to check.
    /// </typeparam>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <param name="valueExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="value"/> parameter.</para>
    ///     <para><b>NOTE</b>: Do not pass a value for this parameter as it is automatically injected by the compiler (.NET Core 3.0+ and C# 10+).</para>
    /// </param>
    /// <returns>
    ///     The value which underlies the specified nullable value, if it is not <see langword="null"/>
    ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <see langword="true"/>).
    /// </returns>
    /// <exception cref="AssertionException">
    ///     <paramref name="value"/> is <see langword="null"/>, that is, its <see cref="Nullable{T}.HasValue"/> property is
    ///     <see langword="false"/>.
    /// </exception>
    [DebuggerStepThrough]
    public static T AssertNotNull<T>(
        [CanBeNull] [System.Diagnostics.CodeAnalysis.NotNull] this T? value,
        [CallerArgumentExpression("value")]
        string? valueExpression = null)
        where T : struct
    {
        Assert.That(value.HasValue, Is.True, GetAssertNotNullFailureMessage(valueExpression));
        return value.EnsureNotNull();
    }

    /// <summary>
    ///     Returns a new instance of <see cref="AssertCastHelper{TSource}"/> to invoke its
    ///     <see cref="AssertCastHelper{TSource}.To{TDestination}"/> method.
    /// </summary>
    /// <param name="source">
    ///     The source value.
    /// </param>
    /// <typeparam name="TSource">
    ///     The type of the source value.
    /// </typeparam>
    /// <returns>
    ///     A new instance of <see cref="AssertCastHelper{TSource}"/>.
    /// </returns>
    /// <example>
    ///     <code>
    ///         ICustomer customer = testObject.GetCustomer();
    ///         var castCustomer = customer.AssertCast().To&lt;Customer&gt;();
    ///     </code>
    /// </example>
    public static AssertCastHelper<TSource> AssertCast<TSource>(this TSource source) => new(source);

    /// <summary>
    ///     Determines if the access attribute of the specified method matches the specified attribute.
    /// </summary>
    /// <param name="method">
    ///     The method whose access attribute to check.
    /// </param>
    /// <param name="expectedAttribute">
    ///     The expected access attribute value.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the access attribute of the specified method matches the specified attribute;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    private static bool AccessAttributesMatch([NotNull] MethodInfo method, MethodAttributes expectedAttribute)
    {
        Assert.That(method, Is.Not.Null);

        const MethodAttributes Mask = MethodAttributes.MemberAccessMask;

        var result = (method.Attributes & Mask) == (expectedAttribute & Mask);
        return result;
    }

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static string? GetAssertNotNullFailureMessage(string? valueExpression = null)
        => valueExpression is null ? null : AsInvariant($@"The following expression is null: {{ {valueExpression} }}.");

    /// <summary>
    ///     Provides a convenient access to helper methods for the specified type.
    /// </summary>
    /// <typeparam name="TObject">
    ///     The type that the helper methods are provided for.
    /// </typeparam>
    public static class For<TObject>
    {
        /// <summary>
        ///     Asserts the readability and writability of the specified property.
        /// </summary>
        /// <typeparam name="TProperty">
        ///     The type of the property to check.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression specifying the property to check,
        ///     in the following form: (SomeType x) => x.Property.
        /// </param>
        /// <param name="expectedAccessMode">
        ///     The expected readability and writability of the specified property.
        /// </param>
        /// <param name="visibleAccessorAttribute">
        ///     The attribute from <see cref="MethodAttributes.MemberAccessMask"/> specifying the visibility of the
        ///     accessors.
        /// </param>
        public static void AssertReadableWritable<TProperty>(
            [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
            PropertyAccessMode expectedAccessMode,
            MethodAttributes visibleAccessorAttribute = MethodAttributes.Public)
            => AssertReadableWritable<TObject, TProperty>(
                propertyGetterExpression,
                expectedAccessMode,
                visibleAccessorAttribute);
    }

    /// <summary>
    ///     Helper class for the <see cref="NUnitFactotum.AssertCast{TSource}"/> method.
    /// </summary>
    /// <typeparam name="TSource">
    ///     The type of the source value.
    /// </typeparam>
    public readonly struct AssertCastHelper<TSource>
    {
        private readonly TSource _source;

        internal AssertCastHelper(TSource source)
        {
            Assert.That(source, Is.Not.Null);
            _source = source;
        }

        /// <summary>
        ///     Asserts the type of the source value to be compatible with the destination type
        ///     and returns the cast value.
        /// </summary>
        /// <typeparam name="TDestination">
        ///     The destination type to cast to.
        /// </typeparam>
        /// <returns>
        ///     The source value cast to the destination type.
        /// </returns>
        public TDestination To<TDestination>()
            where TDestination : class, TSource
        {
            Assert.That(_source, Is.InstanceOf<TDestination>());
            return ((TDestination?)_source!).EnsureNotNull();
        }
    }
}