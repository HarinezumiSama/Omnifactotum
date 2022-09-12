using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of <see cref="RecursiveProcessingContext{T}"/> using type inference in a friendly way.
/// </summary>
public static class RecursiveProcessingContext
{
    private static readonly MethodInfo CreateHashSetForReferenceTypeMethodDefinition =
        new Func<IEqualityComparer<object>?, HashSet<object>>(CreateHashSetForReferenceType)
            .Method
            .GetGenericMethodDefinition();

    /// <summary>
    ///     Creates a new instance of the <see cref="RecursiveProcessingContext{T}"/> class
    ///     using the specified equality comparer for the instances being processed recursively.
    /// </summary>
    /// <param name="equalityComparer">
    ///     The equality comparer to use for eliminating duplicated instances,
    ///     or <see langword="null"/> to use <see cref="ByReferenceEqualityComparer{T}"/>.
    /// </param>
    /// <seealso cref="ByReferenceEqualityComparer{T}"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RecursiveProcessingContext<T> Create<T>([CanBeNull] IEqualityComparer<T>? equalityComparer = null) => new(equalityComparer);

    internal static Func<IEqualityComparer<T>?, HashSet<T>?> GenerateCreateHashSetMethod<T>()
    {
        if (typeof(T).IsValueType)
        {
            return _ => null;
        }

        var method = CreateHashSetForReferenceTypeMethodDefinition.MakeGenericMethod(typeof(T));

        var parameterExpression = Expression.Parameter(typeof(IEqualityComparer<T>));
        var methodCallExpression = Expression.Call(method, parameterExpression);

        var result =
            Expression
                .Lambda<Func<IEqualityComparer<T>?, HashSet<T>>>(methodCallExpression, parameterExpression)
                .Compile();

        return result;
    }

    private static HashSet<TInner> CreateHashSetForReferenceType<TInner>([CanBeNull] IEqualityComparer<TInner>? equalityComparer = null)
        where TInner : class
        => new(equalityComparer ?? ByReferenceEqualityComparer<TInner>.Instance);
}