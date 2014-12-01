using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Provides helper methods and properties for common use in the NUnit tests.
    /// </summary>
    public static class NUnitHelper
    {
        #region Public Methods

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
            Assert.IsNotNull(propertyInfo);

            var expectedReadability = expectedAccessMode != PropertyAccessMode.WriteOnly;
            var actualReadability = propertyInfo.CanRead
                && AccessAttributesMatch(propertyInfo.GetGetMethod(true), visibleAccessorAttribute);
            Assert.AreEqual(
                expectedReadability,
                actualReadability,
                "The property '{0}{1}{2}' MUST {3}be readable.",
                propertyInfo.DeclaringType.EnsureNotNull().GetFullName(),
                Type.Delimiter,
                propertyInfo.Name,
                expectedReadability ? string.Empty : "NOT ");

            var expectedWritability = expectedAccessMode != PropertyAccessMode.ReadOnly;
            var actualWritability = propertyInfo.CanWrite
                && AccessAttributesMatch(propertyInfo.GetSetMethod(true), visibleAccessorAttribute);
            Assert.AreEqual(
                expectedWritability,
                actualWritability,
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
            if (!ReferenceEquals(value1, null) && !ReferenceEquals(value2, null)
                && equalityExpectation != AssertEqualityExpectation.EqualAndMayBeSame)
            {
                Assert.That(ReferenceEquals(value1, value2), Is.False);
            }

            var equals = equalityExpectation == AssertEqualityExpectation.EqualAndMayBeSame
                || equalityExpectation == AssertEqualityExpectation.EqualAndCannotBeSame;

            Assert.That(EqualityComparer<T>.Default.Equals(value1, value2), Is.EqualTo(equals));
            Assert.That(Equals(value1, value2), Is.EqualTo(equals));

            if (!ReferenceEquals(value1, null))
            {
                Assert.That(value1.Equals(value2), Is.EqualTo(equals));
                Assert.That(((object)value1).Equals(value2), Is.EqualTo(equals));
            }

            if (!ReferenceEquals(value2, null))
            {
                Assert.That(value2.Equals(value1), Is.EqualTo(equals));
                Assert.That(((object)value2).Equals(value1), Is.EqualTo(equals));
            }

            if (equals && !ReferenceEquals(value1, null) && !ReferenceEquals(value2, null))
            {
                Assert.That(value1.GetHashCode(), Is.EqualTo(value2.GetHashCode()));
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
            [CanBeNull] Action<TestCaseData> processTestCase,
            [NotNull] params object[] arguments)
        {
            Assert.That(arguments, Is.Not.Null);
            CollectionAssert.IsNotEmpty(arguments);

            var result = new List<TestCaseData>();

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
                    if (processTestCase != null)
                    {
                        processTestCase(testCase);
                    }

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
        {
            return GenerateCombinatorialTestCases(null, arguments);
        }

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
        public static T AssertNotNull<T>(this T value)
            where T : class
        {
            Assert.That(value, Is.Not.Null);
            return value;
        }

        #endregion

        #region Private Methods

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
        private static bool AccessAttributesMatch(MethodInfo method, MethodAttributes expectedAttribute)
        {
            Assert.That(method, Is.Not.Null);

            const MethodAttributes Mask = MethodAttributes.MemberAccessMask;

            var result = (method.Attributes & Mask) == (expectedAttribute & Mask);
            return result;
        }

        #endregion

        #region For<TObject> Class

        /// <summary>
        ///     Provides a convenient access to helper methods for the specified type.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type that the helper methods are provided for.
        /// </typeparam>
        public static class For<TObject>
        {
            #region Public Methods

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
            {
                AssertReadableWritable<TObject, TProperty>(
                    propertyGetterExpression,
                    expectedAccessMode,
                    visibleAccessorAttribute);
            }

            #endregion
        }

        #endregion
    }
}