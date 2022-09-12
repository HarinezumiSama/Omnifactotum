using System.Reflection;
using Omnifactotum.Annotations;
using NotNullIfNotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Linq.Expressions;

/// <summary>
///     Contains extension methods for the classes in the <c>System.Linq.Expressions</c> namespace.
/// </summary>
public static class OmnifactotumExpressionExtensions
{
    /// <summary>
    ///     Gets the last method called in the expression.
    /// </summary>
    /// <typeparam name="TDelegate">
    ///     The type of the delegate.
    /// </typeparam>
    /// <param name="expression">
    ///     The expression to get the last called method of.
    /// </param>
    /// <returns>
    ///     The last called method, or <see langword="null"/> if the last element in the expression is not a method call.
    /// </returns>
    [CanBeNull]
    public static MethodInfo? GetLastMethod<TDelegate>([NotNull] this Expression<TDelegate> expression)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var methodCallExpression = expression.Body as MethodCallExpression;
        return methodCallExpression?.Method.EnsureNotNull();
    }

    /// <summary>
    ///     Injects the specified source lambda expression instead of the sole parameter of the target expression.
    /// </summary>
    /// <typeparam name="TInput">
    ///     The type of the sole input parameter of the source lambda expression.
    /// </typeparam>
    /// <typeparam name="TIntermediate">
    ///     The type of the result of the source lambda expression
    ///     (as well as the type of the sole input parameter of the target lambda expression).
    /// </typeparam>
    /// <typeparam name="TResult">
    ///     The type of the result of the target lambda expression.
    /// </typeparam>
    /// <param name="sourceExpression">
    ///     The lambda expression to inject into the target lambda expression.
    /// </param>
    /// <param name="targetExpression">
    ///     The lambda expression to inject the source lambda expression into.
    /// </param>
    /// <returns>
    ///     A new lambda expression.
    /// </returns>
    [NotNull]
    public static Expression<Func<TInput, TResult>> InjectInto<TInput, TIntermediate, TResult>(
        [NotNull] this Expression<Func<TInput, TIntermediate>> sourceExpression,
        [NotNull] Expression<Func<TIntermediate, TResult>> targetExpression)
    {
        if (sourceExpression is null)
        {
            throw new ArgumentNullException(nameof(sourceExpression));
        }

        if (targetExpression is null)
        {
            throw new ArgumentNullException(nameof(targetExpression));
        }

        var parameterExpression = sourceExpression.Parameters.Single();

        var newSecondBody =
            new ReplaceExpressionVisitor(targetExpression.Parameters.Single(), sourceExpression.Body)
                .Visit(targetExpression.Body);

        var result = Expression.Lambda<Func<TInput, TResult>>(newSecondBody, parameterExpression);
        return result;
    }

    /// <summary>
    ///     Injects the specified source lambda expression instead of the sole parameter of the target expression.
    /// </summary>
    /// <param name="sourceExpression">
    ///     The lambda expression to inject into the target lambda expression.
    /// </param>
    /// <param name="targetExpression">
    ///     The lambda expression to inject the source lambda expression into.
    /// </param>
    /// <returns>
    ///     A new lambda expression.
    /// </returns>
    [NotNull]
    public static LambdaExpression InjectInto(
        [NotNull] this LambdaExpression sourceExpression,
        [NotNull] LambdaExpression targetExpression)
    {
        if (sourceExpression is null)
        {
            throw new ArgumentNullException(nameof(sourceExpression));
        }

        if (sourceExpression.Parameters.Count != 1)
        {
            throw new ArgumentException(
                "The source expression must have a single parameter.",
                nameof(sourceExpression));
        }

        if (targetExpression is null)
        {
            throw new ArgumentNullException(nameof(targetExpression));
        }

        if (targetExpression.Parameters.Count != 1)
        {
            throw new ArgumentException(
                "The target expression must have a single parameter.",
                nameof(targetExpression));
        }

        if (sourceExpression.ReturnType != targetExpression.Parameters.Single().Type)
        {
            throw new ArgumentException(
                "The type of the result of the source expression does not match the type of the sole input parameter of the target expression.");
        }

        var parameterExpression = sourceExpression.Parameters.Single();

        var newSecondBody =
            new ReplaceExpressionVisitor(targetExpression.Parameters.Single(), sourceExpression.Body)
                .Visit(targetExpression.Body);

        var funcType = Expression.GetFuncType(parameterExpression.Type, targetExpression.ReturnType);

        var result = Expression.Lambda(funcType, newSecondBody, parameterExpression);
        return result;
    }

    /// <summary>
    ///     Represents the expression visitor that replaces one expression to another.
    /// </summary>
    private sealed class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _sourceExpression;
        private readonly Expression _targetExpression;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReplaceExpressionVisitor"/> class.
        /// </summary>
        /// <param name="sourceExpression">
        ///     The expression to replace with the target expression.
        /// </param>
        /// <param name="targetExpression">
        ///     The expression to replace the source expression with.
        /// </param>
        public ReplaceExpressionVisitor(Expression sourceExpression, Expression targetExpression)
        {
            _sourceExpression = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            _targetExpression = targetExpression ?? throw new ArgumentNullException(nameof(targetExpression));
        }

        /// <summary>
        ///     Dispatches the expression to one of the more specialized visit methods in this class.
        /// </summary>
        /// <param name="node">
        ///     The expression to visit.
        /// </param>
        /// <returns>
        ///     The modified expression, if it or any subexpression was modified;
        ///     otherwise, returns the original expression.
        /// </returns>
        [return: NotNullIfNotNull("node")]
        //// ReSharper disable once ReturnTypeCanBeNotNullable :: No nullability annotations in `ExpressionVisitor` in the older .NET versions
        public override Expression? Visit(Expression? node) => node == _sourceExpression ? _targetExpression : base.Visit(node);
    }
}