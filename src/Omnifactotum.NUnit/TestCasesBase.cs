using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.NUnit
{
    /// <summary>
    ///     Represents the base class providing test cases for a test.
    /// </summary>
    public abstract class TestCasesBase : IEnumerable<TestCaseData>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestCasesBase"/> class.
        /// </summary>
        protected TestCasesBase()
        {
            // Nothing to do
        }

        #endregion

        #region IEnumerable<TestCaseData> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{TestCaseData}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TestCaseData> GetEnumerator()
        {
            return GetCases().AssertNotNull().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the collection of test cases.
        /// </summary>
        /// <returns>
        ///     The collection of test cases.
        /// </returns>
        protected abstract IEnumerable<TestCaseData> GetCases();

        #endregion
    }
}