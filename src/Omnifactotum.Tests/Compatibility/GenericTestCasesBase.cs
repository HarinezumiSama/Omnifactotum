using System.Collections;
using System.Collections.Generic;

//// ReSharper disable once CheckNamespace
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
        public IEnumerator<T> GetEnumerator() => GetCases().AssertNotNull().GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///     Gets the collection of test cases.
        /// </summary>
        /// <returns>
        ///     The collection of test cases.
        /// </returns>
        protected abstract IEnumerable<T> GetCases();
    }
}