using System;
using System.Linq.Expressions;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents the context of member constraint validation.
/// </summary>
public sealed class MemberConstraintValidationContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MemberConstraintValidationContext"/> class.
    /// </summary>
    /// <param name="root">
    ///     The root object that is being, or was, validated.
    /// </param>
    /// <param name="container">
    ///     The object containing the value that is being, or was, validated.
    /// </param>
    /// <param name="expression">
    ///     The expression describing the path to the value starting from the root object.
    /// </param>
    /// <param name="rootParameterExpression">
    ///     The root parameter expression.
    /// </param>
    internal MemberConstraintValidationContext(
        [NotNull] object root,
        [NotNull] object container,
        [NotNull] Expression expression,
        [NotNull] ParameterExpression rootParameterExpression)
    {
        Root = root ?? throw new ArgumentNullException(nameof(root));
        Container = container ?? throw new ArgumentNullException(nameof(container));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        RootParameterExpression = rootParameterExpression ?? throw new ArgumentNullException(nameof(rootParameterExpression));

        LambdaExpression = Expression.Lambda(expression, rootParameterExpression);
    }

    /// <summary>
    ///     Gets the root object that is being, or was, checked.
    /// </summary>
    [NotNull]
    public object Root { get; }

    /// <summary>
    ///     Gets the object containing the value that is being, or was, validated.
    /// </summary>
    [NotNull]
    public object Container { get; }

    /// <summary>
    ///     Gets the expression describing the path to the value from the root object.
    /// </summary>
    [NotNull]
    public Expression Expression { get; }

    /// <summary>
    ///     Gets the lambda expression describing the path to the value from the root object.
    /// </summary>
    [NotNull]
    internal LambdaExpression LambdaExpression { get; }

    /// <summary>
    ///     Gets the root parameter expression.
    /// </summary>
    internal ParameterExpression RootParameterExpression { get; }

    /// <summary>
    ///     Creates a lambda expression, using the specified parameter name, based on the expression describing
    ///     the path to the value from the root object.
    /// </summary>
    /// <param name="parameterName">
    ///     The name of the parameter to use in the lambda expression.
    /// </param>
    /// <param name="parameterExpression">
    ///     When this method returns, contains the parameter expression used in the created lambda expression.
    /// </param>
    /// <returns>
    ///     A lambda expression based on the expression describing the path to the value from the root object.
    /// </returns>
    [NotNull]
    public Expression<Func<object, object>> CreateLambdaExpression(string parameterName, out ParameterExpression parameterExpression)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
        {
            throw new ArgumentException(
                @"The value can be neither empty nor whitespace-only string nor null.",
                nameof(parameterName));
        }

        var rootType = Root.GetTypeSafely();

        parameterExpression = Expression.Parameter(typeof(object), parameterName);

        var combined = Expression
            .Lambda(Expression.Convert(parameterExpression, rootType), parameterExpression)
            .InjectInto(LambdaExpression);

        var temporaryParameterExpression = Expression.Parameter(combined.ReturnType, "x");

        var result = combined.InjectInto(
            Expression.Lambda(
                Expression.Convert(temporaryParameterExpression, typeof(object)),
                temporaryParameterExpression));

        return (Expression<Func<object, object>>)result;
    }

    /// <summary>
    ///     Creates a lambda expression, using the specified parameter name, based on the expression describing
    ///     the path to the value from the root object.
    /// </summary>
    /// <param name="parameterName">
    ///     The name of the parameter to use in the lambda expression.
    /// </param>
    /// <returns>
    ///     A lambda expression based on the expression describing the path to the value from the root object.
    /// </returns>
    [NotNull]
    public Expression<Func<object, object>> CreateLambdaExpression(string parameterName) => CreateLambdaExpression(parameterName, out _);

    /// <summary>
    ///     Creates a lambda expression, using the default parameter name, based on the expression describing
    ///     the path to the value from the root object.
    /// </summary>
    /// <param name="parameterExpression">
    ///     When this method returns, contains the parameter expression used in the created lambda expression.
    /// </param>
    /// <returns>
    ///     A lambda expression based on the expression describing the path to the value from the root object.
    /// </returns>
    [NotNull]
    public Expression<Func<object, object>> CreateLambdaExpression(out ParameterExpression parameterExpression)
        => CreateLambdaExpression(ObjectValidator.DefaultRootObjectParameterName, out parameterExpression);

    /// <summary>
    ///     Creates a lambda expression, using the default parameter name, based on the expression describing
    ///     the path to the value from the root object.
    /// </summary>
    /// <returns>
    ///     A lambda expression based on the expression describing the path to the value from the root object.
    /// </returns>
    [NotNull]
    public Expression<Func<object, object>> CreateLambdaExpression() => CreateLambdaExpression(ObjectValidator.DefaultRootObjectParameterName);
}