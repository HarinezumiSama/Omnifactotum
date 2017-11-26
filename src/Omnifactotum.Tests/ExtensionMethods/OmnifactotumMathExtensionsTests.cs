using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Tests.Internal;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    internal sealed class OmnifactotumMathExtensionsTests
    {
        internal static IEnumerable<SimpleTestCase<int>> TestSqrWithIntSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0, Is.EqualTo(0));
                yield return SimpleTestCase.Create(13, Is.EqualTo(169));
                yield return SimpleTestCase.Create(-17, Is.EqualTo(289));
                yield return SimpleTestCase.Create(126, Is.EqualTo(15_876));
                yield return SimpleTestCase.Create(-2_048, Is.EqualTo(4_194_304));
                yield return SimpleTestCase.Create((int)short.MaxValue, Is.EqualTo(1_073_676_289));
                yield return SimpleTestCase.Create(46_340, Is.EqualTo(2_147_395_600));
                yield return SimpleTestCase.Create(46_341, Is.EqualTo(-2_147_479_015));
                yield return SimpleTestCase.Create(-2_147_479_015, Is.EqualTo(21_464_689));
                yield return SimpleTestCase.Create(2_147_479_015, Is.EqualTo(21_464_689));
            }
        }

        internal static IEnumerable<SimpleTestCase<uint>> TestSqrWithUintSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0U, Is.EqualTo(0U));
                yield return SimpleTestCase.Create(13U, Is.EqualTo(169U));
                yield return SimpleTestCase.Create(17U, Is.EqualTo(289U));
                yield return SimpleTestCase.Create(126U, Is.EqualTo(15_876U));
                yield return SimpleTestCase.Create(2_048U, Is.EqualTo(4_194_304U));
                yield return SimpleTestCase.Create((uint)ushort.MaxValue, Is.EqualTo(4_294_836_225U));
                yield return SimpleTestCase.Create(ushort.MaxValue + 1U, Is.EqualTo(0U));
                yield return SimpleTestCase.Create(ushort.MaxValue + 2U, Is.EqualTo(131_073U));
            }
        }

        internal static IEnumerable<SimpleTestCase<long>> TestSqrWithLongSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0L, Is.EqualTo(0L));
                yield return SimpleTestCase.Create(13L, Is.EqualTo(169L));
                yield return SimpleTestCase.Create(-17L, Is.EqualTo(289L));
                yield return SimpleTestCase.Create(126L, Is.EqualTo(15_876L));
                yield return SimpleTestCase.Create(-2_048L, Is.EqualTo(4_194_304L));
                yield return SimpleTestCase.Create((long)ushort.MaxValue, Is.EqualTo(4_294_836_225L));
                yield return SimpleTestCase.Create((long)int.MaxValue, Is.EqualTo(4_611_686_014_132_420_609L));
                yield return SimpleTestCase.Create(611_686_014_132_420_609L, Is.EqualTo(-9_163_634_054_986_203_135L));
                yield return
                    SimpleTestCase.Create(-611_686_014_132_420_609L, Is.EqualTo(-9_163_634_054_986_203_135L));
            }
        }

        internal static IEnumerable<SimpleTestCase<ulong>> TestSqrWithUlongSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0UL, Is.EqualTo(0UL));
                yield return SimpleTestCase.Create(13UL, Is.EqualTo(169UL));
                yield return SimpleTestCase.Create(17UL, Is.EqualTo(289UL));
                yield return SimpleTestCase.Create(126UL, Is.EqualTo(15_876UL));
                yield return SimpleTestCase.Create(2_048UL, Is.EqualTo(4_194_304UL));
                yield return SimpleTestCase.Create((ulong)ushort.MaxValue, Is.EqualTo(4_294_836_225UL));
                yield return SimpleTestCase.Create((ulong)uint.MaxValue, Is.EqualTo(18_446_744_065_119_617_025UL));
                yield return
                    SimpleTestCase.Create(611_686_014_132_420_609UL, Is.EqualTo(9_283_110_018_723_348_481UL));
            }
        }

        internal static IEnumerable<SimpleTestCase<float>> TestSqrWithFloatSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0F, Is.EqualTo(0F));
                yield return SimpleTestCase.Create(13F, Is.EqualTo(169F));
                yield return SimpleTestCase.Create(-17F, Is.EqualTo(289F));
                yield return SimpleTestCase.Create(126F, Is.EqualTo(15_876F));
                yield return SimpleTestCase.Create(-2_048F, Is.EqualTo(4_194_304F));
                yield return SimpleTestCase.Create((float)short.MaxValue, Is.EqualTo(1.07367629E+09F));
                yield return SimpleTestCase.Create(123_456_789F, Is.EqualTo(1.52415794E+16F));
                yield return SimpleTestCase.Create(-123_456_789F, Is.EqualTo(1.52415794E+16F));
                yield return SimpleTestCase.Create(1.84467435239537E+19F, Is.EqualTo(3.40282326E+38F));
                yield return SimpleTestCase.Create(-1.84467435239537E+19F, Is.EqualTo(3.40282326E+38F));
                yield return SimpleTestCase.Create(1.84467435239537E+20F, Is.EqualTo(float.PositiveInfinity));
                yield return SimpleTestCase.Create(-1.84467435239537E+20F, Is.EqualTo(float.PositiveInfinity));
            }
        }

        internal static IEnumerable<SimpleTestCase<double>> TestSqrWithDoubleSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0D, Is.EqualTo(0D));
                yield return SimpleTestCase.Create(13D, Is.EqualTo(169D));
                yield return SimpleTestCase.Create(-17D, Is.EqualTo(289D));
                yield return SimpleTestCase.Create(126D, Is.EqualTo(15_876D));
                yield return SimpleTestCase.Create(-2_048D, Is.EqualTo(4_194_304D));
                yield return SimpleTestCase.Create((double)short.MaxValue, Is.EqualTo(1_073_676_289D));
                yield return SimpleTestCase.Create(123_456_789D, Is.EqualTo(15_241_578_750_190_520D));
                yield return SimpleTestCase.Create(-123_456_789D, Is.EqualTo(15_241_578_750_190_520D));
                yield return SimpleTestCase.Create(15_241_578_750_190_520D, Is.EqualTo(2.3230572279825921E+32D));
                yield return SimpleTestCase.Create(-15_241_578_750_190_520D, Is.EqualTo(2.3230572279825921E+32D));
                yield return SimpleTestCase.Create(1.34078079299425E+154D, Is.EqualTo(1.79769313486229E+308));
                yield return SimpleTestCase.Create(-1.34078079299425E+154D, Is.EqualTo(1.79769313486229E+308));
                yield return SimpleTestCase.Create(1.34078079299425E+155D, Is.EqualTo(double.PositiveInfinity));
                yield return SimpleTestCase.Create(-1.34078079299425E+155D, Is.EqualTo(double.PositiveInfinity));
            }
        }

        internal static IEnumerable<SimpleTestCase<int>> TestSqrCheckedWithIntSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0, Is.EqualTo(0));
                yield return SimpleTestCase.Create(13, Is.EqualTo(169));
                yield return SimpleTestCase.Create(-17, Is.EqualTo(289));
                yield return SimpleTestCase.Create(126, Is.EqualTo(15_876));
                yield return SimpleTestCase.Create(-2_048, Is.EqualTo(4_194_304));
                yield return SimpleTestCase.Create((int)short.MaxValue, Is.EqualTo(1_073_676_289));
                yield return SimpleTestCase.Create(46_340, Is.EqualTo(2_147_395_600));
                yield return SimpleTestCase.Create(46_341, Throws.TypeOf<OverflowException>());
                yield return SimpleTestCase.Create(-2_147_479_015, Throws.TypeOf<OverflowException>());
                yield return SimpleTestCase.Create(2_147_479_015, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<uint>> TestSqrCheckedWithUintSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0U, Is.EqualTo(0U));
                yield return SimpleTestCase.Create(13U, Is.EqualTo(169U));
                yield return SimpleTestCase.Create(17U, Is.EqualTo(289U));
                yield return SimpleTestCase.Create(126U, Is.EqualTo(15_876U));
                yield return SimpleTestCase.Create(2_048U, Is.EqualTo(4_194_304U));
                yield return SimpleTestCase.Create((uint)ushort.MaxValue, Is.EqualTo(4_294_836_225U));
                yield return SimpleTestCase.Create(ushort.MaxValue + 1U, Throws.TypeOf<OverflowException>());
                yield return SimpleTestCase.Create(ushort.MaxValue + 2U, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<long>> TestSqrCheckedWithLongSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0L, Is.EqualTo(0L));
                yield return SimpleTestCase.Create(13L, Is.EqualTo(169L));
                yield return SimpleTestCase.Create(-17L, Is.EqualTo(289L));
                yield return SimpleTestCase.Create(126L, Is.EqualTo(15_876L));
                yield return SimpleTestCase.Create(-2_048L, Is.EqualTo(4_194_304L));
                yield return SimpleTestCase.Create((long)ushort.MaxValue, Is.EqualTo(4_294_836_225L));
                yield return SimpleTestCase.Create((long)int.MaxValue, Is.EqualTo(4_611_686_014_132_420_609L));
                yield return SimpleTestCase.Create(611_686_014_132_420_609L, Throws.TypeOf<OverflowException>());
                yield return SimpleTestCase.Create(-611_686_014_132_420_609L, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<ulong>> TestSqrCheckedWithUlongSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0UL, Is.EqualTo(0UL));
                yield return SimpleTestCase.Create(13UL, Is.EqualTo(169UL));
                yield return SimpleTestCase.Create(17UL, Is.EqualTo(289UL));
                yield return SimpleTestCase.Create(126UL, Is.EqualTo(15_876UL));
                yield return SimpleTestCase.Create(2_048UL, Is.EqualTo(4_194_304UL));
                yield return SimpleTestCase.Create((ulong)ushort.MaxValue, Is.EqualTo(4_294_836_225UL));
                yield return SimpleTestCase.Create((ulong)uint.MaxValue, Is.EqualTo(18_446_744_065_119_617_025UL));
                yield return SimpleTestCase.Create(611_686_014_132_420_609UL, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<decimal>> TestSqrCheckedWithDecimalSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0M, Is.EqualTo(0M));
                yield return SimpleTestCase.Create(13M, Is.EqualTo(169M));
                yield return SimpleTestCase.Create(-17M, Is.EqualTo(289M));
                yield return SimpleTestCase.Create(126M, Is.EqualTo(15_876M));
                yield return SimpleTestCase.Create(-2_048M, Is.EqualTo(4_194_304M));
                yield return SimpleTestCase.Create((decimal)short.MaxValue, Is.EqualTo(1_073_676_289M));
                yield return SimpleTestCase.Create(123_456_789M, Is.EqualTo(15_241_578_750_190_521M));
                yield return SimpleTestCase.Create(-123_456_789M, Is.EqualTo(15_241_578_750_190_521M));
                yield return
                    SimpleTestCase.Create(281_474_976_710_655M, Is.EqualTo(79_228_162_514_263_774_643_590_529_025M));
                yield return
                    SimpleTestCase.Create(281_474_976_710_656M, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<double>> TestSqrtSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0D, Is.EqualTo(0D));
                yield return SimpleTestCase.Create(1D, Is.EqualTo(1D));
                yield return SimpleTestCase.Create(2D, Is.EqualTo(1.4142135623730952D));
                yield return SimpleTestCase.Create(1000D, Is.EqualTo(31.622776601683793D));

                yield return SimpleTestCase.Create(-1D, Is.NaN);
            }
        }

        internal static IEnumerable<SimpleTestCase<sbyte>> TestAbsWithSbyteSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create((sbyte)0, Is.EqualTo((sbyte)0));
                yield return SimpleTestCase.Create((sbyte)-1, Is.EqualTo((sbyte)1));
                yield return SimpleTestCase.Create((sbyte)1, Is.EqualTo((sbyte)1));
                yield return SimpleTestCase.Create((sbyte)-sbyte.MaxValue, Is.EqualTo(sbyte.MaxValue));
                yield return SimpleTestCase.Create(sbyte.MaxValue, Is.EqualTo(sbyte.MaxValue));

                yield return SimpleTestCase.Create(sbyte.MinValue, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<short>> TestAbsWithShortSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create((short)0, Is.EqualTo((short)0));
                yield return SimpleTestCase.Create((short)-1, Is.EqualTo((short)1));
                yield return SimpleTestCase.Create((short)1, Is.EqualTo((short)1));
                yield return SimpleTestCase.Create((short)-short.MaxValue, Is.EqualTo(short.MaxValue));
                yield return SimpleTestCase.Create(short.MaxValue, Is.EqualTo(short.MaxValue));

                yield return SimpleTestCase.Create(short.MinValue, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<int>> TestAbsWithIntSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0, Is.EqualTo(0));
                yield return SimpleTestCase.Create(-1, Is.EqualTo(1));
                yield return SimpleTestCase.Create(1, Is.EqualTo(1));
                yield return SimpleTestCase.Create(-int.MaxValue, Is.EqualTo(int.MaxValue));
                yield return SimpleTestCase.Create(int.MaxValue, Is.EqualTo(int.MaxValue));

                yield return SimpleTestCase.Create(int.MinValue, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<long>> TestAbsWithLongSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0L, Is.EqualTo(0L));
                yield return SimpleTestCase.Create(-1L, Is.EqualTo(1L));
                yield return SimpleTestCase.Create(1L, Is.EqualTo(1L));
                yield return SimpleTestCase.Create(-long.MaxValue, Is.EqualTo(long.MaxValue));
                yield return SimpleTestCase.Create(long.MaxValue, Is.EqualTo(long.MaxValue));

                yield return SimpleTestCase.Create(long.MinValue, Throws.TypeOf<OverflowException>());
            }
        }

        internal static IEnumerable<SimpleTestCase<float>> TestAbsWithFloatSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0F, Is.EqualTo(0F));
                yield return SimpleTestCase.Create(-1F, Is.EqualTo(1F));
                yield return SimpleTestCase.Create(1F, Is.EqualTo(1F));
                yield return SimpleTestCase.Create(-float.MaxValue, Is.EqualTo(float.MaxValue));
                yield return SimpleTestCase.Create(float.MaxValue, Is.EqualTo(float.MaxValue));

                yield return SimpleTestCase.Create(float.MinValue, Is.EqualTo(float.MaxValue));
            }
        }

        internal static IEnumerable<SimpleTestCase<double>> TestAbsWithDoubleSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0D, Is.EqualTo(0D));
                yield return SimpleTestCase.Create(-1D, Is.EqualTo(1D));
                yield return SimpleTestCase.Create(1D, Is.EqualTo(1D));
                yield return SimpleTestCase.Create(-double.MaxValue, Is.EqualTo(double.MaxValue));
                yield return SimpleTestCase.Create(double.MaxValue, Is.EqualTo(double.MaxValue));

                yield return SimpleTestCase.Create(double.MinValue, Is.EqualTo(double.MaxValue));
            }
        }

        internal static IEnumerable<SimpleTestCase<decimal>> TestAbsWithDecimalSucceedsCases
        {
            get
            {
                yield return SimpleTestCase.Create(0M, Is.EqualTo(0M));
                yield return SimpleTestCase.Create(-1M, Is.EqualTo(1M));
                yield return SimpleTestCase.Create(1M, Is.EqualTo(1M));
                yield return SimpleTestCase.Create(-decimal.MaxValue, Is.EqualTo(decimal.MaxValue));
                yield return SimpleTestCase.Create(decimal.MaxValue, Is.EqualTo(decimal.MaxValue));

                yield return SimpleTestCase.Create(decimal.MinValue, Is.EqualTo(decimal.MaxValue));
            }
        }

        [Test]
        [TestCaseSource(nameof(TestSqrWithIntSucceedsCases))]
        public void TestSqrWithIntSucceeds(SimpleTestCase<int> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrWithUintSucceedsCases))]
        public void TestSqrWithUintSucceeds(SimpleTestCase<uint> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrWithLongSucceedsCases))]
        public void TestSqrWithLongSucceeds(SimpleTestCase<long> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrWithUlongSucceedsCases))]
        public void TestSqrWithUlongSucceeds(SimpleTestCase<ulong> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrWithFloatSucceedsCases))]
        public void TestSqrWithFloatSucceeds(SimpleTestCase<float> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrWithDoubleSucceedsCases))]
        public void TestSqrWithDoubleSucceeds(SimpleTestCase<double> testCase)
            => Assert.That(() => testCase.Input.Sqr(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrCheckedWithDecimalSucceedsCases))]
        public void TestSqrCheckedWithDecimalSucceeds(SimpleTestCase<decimal> testCase)
            => Assert.That(() => testCase.Input.SqrChecked(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrCheckedWithIntSucceedsCases))]
        public void TestSqrCheckedWithIntSucceeds(SimpleTestCase<int> testCase)
            => Assert.That(() => testCase.Input.SqrChecked(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrCheckedWithUintSucceedsCases))]
        public void TestSqrCheckedWithUintSucceeds(SimpleTestCase<uint> testCase)
            => Assert.That(() => testCase.Input.SqrChecked(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrCheckedWithLongSucceedsCases))]
        public void TestSqrCheckedWithLongSucceeds(SimpleTestCase<long> testCase)
            => Assert.That(() => testCase.Input.SqrChecked(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrCheckedWithUlongSucceedsCases))]
        public void TestSqrCheckedWithUlongSucceeds(SimpleTestCase<ulong> testCase)
            => Assert.That(() => testCase.Input.SqrChecked(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestSqrtSucceedsCases))]
        public void TestSqrtSucceeds(SimpleTestCase<double> testCase)
            => Assert.That(() => testCase.Input.Sqrt(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithSbyteSucceedsCases))]
        public void TestAbsWithSbyteSucceeds(SimpleTestCase<sbyte> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithShortSucceedsCases))]
        public void TestAbsWithShortSucceeds(SimpleTestCase<short> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithIntSucceedsCases))]
        public void TestAbsWithIntSucceeds(SimpleTestCase<int> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithLongSucceedsCases))]
        public void TestAbsWithLongSucceeds(SimpleTestCase<long> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithFloatSucceedsCases))]
        public void TestAbsWithFloatSucceeds(SimpleTestCase<float> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithDoubleSucceedsCases))]
        public void TestAbsWithDoubleSucceeds(SimpleTestCase<double> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);

        [Test]
        [TestCaseSource(nameof(TestAbsWithDecimalSucceedsCases))]
        public void TestAbsWithDecimalSucceeds(SimpleTestCase<decimal> testCase)
            => Assert.That(() => testCase.Input.Abs(), testCase.Constraint);
    }
}