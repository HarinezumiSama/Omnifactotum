using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

#if NET5_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Provides functionality to recursively validate objects annotated with <see cref="MemberConstraintAttribute"/>
///     and <see cref="MemberItemConstraintAttribute"/>.
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
    [Pure]
    [Omnifactotum.Annotations.Pure]
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
            throw new ArgumentException("The value cannot be a blank string (but can be null).", nameof(instanceExpression));
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
    internal static ObjectValidationResult Validate<T>(
        [DisallowNull] T instance,
        string instanceExpression,
        RecursiveProcessingContext<MemberData>? recursiveProcessingContext)
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        if (string.IsNullOrWhiteSpace(instanceExpression))
        {
            throw new ArgumentException("The value cannot be null or a blank string.", nameof(instanceExpression));
        }

        var parameterExpression = Expression.Parameter(instance.GetType(), instanceExpression);

        var rootMemberData = new MemberData(
            parameterExpression,
            null,
            instance,
            Array.Empty<IBaseValidatableMemberAttribute>(),
            Array.Empty<IBaseMemberConstraintAttribute>());

        var objectValidatorContext = new ObjectValidatorContext(recursiveProcessingContext);
        Factotum.ProcessRecursively(rootMemberData, GetMembers, ProcessMember, objectValidatorContext.RecursiveProcessingContext);
        objectValidatorContext.OnCompleteValidation();

        return objectValidatorContext.Errors.Count == 0
            ? ObjectValidationResult.SuccessfulResult
            : new ObjectValidationResult(objectValidatorContext.Errors);

        void ProcessMember(MemberData data)
        {
            var effectiveAttributes = data.EffectiveAttributes;
            if (effectiveAttributes.Length == 0)
            {
                return;
            }

            var context = new MemberConstraintValidationContext(
                objectValidatorContext,
                instance,
                data.Container.EnsureNotNull(),
                data.Expression,
                parameterExpression);

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var constraintAttribute in effectiveAttributes)
            {
                var constraint = objectValidatorContext.ResolveConstraint(constraintAttribute.ConstraintType);
                constraint.Validate(context, data.Value);
            }
        }
    }

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static bool IsReadableProperty(MemberInfo info)
        => info is PropertyInfo { CanRead: true } propertyInfo && propertyInfo.GetIndexParameters().Length == 0;

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static bool IsSimpleTypeInternal(Type type)
        => type.IsPrimitive
            || type.IsEnum
            || type.IsPointer
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(Pointer)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type.IsNullableValueType();

    [Pure]
    [Omnifactotum.Annotations.Pure]
    private static IBaseMemberConstraintAttribute[] FilterBy<TAttribute>(
        this IEnumerable<IBaseValidatableMemberAttribute> attributes)
        where TAttribute : IBaseMemberConstraintAttribute
        => attributes.OfType<TAttribute>().Cast<IBaseMemberConstraintAttribute>().ToArray();

    [MustUseReturnValue]
    private static object? GetMemberValue(object instance, MemberInfo memberInfo)
        => memberInfo switch
        {
            FieldInfo field => field.GetValue(instance),
            PropertyInfo property => property.GetValue(instance, null),
            _ => throw new InvalidOperationException(
                $"Invalid type of {nameof(memberInfo).ToUIString()}: {memberInfo.GetType().GetFullName().ToUIString()}")
        };

    [MustUseReturnValue]
    private static IEnumerable<MemberData> GetMembers(MemberData parentMemberData)
    {
        if (parentMemberData is null)
        {
            throw new ArgumentNullException(nameof(parentMemberData));
        }

        var instance = parentMemberData.Value;
        var instanceType = parentMemberData.ValueType;
        var parentExpression = parentMemberData.Expression;

        if (instance is null || IsSimpleTypeInternal(instanceType) || ValidationFactotum.IsDefaultImmutableArray(instance))
        {
            return Enumerable.Empty<MemberData>();
        }

        var allDataMemberInfos = instanceType.FindMembers(
            MemberTypes.Field | MemberTypes.Property,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            static (info, _) => info is FieldInfo || IsReadableProperty(info),
            null);

        var internalMembers = allDataMemberInfos
            .Select(
                static obj =>
                    new
                    {
                        Member = obj,
                        Attributes = obj.GetCustomAttributeArray<BaseValidatableMemberAttribute>(true).ToArray<IBaseValidatableMemberAttribute>()
                    })
            .Where(static obj => obj.Attributes.Length != 0)
            .OrderBy(static obj => obj.Member.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(static obj => obj.Member.Name, StringComparer.Ordinal)
            .ToArray();

        var members = internalMembers
            .Select(
                obj => new MemberData(
                    Expression.MakeMemberAccess(ValidationFactotum.ConvertTypeAuto(parentExpression, instance), obj.Member),
                    instance,
                    GetMemberValue(instance, obj.Member),
                    obj.Attributes,
                    obj.Attributes.FilterBy<IMemberConstraintAttribute>()))
            .ToList();

        if (instanceType.IsArray && instanceType.GetArrayRank() == 1)
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
                    parentMemberData.Attributes.FilterBy<IMemberItemConstraintAttribute>());

                members.Add(itemData);
            }
        }
        else if (TryAddSupportedListMembers(
            parentMemberData,
            members,
            typeof(ImmutableArray<>),
            typeof(ICollection<>),
            nameof(ICollection<object>.Count)))
        {
            // Nothing to do
        }
        else if (TryAddSupportedListMembers(
            parentMemberData,
            members,
            typeof(IReadOnlyList<>),
            typeof(IReadOnlyCollection<>),
            nameof(IReadOnlyCollection<object>.Count)))
        {
            // Nothing to do
        }
        else if (TryAddSupportedListMembers(
            parentMemberData,
            members,
            typeof(IList<>),
            typeof(ICollection<>),
            nameof(ICollection<object>.Count)))
        {
            // Nothing to do
        }
        else if (TryAddSupportedListMembers(
            parentMemberData,
            members,
            typeof(IList),
            typeof(ICollection),
            nameof(ICollection.Count)))
        {
            // Nothing to do
        }
        else if (instanceType
                .GetInterfaces()
                .FirstOrDefault(static t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)) is { } enumerableType)
        {
            var addGenericEnumerableMembersMethodDefinition =
                ((Expression<Action<MemberData, ICollection<MemberData>>>)(static (data, members) => AddGenericEnumerableMembers<object>(data, members)))
                .GetLastMethod()
                .EnsureNotNull()
                .GetGenericMethodDefinition();

            var declaredElementType = enumerableType.GetGenericArguments().Single();

            var addGenericEnumerableMembersMethod = addGenericEnumerableMembersMethodDefinition.MakeGenericMethod(declaredElementType);
            addGenericEnumerableMembersMethod.Invoke(null, new object?[] { parentMemberData, members });
        }
        else if (typeof(IEnumerable).IsAssignableFrom(instanceType))
        {
            var enumerable = (IEnumerable)instance;

            var enumerableParentExpression = Expression.Call(
                EnumerableCastToObjectMethodInfo,
                ValidationFactotum.ConvertTypeAuto(parentExpression, typeof(IEnumerable)));

            var index = 0;
            //// ReSharper disable once LoopCanBePartlyConvertedToQuery
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
                    parentMemberData.Attributes.FilterBy<IMemberItemConstraintAttribute>());

                members.Add(itemData);

                index++;
            }
        }

        return members;
    }

    [MustUseReturnValue]
    private static bool TryAddSupportedListMembers(
        MemberData parentMemberData,
        ICollection<MemberData> members,
        Type listTypeDefinition,
        Type collectionTypeDefinition,
        string collectionCountPropertyName)
    {
        var isGenericList = listTypeDefinition.IsGenericTypeDefinition;
        var isInterface = listTypeDefinition.IsInterface;

        Factotum.Assert(isGenericList || isInterface);
        Factotum.Assert(collectionTypeDefinition.IsInterface && collectionTypeDefinition.IsGenericTypeDefinition == isGenericList);

        var instance = parentMemberData.Value;
        if (instance is null)
        {
            return false;
        }

        var valueType = parentMemberData.ValueType;

        Type? listType;
        if (isInterface)
        {
            var interfaces = valueType.GetInterfaces();

            listType = isGenericList
                ? interfaces.FirstOrDefault(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == listTypeDefinition)
                : interfaces.Contains(listTypeDefinition)
                    ? listTypeDefinition
                    : null;
        }
        else
        {
            listType = valueType.IsConstructedGenericType && valueType.GetGenericTypeDefinition() is { } type && type == listTypeDefinition
                ? valueType
                : null;
        }

        if (listType is null)
        {
            return false;
        }

        var readOnlyCollectionType = isGenericList
            ? collectionTypeDefinition.MakeGenericType(listType.GetGenericArguments().Single())
            : collectionTypeDefinition;

        Factotum.Assert(readOnlyCollectionType.IsAssignableFrom(listType));

        var countPropertyInfo = readOnlyCollectionType.GetProperty(collectionCountPropertyName, BindingFlags.Instance | BindingFlags.Public).EnsureNotNull();
        Factotum.Assert(countPropertyInfo.PropertyType == typeof(int) && countPropertyInfo.GetMethod is not null);
        var countMethodInfo = valueType.GetInterfaceMethodImplementation(countPropertyInfo.GetMethod);

        var count = Expression.Lambda<Func<int>>(Expression.Property(Expression.Constant(instance), countMethodInfo)).Compile().Invoke();

        var getItemPropertyInfo = listType
            .GetProperties()
            .SingleOrDefault(static propertyInfo => propertyInfo.GetIndexParameters() is [var parameterInfo] && parameterInfo.ParameterType == typeof(int))
            .EnsureNotNull();

        if (isInterface)
        {
            var getItemMethodInfo = valueType.GetInterfaceMethodImplementation(getItemPropertyInfo.GetMethod.EnsureNotNull());

            getItemPropertyInfo = valueType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .SingleOrDefault(p => p.GetMethod == getItemMethodInfo)
                .EnsureNotNull();
        }

        for (var index = 0; index < count; index++)
        {
            var getItemExpression = Expression.MakeIndex(
                Expression.Constant(instance),
                getItemPropertyInfo,
                new Expression[] { Expression.Constant(index) });

            var item = Expression.Lambda<Func<object>>(ValidationFactotum.ConvertTypeAuto(getItemExpression, typeof(object))).Compile().Invoke();

            var memberDataExpression = ValidationFactotum.ConvertTypeAuto(
                Expression.MakeIndex(
                    ValidationFactotum.ConvertTypeAuto(parentMemberData.Expression, instance),
                    getItemPropertyInfo,
                    new Expression[] { Expression.Constant(index) }),
                item);

            var itemData = new MemberData(
                memberDataExpression,
                instance,
                item,
                parentMemberData.Attributes,
                parentMemberData.Attributes.FilterBy<IMemberItemConstraintAttribute>());

            members.Add(itemData);
        }

        return true;
    }

    private static void AddGenericEnumerableMembers<T>(MemberData parentMemberData, ICollection<MemberData> members)
    {
        var enumerable = (IEnumerable<T>)parentMemberData.Value.EnsureNotNull();
        var parentExpression = parentMemberData.Expression;

        var enumerableSkipMethodInfo = EnumerableSkipBaseMethodInfo.MakeGenericMethod(typeof(T));
        var enumerableFirstMethodInfo = EnumerableFirstBaseMethodInfo.MakeGenericMethod(typeof(T));

        var enumerableParentExpression = ValidationFactotum.ConvertTypeAuto(parentExpression, typeof(IEnumerable<T>));

        var index = 0;
        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var item in enumerable)
        {
            var optionalSkipExpression = index == 0
                ? enumerableParentExpression
                : Expression.Call(
                    enumerableSkipMethodInfo,
                    enumerableParentExpression,
                    Expression.Constant(index));

            var firstExpression = Expression.Call(enumerableFirstMethodInfo, optionalSkipExpression);

            var itemData = new MemberData(
                firstExpression,
                enumerable,
                item,
                parentMemberData.Attributes,
                parentMemberData.Attributes.FilterBy<IMemberItemConstraintAttribute>());

            members.Add(itemData);

            index++;
        }
    }
}