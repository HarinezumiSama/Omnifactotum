﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Provides functionality to recursively validate objects annotated
    ///     with <see cref="MemberConstraintAttribute"/>.
    /// </summary>
    public static class ObjectValidator
    {
        /// <summary>
        ///     The root object parameter name (used in expressions).
        /// </summary>
        internal const string RootObjectParameterName = "instance";

        private static readonly MethodInfo EnumerableCastBaseMethodInfo =
            new Func<IEnumerable, object>(Enumerable.Cast<object>).Method.GetGenericMethodDefinition();

        private static readonly MethodInfo EnumerableCastToObjectMethodInfo =
            EnumerableCastBaseMethodInfo.MakeGenericMethod(typeof(object));

        private static readonly MethodInfo EnumerableSkipBaseMethodInfo =
            new Func<IEnumerable<object>, int, IEnumerable<object>>(Enumerable.Skip)
                .Method
                .GetGenericMethodDefinition();

        private static readonly MethodInfo EnumerableSkipOfObjectMethodInfo =
            EnumerableSkipBaseMethodInfo.MakeGenericMethod(typeof(object));

        private static readonly MethodInfo EnumerableFirstBaseMethodInfo =
            new Func<IEnumerable<object>, object>(Enumerable.First).Method.GetGenericMethodDefinition();

        private static readonly MethodInfo EnumerableFirstOfObjectMethodInfo =
            EnumerableFirstBaseMethodInfo.MakeGenericMethod(typeof(object));

        /// <summary>
        ///     Validates the specified instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instance to validate.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to validate.
        /// </param>
        /// <returns>
        ///     An <see cref="ObjectValidationResult"/> representing the validation result.
        /// </returns>
        [NotNull]
        public static ObjectValidationResult Validate<T>([NotNull] T instance)
        {
            return Validate(instance, null);
        }

        /// <summary>
        ///     Validates the specified instance.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instance to validate.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to validate.
        /// </param>
        /// <param name="recursiveProcessingContext">
        ///     The context of the recursive processing, or <c>null</c> to use a new context.
        /// </param>
        /// <returns>
        ///     An <see cref="ObjectValidationResult"/> representing the validation result.
        /// </returns>
        [NotNull]
        internal static ObjectValidationResult Validate<T>(
            [NotNull] T instance,
            [CanBeNull] RecursiveProcessingContext<MemberData> recursiveProcessingContext)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var parameterExpression = Expression.Parameter(instance.GetType(), RootObjectParameterName);
            var rootMemberData = new MemberData(parameterExpression, null, instance, null, null);

            var objectValidatorContext = new ObjectValidatorContext(recursiveProcessingContext);

            Factotum.ProcessRecursively(
                rootMemberData,
                GetMembers,
                obj => ValidateInternal(instance, parameterExpression, obj, objectValidatorContext));

            return new ObjectValidationResult(objectValidatorContext.Errors.Items);
        }

        private static bool IsReadableProperty([NotNull] MemberInfo info)
        {
            var propertyInfo = info as PropertyInfo;
            return propertyInfo != null && propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.CanRead;
        }

        private static bool IsSimpleTypeInternal([NotNull] Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type.IsPointer
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(Pointer)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type.IsNullable();
        }

        private static BaseMemberConstraintAttribute[] FilterBy<TAttribute>(
            this IEnumerable<BaseValidatableMemberAttribute> attributes)
            where TAttribute : BaseMemberConstraintAttribute
            => attributes?.Where(obj => obj is TAttribute).Cast<BaseMemberConstraintAttribute>().ToArray();

        private static object GetMemberValue(object instance, MemberInfo memberInfo)
        {
            var field = memberInfo as FieldInfo;
            if (field != null)
            {
                return field.GetValue(instance);
            }

            var property = (PropertyInfo)memberInfo;
            return property.GetValue(instance, null);
        }

        private static IEnumerable<MemberData> GetMembers(MemberData parentMemberData)
        {
            if (parentMemberData is null)
            {
                throw new ArgumentNullException(nameof(parentMemberData));
            }

            var instance = parentMemberData.Value;
            var instanceType = instance.GetTypeSafely();

            if (instance is null || IsSimpleTypeInternal(instanceType))
            {
                return Enumerable.Empty<MemberData>();
            }

            var parentExpression = parentMemberData.Expression;

            var allDataMemberInfos = instanceType.FindMembers(
                MemberTypes.Field | MemberTypes.Property,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                (info, criteria) => info is FieldInfo || IsReadableProperty(info),
                null);

            var internalMembers = allDataMemberInfos
                .Select(
                    obj =>
                        new
                        {
                            Member = obj,
                            Attributes = obj.GetCustomAttributeArray<BaseValidatableMemberAttribute>(true)
                        })
                .Where(obj => obj.Attributes.Length != 0)
                .ToArray();

            var members = internalMembers
                .Select(
                    obj => new MemberData(
                        Expression.MakeMemberAccess(
                            ValidationFactotum.ConvertTypeAuto(parentExpression, instance),
                            obj.Member),
                        instance,
                        GetMemberValue(instance, obj.Member),
                        obj.Attributes,
                        obj.Attributes.FilterBy<MemberConstraintAttribute>()))
                .ToList();

            if (parentExpression.Type.IsArray && parentExpression.Type.GetArrayRank() == 1)
            {
                var array = (Array)instance;
                for (var index = 0; index < array.Length; index++)
                {
                    var item = array.GetValue(index);

                    var expression = ValidationFactotum.ConvertTypeAuto(
                        Expression.ArrayIndex(parentExpression, Expression.Constant(index)),
                        item);

                    var itemData = new MemberData(
                        expression,
                        instance,
                        item,
                        parentMemberData.Attributes,
                        parentMemberData.Attributes.FilterBy<MemberItemConstraintAttribute>());

                    members.Add(itemData);
                }
            }
            //////// TODO [HarinezumiSama] Support IEnumerable<T>
            ////else if (parentExpression.Type.IsGenericType
            ////    && typeof(IEnumerable<>).IsAssignableFrom(parentExpression.Type.GetGenericTypeDefinition()))
            ////{
            ////    throw new NotImplementedException();
            ////}
            else if (typeof(IEnumerable).IsAssignableFrom(parentExpression.Type))
            {
                var enumerable = (IEnumerable)instance;

                var enumerableParentExpression = Expression.Call(
                    EnumerableCastToObjectMethodInfo,
                    ValidationFactotum.ConvertTypeAuto(parentExpression, typeof(IEnumerable)));

                var index = 0;
                foreach (var item in enumerable)
                {
                    var optionalSkipExpression = index == 0
                        ? enumerableParentExpression
                        : Expression.Call(
                            EnumerableSkipOfObjectMethodInfo,
                            enumerableParentExpression,
                            Expression.Constant(index));

                    var firstExpression = Expression.Call(EnumerableFirstOfObjectMethodInfo, optionalSkipExpression);

                    var itemData = new MemberData(
                        firstExpression,
                        instance,
                        item,
                        parentMemberData.Attributes,
                        parentMemberData.Attributes.FilterBy<MemberItemConstraintAttribute>());

                    members.Add(itemData);

                    index++;
                }
            }

            return members;
        }

        private static void ValidateInternal(
            object root,
            ParameterExpression parameterExpression,
            MemberData memberData,
            ObjectValidatorContext objectValidatorContext)
        {
            root.EnsureNotNull();
            parameterExpression.EnsureNotNull();
            memberData.EnsureNotNull();
            objectValidatorContext.EnsureNotNull();

            var effectiveAttributes = memberData.EffectiveAttributes;
            if (effectiveAttributes is null || effectiveAttributes.Length == 0)
            {
                return;
            }

            foreach (var constraintAttribute in effectiveAttributes)
            {
                var constraint = objectValidatorContext.ResolveConstraint(constraintAttribute.ConstraintType);

                var context = new MemberConstraintValidationContext(
                    root,
                    memberData.Container,
                    memberData.Expression,
                    parameterExpression);

                constraint.Validate(objectValidatorContext, context, memberData.Value);
            }
        }
    }
}