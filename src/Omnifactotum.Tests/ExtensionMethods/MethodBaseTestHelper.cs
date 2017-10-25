using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    internal sealed class MethodBaseTestHelper
    {
        public static readonly MethodBaseTestHelper Instance = new MethodBaseTestHelper();

        private MethodBaseTestHelper()
        {
            InstanceConstructorMethod = MethodBase.GetCurrentMethod();
            InstanceConstructorName = ".ctor";

            var genericMethodDefinition = GetType().GetMethod(
                MethodName,
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            Assert.That(genericMethodDefinition, Is.Not.Null);
            Assert.That(genericMethodDefinition.IsGenericMethodDefinition, Is.True);
            GenericMethodDefinition = genericMethodDefinition;

            var genericMethod = genericMethodDefinition.MakeGenericMethod(
                typeof(List<Dictionary<Attribute, Exception>>),
                typeof(ConsoleKeyInfo));

            Assert.That(genericMethod, Is.Not.Null);
            GenericMethod = genericMethod;

            ////List<Dictionary<Attribute, Exception>> list = null;
            ////var info = new ConsoleKeyInfo();
            ////var nullableInfo = new ConsoleKeyInfo?();

            ////List<Dictionary<Attribute, Exception>>[] listArray = null;
            ////ConsoleKeyInfo[] infoArray = null;
            ////ConsoleKeyInfo?[] nullableInfoArray = null;

            ////string stringValue = null;
            ////var longValue = 0L;
            ////long? nullableLongValue = 0L;
            ////long* pointerToLongValue = null;

            ////string[] stringArray = null;
            ////long[] longArray = null;
            ////long?[] nullableLongArray = null;
            ////long*[] pointerToLongArray = null;

            ////var testMethod = GetTestMethod(
            ////    null,
            ////    ref list,
            ////    out _,
            ////    info,
            ////    ref info,
            ////    out _,
            ////    null,
            ////    ref nullableInfo,
            ////    out _,
            ////    null,
            ////    ref listArray,
            ////    out _,
            ////    null,
            ////    ref infoArray,
            ////    out _,
            ////    null,
            ////    ref nullableInfoArray,
            ////    out _,
            ////    null,
            ////    ref stringValue,
            ////    out _,
            ////    longValue,
            ////    ref longValue,
            ////    out _,
            ////    nullableLongValue,
            ////    ref nullableLongValue,
            ////    out _,
            ////    null,
            ////    ref pointerToLongValue,
            ////    out _,
            ////    null,
            ////    ref stringArray,
            ////    out _,
            ////    null,
            ////    ref longArray,
            ////    out _,
            ////    null,
            ////    ref nullableLongArray,
            ////    out _,
            ////    null,
            ////    ref pointerToLongArray,
            ////    out _);

            ////Assert.That(testMethod, Is.Not.Null);
        }

        public MethodBase InstanceConstructorMethod
        {
            get;
        }

        public string InstanceConstructorName
        {
            get;
        }

        public string MethodName => nameof(GetTestMethod);

        public MethodBase GenericMethodDefinition
        {
            get;
        }

        public MethodBase GenericMethod
        {
            get;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static unsafe MethodBase GetTestMethod<TReference, TValue>(
            TReference byValueReferenceTypeGenericValue,
            ref TReference byRefReferenceTypeGenericValue,
            out TReference byOutReferenceTypeGenericValue,
            TValue byValueValueTypeGenericValue,
            ref TValue byRefValueTypeGenericValue,
            out TValue byOutValueTypeGenericValue,
            TValue? byValueNullableValueTypeGenericValue,
            ref TValue? byRefNullableValueTypeGenericValue,
            out TValue? byOutNullableValueTypeGenericValue,
            TReference[] byValueArrayOfReferenceTypeGenericValues,
            ref TReference[] byRefArrayOfReferenceTypeGenericValues,
            out TReference[] byOutArrayOfReferenceTypeGenericValues,
            TValue[] byValueArrayOfValueTypeGenericValues,
            ref TValue[] byRefArrayOfValueTypeGenericValues,
            out TValue[] byOutArrayOfValueTypeGenericValues,
            TValue?[] byValueArrayOfNullableValueTypeGenericValues,
            ref TValue?[] byRefArrayOfNullableValueTypeGenericValues,
            out TValue?[] byOutArrayOfNullableValueTypeGenericValues,
            string byValueReferenceTypeSpecificValue,
            ref string byRefReferenceTypeSpecificValue,
            out string byOutReferenceTypeSpecificValue,
            long byValueValueTypeSpecificValue,
            ref long byRefValueTypeSpecificValue,
            out long byOutValueTypeSpecificValue,
            long? byValueNullableValueTypeSpecificValue,
            ref long? byRefNullableValueTypeSpecificValue,
            out long? byOutNullableValueTypeSpecificValue,
            long* byValuePointerToValueTypeSpecificValue,
            ref long* byRefPointerToValueTypeSpecificValue,
            out long* byOutPointerToValueTypeSpecificValue,
            string[] byValueArrayOfReferenceTypeSpecificValue,
            ref string[] byRefArrayOfReferenceTypeSpecificValue,
            out string[] byOutArrayOfReferenceTypeSpecificValue,
            long[] byValueArrayOfValueTypeSpecificValue,
            ref long[] byRefArrayOfValueTypeSpecificValue,
            out long[] byOutArrayOfValueTypeSpecificValue,
            long?[] byValueArrayOfNullableValueTypeSpecificValue,
            ref long?[] byRefArrayOfNullableValueTypeSpecificValue,
            out long?[] byOutArrayOfNullableValueTypeSpecificValue,
            long*[] byValueArrayOfPointerToValueTypeSpecificValue,
            ref long*[] byRefArrayOfPointerToValueTypeSpecificValue,
            out long*[] byOutArrayOfPointerToValueTypeSpecificValue)
            where TReference : class
            where TValue : struct
        {
            byValueReferenceTypeGenericValue.UseValue();
            byRefReferenceTypeGenericValue.UseValue();
            byOutReferenceTypeGenericValue = default(TReference);
            byValueValueTypeGenericValue.UseValue();
            byRefValueTypeGenericValue.UseValue();
            byOutValueTypeGenericValue = default(TValue);
            byValueNullableValueTypeGenericValue.UseValue();
            byRefNullableValueTypeGenericValue.UseValue();
            byOutNullableValueTypeGenericValue = default(TValue?);
            byValueArrayOfReferenceTypeGenericValues.UseValue();
            byRefArrayOfReferenceTypeGenericValues.UseValue();
            byOutArrayOfReferenceTypeGenericValues = default(TReference[]);
            byValueArrayOfValueTypeGenericValues.UseValue();
            byRefArrayOfValueTypeGenericValues.UseValue();
            byOutArrayOfValueTypeGenericValues = default(TValue[]);
            byValueArrayOfNullableValueTypeGenericValues.UseValue();
            byRefArrayOfNullableValueTypeGenericValues.UseValue();
            byOutArrayOfNullableValueTypeGenericValues = default(TValue?[]);
            byValueReferenceTypeSpecificValue.UseValue();
            byRefReferenceTypeSpecificValue.UseValue();
            byOutReferenceTypeSpecificValue = default(string);
            byValueValueTypeSpecificValue.UseValue();
            byRefValueTypeSpecificValue.UseValue();
            byOutValueTypeSpecificValue = default(long);
            byValueNullableValueTypeSpecificValue.UseValue();
            byRefNullableValueTypeSpecificValue.UseValue();
            byOutNullableValueTypeSpecificValue = default(long?);
            Assert.That(byValuePointerToValueTypeSpecificValue == null, Is.True);
            Assert.That(byRefPointerToValueTypeSpecificValue == null, Is.True);
            byOutPointerToValueTypeSpecificValue = default(long*);
            byValueArrayOfReferenceTypeSpecificValue.UseValue();
            byRefArrayOfReferenceTypeSpecificValue.UseValue();
            byOutArrayOfReferenceTypeSpecificValue = default(string[]);
            byValueArrayOfValueTypeSpecificValue.UseValue();
            byRefArrayOfValueTypeSpecificValue.UseValue();
            byOutArrayOfValueTypeSpecificValue = default(long[]);
            byValueArrayOfNullableValueTypeSpecificValue.UseValue();
            byRefArrayOfNullableValueTypeSpecificValue.UseValue();
            byOutArrayOfNullableValueTypeSpecificValue = default(long?[]);
            byValueArrayOfPointerToValueTypeSpecificValue.UseValue();
            byRefArrayOfPointerToValueTypeSpecificValue.UseValue();
            byOutArrayOfPointerToValueTypeSpecificValue = default(long*[]);

            return MethodBase.GetCurrentMethod();
        }
    }
}