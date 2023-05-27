using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;

#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Provides functionality to recursively validate objects annotated with <see cref="MemberConstraintAttribute"/>.
/// </summary>
public static class ObjectValidator
{
    /// <summary>
    ///     The default root object parameter name (used in expressions).
    /// </summary>
    internal const string DefaultRootObjectParameterName = "instance";

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
    /// <param name="instanceExpression">
    ///     <para>
    ///         A string value representing the expression passed as the value of the <paramref name="instance"/> parameter.
    ///         It's later used in the error validation message to display the path to properties that failed validation.
    ///     </para>
    ///     <para>
    ///         <b>NOTE</b>: By default, the value for this parameter is automatically injected by the compiler (.NET 5+ and C# 10+).
    ///         Pass the value explicitly only if you wish to override it.
    ///     </para>
    /// </param>
    /// <returns>
    ///     An <see cref="ObjectValidationResult"/> representing the validation result.
    /// </returns>
    [NotNull]
    public static ObjectValidationResult Validate<T>(
        [DisallowNull] T instance,
#if NET5_0_OR_GREATER
        [CallerArgumentExpression(nameof(instance))]
#endif
        string? instanceExpression = null)
    {
        if (instanceExpression is not null && string.IsNullOrWhiteSpace(instanceExpression))
        {
            throw new ArgumentException(@"The value cannot be a blank string (but can be null).", nameof(instanceExpression));
        }

        return Validate(instance, instanceExpression ?? DefaultRootObjectParameterName, null);
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
    /// <param name="instanceExpression">
    ///     <para>A string value representing the expression passed as the value of the <paramref name="instance"/> parameter.</para>
    /// </param>
    /// <param name="recursiveProcessingContext">
    ///     The context of the recursive processing, or <see langword="null"/> to use a new context.
    /// </param>
    /// <returns>
    ///     An <see cref="ObjectValidationResult"/> representing the validation result.
    /// </returns>
    [NotNull]
    internal static ObjectValidationResult Validate<T>(
        [DisallowNull] T instance,
        [NotNull] string instanceExpression,
        [CanBeNull] RecursiveProcessingContext<MemberData>? recursiveProcessingContext)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrWhiteSpace(instanceExpression))
        {
            throw new ArgumentException(@"The value cannot be null or a blank string.", nameof(instanceExpression));
        }

        var parameterExpression = Expression.Parameter(instance.GetType(), instanceExpression);
        var rootMemberData = new MemberData(parameterExpression, null, instance, null, null);

        var objectValidatorContext = new ObjectValidatorContext(recursiveProcessingContext);

        Factotum.ProcessRecursively(rootMemberData, GetMembers, ProcessItem);

        return new ObjectValidationResult(objectValidatorContext.Errors.Items);

        void ProcessItem(MemberData data) => ValidateInternal(instance, parameterExpression, data, objectValidatorContext);
    }

    private static bool IsReadableProperty([NotNull] MemberInfo info)
        => info is PropertyInfo { CanRead: true } propertyInfo && propertyInfo.GetIndexParameters().Length == 0;

    private static bool IsSimpleTypeInternal([NotNull] Type type)
        => type.IsPrimitive
            || type.IsEnum
            || type.IsPointer
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(Pointer)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type.IsNullableValueType();

    [CanBeNull]
    private static BaseMemberConstraintAttribute[]? FilterBy<TAttribute>(
        [CanBeNull] this IEnumerable<BaseValidatableMemberAttribute>? attributes)
        where TAttribute : BaseMemberConstraintAttribute
        => attributes?.Where(obj => obj is TAttribute).Cast<BaseMemberConstraintAttribute>().ToArray();

    [CanBeNull]
    private static object? GetMemberValue([NotNull] object instance, [NotNull] MemberInfo memberInfo)
        => memberInfo switch
        {
            FieldInfo field => field.GetValue(instance),
            PropertyInfo property => property.GetValue(instance, null),
            _ => throw new InvalidOperationException(
                $@"Invalid type of {nameof(memberInfo).ToUIString()}: {memberInfo.GetType().GetFullName().ToUIString()}")
        };

    [NotNull]
    private static IEnumerable<MemberData> GetMembers([NotNull] MemberData parentMemberData)
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
            (info, _) => info is FieldInfo || IsReadableProperty(info),
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
                    Expression.MakeMemberAccess(ValidationFactotum.ConvertTypeAuto(parentExpression, instance), obj.Member),
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
        [NotNull] object root,
        [NotNull] ParameterExpression parameterExpression,
        [NotNull] MemberData memberData,
        [NotNull] ObjectValidatorContext objectValidatorContext)
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

        var memberDataContainer = memberData.Container.EnsureNotNull();
        var memberDataExpression = memberData.Expression;

        foreach (var constraintAttribute in effectiveAttributes)
        {
            var constraint = objectValidatorContext.ResolveConstraint(constraintAttribute.ConstraintType);

            var context = new MemberConstraintValidationContext(root, memberDataContainer, memberDataExpression, parameterExpression);
            constraint.Validate(objectValidatorContext, context, memberData.Value);
        }
    }
}