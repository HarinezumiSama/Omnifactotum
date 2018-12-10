using System;
using System.Linq.Expressions;
using System.Reflection;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    public static partial class Factotum
    {
        /// <summary>
        ///     Provides a convenient access to helper methods for the specified type.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type that the helper methods are provided for.
        /// </typeparam>
        public static class For<TObject>
        {
            /// <summary>
            ///     Gets the <see cref="MemberInfo"/> of the field or property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TMember">
            ///     The type of the field or property.
            /// </typeparam>
            /// <param name="memberGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Member</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="MemberInfo"/> containing information about the required field or property.
            /// </returns>
            [NotNull]
            public static MemberInfo GetFieldOrPropertyInfo<TMember>(
                [NotNull] Expression<Func<TObject, TMember>> memberGetterExpression)
            {
                var result = GetDataMemberInfo(memberGetterExpression);
                return result;
            }

            /// <summary>
            ///     Gets the <see cref="FieldInfo"/> of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="FieldInfo"/> containing information about the required field.
            /// </returns>
            [NotNull]
            public static FieldInfo GetFieldInfo<TField>(
                [NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(fieldGetterExpression);

                if (!(memberInfo is FieldInfo result))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            fieldGetterExpression),
                        nameof(fieldGetterExpression));
                }

                return result;
            }

            /// <summary>
            ///     Gets the name of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The name of the field specified by the lambda expression.
            /// </returns>
            [NotNull]
            public static string GetFieldName<TField>(
                [NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var fieldInfo = GetFieldInfo(fieldGetterExpression);
                return fieldInfo.Name;
            }

            /// <summary>
            ///     Gets the type-qualified name of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The name of the field in the following form: <c>SomeType.Field</c>.
            /// </returns>
            [NotNull]
            public static string GetQualifiedFieldName<TField>(
                [NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var fieldInfo = GetFieldInfo(fieldGetterExpression);
                return typeof(TObject).GetQualifiedName() + Type.Delimiter + fieldInfo.Name;
            }

            /// <summary>
            ///     Gets the <see cref="PropertyInfo"/> of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="PropertyInfo"/> containing information about the required property.
            /// </returns>
            [NotNull]
            public static PropertyInfo GetPropertyInfo<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(propertyGetterExpression);

                if (!(memberInfo is PropertyInfo result))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            propertyGetterExpression),
                        nameof(propertyGetterExpression));
                }

                return result;
            }

            /// <summary>
            ///     Gets the name of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The name of the property.
            /// </returns>
            [NotNull]
            public static string GetPropertyName<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var propertyInfo = GetPropertyInfo(propertyGetterExpression);
                return propertyInfo.Name;
            }

            /// <summary>
            ///     Gets the type-qualified name of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The name of the property in the following form: <c>SomeType.Property</c>.
            /// </returns>
            [NotNull]
            public static string GetQualifiedPropertyName<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var propertyInfo = GetPropertyInfo(propertyGetterExpression);
                return typeof(TObject).GetQualifiedName() + Type.Delimiter + propertyInfo.Name;
            }

            [NotNull]
            private static MemberInfo GetDataMemberInfo<TMember>(
                [NotNull] Expression<Func<TObject, TMember>> memberGetterExpression)
            {
                if (memberGetterExpression is null)
                {
                    throw new ArgumentNullException(nameof(memberGetterExpression));
                }

                var objectType = typeof(TObject);

                if (!(memberGetterExpression.Body is MemberExpression memberExpression)
                    || memberExpression.NodeType != ExpressionType.MemberAccess)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        nameof(memberGetterExpression));
                }

                var result = memberExpression.Member;

                var fieldInfo = memberExpression.Member as FieldInfo;
                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (fieldInfo is null && propertyInfo is null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        nameof(memberGetterExpression));
                }

                if (result.DeclaringType is null || !result.DeclaringType.IsAssignableFrom(objectType))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        nameof(memberGetterExpression));
                }

                if (memberExpression.Expression is null)
                {
                    if (propertyInfo != null)
                    {
                        var accessor = propertyInfo.GetGetMethod(true) ?? propertyInfo.GetSetMethod(true);
                        if (accessor is null || !accessor.IsStatic || result.ReflectedType != objectType)
                        {
                            throw new ArgumentException(
                                string.Format(
                                    InvalidExpressionMessageFormat,
                                    objectType.GetFullName(),
                                    memberGetterExpression),
                                nameof(memberGetterExpression));
                        }
                    }
                }
                else
                {
                    if (!(memberExpression.Expression is ParameterExpression parameterExpression)
                        || parameterExpression.NodeType != ExpressionType.Parameter
                        || parameterExpression.Type != typeof(TObject))
                    {
                        throw new ArgumentException(
                            string.Format(
                                InvalidExpressionMessageFormat,
                                objectType.GetFullName(),
                                memberGetterExpression),
                            nameof(memberGetterExpression));
                    }
                }

                return result;
            }
        }
    }
}