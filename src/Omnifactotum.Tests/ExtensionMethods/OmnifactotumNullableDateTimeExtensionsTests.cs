using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumNullableDateTimeExtensions))]
internal sealed class OmnifactotumNullableDateTimeExtensionsTests
{
    [Test]
    public void TestToFixedString()
    {
        TestSingleCase(null, "null");

        const string NonNullExpectedResult = "2001-02-03 07:08:09";

        TestSingleCase(CreateTestValue(DateTimeKind.Local), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Unspecified), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Utc), NonNullExpectedResult);

        static void TestSingleCase(DateTime? value, string expectedResult)
        {
            var actualResult = value.ToFixedString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public void TestToFixedStringWithMilliseconds()
    {
        TestSingleCase(null, "null");

        const string NonNullExpectedResult = "2001-02-03 07:08:09.456";

        TestSingleCase(CreateTestValue(DateTimeKind.Local), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Unspecified), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Utc), NonNullExpectedResult);

        static void TestSingleCase(DateTime? value, string expectedResult)
        {
            var actualResult = value.ToFixedStringWithMilliseconds();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }

    [Test]
    public void TestToPreciseFixedString()
    {
        TestSingleCase(null, "null");

        const string NonNullExpectedResult = "2001-02-03 07:08:09.4560000";

        TestSingleCase(CreateTestValue(DateTimeKind.Local), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Unspecified), NonNullExpectedResult);
        TestSingleCase(CreateTestValue(DateTimeKind.Utc), NonNullExpectedResult);

        static void TestSingleCase(DateTime? value, string expectedResult)
        {
            var actualResult = value.ToPreciseFixedString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }

    private static DateTime CreateTestValue(DateTimeKind kind) => new(2001, 2, 3, 7, 8, 9, 456, kind);
}