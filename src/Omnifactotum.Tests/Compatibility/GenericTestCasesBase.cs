using System.Collections;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

//// ReSharper disable once CheckNamespace :: Compatibility (Omnifactotum.NUnit)
namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Represents the base class providing test cases for a test.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the object specifying arguments for a test method.
    /// </typeparam>
    internal abstract class GenericTestCasesBase<T> : IEnumerable<T>
    {
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        public IEnumerator<T> GetEnumerator() => GetCases().AssertNotNull().GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        [NotNull]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///     Gets the collection of test cases.
        /// </summary>
        /// <returns>
        ///     The collection of test cases.
        /// </returns>
        [NotNull]
        protected abstract IEnumerable<T> GetCases();
    }
}