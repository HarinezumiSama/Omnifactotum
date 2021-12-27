#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    internal abstract class OmnifactotumTimeSpanExtensionsTestsBase
    {
        protected static IEnumerable<TestCaseData> GetToFixedStringMethodsTestCases(bool includeNullValue)
        {
            if (includeNullValue)
            {
                yield return new TestCaseData(
                    null,
                    "null",
                    "null",
                    "null");
            }

            yield return new TestCaseData(
                TimeSpan.Zero,
                "00:00:00",
                "00:00:00.000",
                "00:00:00.0000000");

            yield return new TestCaseData(
                TimeSpan.MinValue,
                "-10675199.02:48:05",
                "-10675199.02:48:05.477",
                "-10675199.02:48:05.4775808");

            yield return new TestCaseData(
                TimeSpan.MaxValue,
                "10675199.02:48:05",
                "10675199.02:48:05.477",
                "10675199.02:48:05.4775807");

            var noDaysValue = new TimeSpan(0, 12, 34, 56, 789);

            yield return new TestCaseData(
                noDaysValue,
                "12:34:56",
                "12:34:56.789",
                "12:34:56.7890000");

            yield return new TestCaseData(
                -noDaysValue,
                "-12:34:56",
                "-12:34:56.789",
                "-12:34:56.7890000");

            var oneDayValue = new TimeSpan(1, 12, 34, 56, 789);

            yield return new TestCaseData(
                oneDayValue,
                "1.12:34:56",
                "1.12:34:56.789",
                "1.12:34:56.7890000");

            yield return new TestCaseData(
                -oneDayValue,
                "-1.12:34:56",
                "-1.12:34:56.789",
                "-1.12:34:56.7890000");

            var manyDaysValue = new TimeSpan(997, 23, 45, 16, 987);

            yield return new TestCaseData(
                manyDaysValue,
                "997.23:45:16",
                "997.23:45:16.987",
                "997.23:45:16.9870000");

            yield return new TestCaseData(
                -manyDaysValue,
                "-997.23:45:16",
                "-997.23:45:16.987",
                "-997.23:45:16.9870000");

            var arbitraryValue = TimeSpan.FromTicks(1234567890123L);

            yield return new TestCaseData(
                arbitraryValue,
                "1.10:17:36",
                "1.10:17:36.789",
                "1.10:17:36.7890123");

            yield return new TestCaseData(
                -arbitraryValue,
                "-1.10:17:36",
                "-1.10:17:36.789",
                "-1.10:17:36.7890123");
        }
    }
}