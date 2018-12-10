using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace

namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Provides helper methods and properties for common use in the NUnit tests.
    /// </summary>
    internal static class NUnitFactotum
    {
        private const string EqualityOperatorMethodName = "op_Equality";
        private const string InequalityOperatorMethodName = "op_Inequality";

        private const BindingFlags OperatorBindingFlags =
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

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
        /// <param name="visibleAccessorAttribute">
        ///     The attribute from <see cref="MethodAttributes.MemberAccessMask"/> specifying the visibility of the
        ///     accessors.
        /// </param>
        public static void AssertReadableWritable<TObject, TProperty>(
            [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
            PropertyAccessMode expectedAccessMode,
            MethodAttributes visibleAccessorAttribute = MethodAttributes.Public)
        {
            Assert.That(propertyGetterExpression, Is.Not.Null);
            Assert.That(Enum.IsDefined(typeof(PropertyAccessMode), expectedAccessMode), Is.True);
            Assert.That(
                (int)(visibleAccessorAttribute & ~MethodAttributes.MemberAccessMask),
                Is.EqualTo(0),
                "Invalid accessor visibility. Must match '{0}' mask.",
                MethodAttributes.MemberAccessMask.GetQualifiedName());

            var propertyInfo = Factotum.For<TObject>.GetPropertyInfo(propertyGetterExpression);
            Assert.That(propertyInfo, Is.Not.Null);

            var expectedReadability = expectedAccessMode != PropertyAccessMode.WriteOnly;
            var actualReadability = propertyInfo.CanRead
                && AccessAttributesMatch(propertyInfo.GetGetMethod(true), visibleAccessorAttribute);
            Assert.That(
                actualReadability,
                Is.EqualTo(expectedReadability),
                "The property '{0}{1}{2}' MUST {3}be readable.",
                propertyInfo.DeclaringType.EnsureNotNull().GetFullName(),
                Type.Delimiter,
                propertyInfo.Name,
                expectedReadability ? string.Empty : "NOT ");

            var expectedWritability = expectedAccessMode != PropertyAccessMode.ReadOnly;
            var actualWritability = propertyInfo.CanWrite
                && AccessAttributesMatch(propertyInfo.GetSetMethod(true), visibleAccessorAttribute);
            Assert.That(
                actualWritability,
                Is.EqualTo(expectedWritability),
                "The property '{0}{1}{2}' MUST {3}be writable.",
                propertyInfo.DeclaringType.EnsureNotNull().GetFullName(),
                Type.Delimiter,
                propertyInfo.Name,
                expectedWritability ? string.Empty : "NOT ");
        }

        /// <summary>
        ///     Asserts the equality of two specified values as well as ensures that they are not the same reference.
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
        public static void AssertEquality<T>(T value1, T value2, AssertEqualityExpectation equalityExpectation)
        {
            if (!ReferenceEquals(value1, null))
            {
                Assert.That(value1, Is.TypeOf<T>());
            }

            if (!ReferenceEquals(value2, null))
            {
                Assert.That(value2, Is.TypeOf<T>());
            }

            if (!ReferenceEquals(value1, null) && !ReferenceEquals(value2, null)
                && equalityExpectation != AssertEqualityExpectation.EqualAndMayBeSame)
            {
                Assert.That(value1, Is.Not.SameAs(value2));
            }

            var equals = equalityExpectation == AssertEqualityExpectation.EqualAndMayBeSame
                || equalityExpectation == AssertEqualityExpectation.EqualAndCannotBeSame;

            Assert.That(EqualityComparer<T>.Default.Equals(value1, value2), Is.EqualTo(equals));
            Assert.That(EqualityComparer<T>.Default.Equals(value2, value1), Is.EqualTo(equals));

            var value1AsObject = (object)value1;
            var value2AsObject = (object)value2;

            Assert.That(Equals(value1AsObject, value2AsObject), Is.EqualTo(equals));
            Assert.That(Equals(value2AsObject, value1AsObject), Is.EqualTo(equals));

            if (!ReferenceEquals(value1, null))
            {
                Assert.That(value1AsObject.Equals(value2AsObject), Is.EqualTo(equals));
            }

            if (!ReferenceEquals(value2, null))
            {
                Assert.That(value2AsObject.Equals(value1AsObject), Is.EqualTo(equals));
            }

            if (equals && !ReferenceEquals(value1, null) && !ReferenceEquals(value2, null))
            {
                Assert.That(value1.GetHashCode(), Is.EqualTo(value2.GetHashCode()));
            }

            var equalityOperatorMethod = typeof(T).GetMethod(EqualityOperatorMethodName, OperatorBindingFlags);

            Assert.That(
                equalityOperatorMethod,
                Is.Not.Null,
                $@"Equality operator (==) must be defined for the type {typeof(T).GetFullName()}.");

            Assert.That(equalityOperatorMethod.Invoke(null, new object[] { value1, value2 }), Is.EqualTo(equals));
            Assert.That(equalityOperatorMethod.Invoke(null, new object[] { value2, value1 }), Is.EqualTo(equals));

            var inequalityOperatorMethod = typeof(T).GetMethod(InequalityOperatorMethodName, OperatorBindingFlags);

            Assert.That(
                inequalityOperatorMethod,
                Is.Not.Null,
                $@"Inequality operator (!=) must be defined for the 1type {typeof(T).GetFullName()}.");

            Assert.That(inequalityOperatorMethod.Invoke(null, new object[] { value1, value2 }), Is.EqualTo(!equals));
            Assert.That(inequalityOperatorMethod.Invoke(null, new object[] { value2, value1 }), Is.EqualTo(!equals));
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
            [CanBeNull] Action<TestCaseData> processTestCase,
            [NotNull] params object[] arguments)
        {
            Assert.That(arguments, Is.Not.Null);
            CollectionAssert.IsNotEmpty(arguments);

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
        public static List<TestCaseData> GenerateCombinatorialTestCases([NotNull] params object[] arguments)
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
        /// <returns>
        ///     The specified value if is not <c>null</c>.
        /// </returns>
        /// <exception cref="AssertionException">
        ///     <paramref name="value"/> is <c>null</c>.
        /// </exception>
        [NotNull]
        public static T AssertNotNull<T>([CanBeNull] this T value)
            where T : class
        {
            Assert.That(value, Is.Not.Null);
            return value.EnsureNotNull();
        }

        /// <summary>
        ///     Returns the value which underlies the specified nullable value, if it is not <c>null</c>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <c>true</c>);
        ///     otherwise, throws <see cref="AssertionException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type which underlies the nullable type of the value to check.
        /// </typeparam>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     The value which underlies the specified nullable value, if it is not <c>null</c>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <c>true</c>).
        /// </returns>
        /// <exception cref="AssertionException">
        ///     <paramref name="value"/> is <c>null</c>, that is, its <see cref="Nullable{T}.HasValue"/> property is
        ///     <c>false</c>.
        /// </exception>
        public static T AssertNotNull<T>([CanBeNull] this T? value)
            where T : struct
        {
            Assert.That(value.HasValue, Is.True, "The specified nullable value is unexpectedly null.");
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
        public static AssertCastHelper<TSource> AssertCast<TSource>(this TSource source)
            => new AssertCastHelper<TSource>(source);

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
        ///     <c>true</c> if the access attribute of the specified method matches the specified attribute;
        ///     otherwise, <c>false</c>.
        /// </returns>
        private static bool AccessAttributesMatch([NotNull] MethodInfo method, MethodAttributes expectedAttribute)
        {
            Assert.That(method, Is.Not.Null);

            const MethodAttributes Mask = MethodAttributes.MemberAccessMask;

            var result = (method.Attributes & Mask) == (expectedAttribute & Mask);
            return result;
        }

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
        public struct AssertCastHelper<TSource>
        {
            private readonly TSource _source;

            internal AssertCastHelper(TSource source)
            {
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
                where TDestination : TSource
            {
                Assert.That(_source, /*Is.Not.Null &*/ Is.InstanceOf<TDestination>());
                return (TDestination)_source;
            }
        }
    }
}