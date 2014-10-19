using System;
using System.Linq;
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
            #region Public Methods

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
                Expression<Func<TObject, TMember>> memberGetterExpression)
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
                Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(fieldGetterExpression);

                var result = memberInfo as FieldInfo;
                if (result == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            fieldGetterExpression),
                        "fieldGetterExpression");
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
                Expression<Func<TObject, TField>> fieldGetterExpression)
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
                Expression<Func<TObject, TField>> fieldGetterExpression)
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
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(propertyGetterExpression);

                var result = memberInfo as PropertyInfo;
                if (result == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            propertyGetterExpression),
                        "propertyGetterExpression");
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
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
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
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var propertyInfo = GetPropertyInfo(propertyGetterExpression);
                return typeof(TObject).GetQualifiedName() + Type.Delimiter + propertyInfo.Name;
            }

            #endregion

            #region Private Methods

            [NotNull]
            private static MemberInfo GetDataMemberInfo<TMember>(
                Expression<Func<TObject, TMember>> memberGetterExpression)
            {
                #region Argument Check

                if (memberGetterExpression == null)
                {
                    throw new ArgumentNullException("memberGetterExpression");
                }

                #endregion

                var objectType = typeof(TObject);

                var memberExpression = memberGetterExpression.Body as MemberExpression;
                if ((memberExpression == null) || (memberExpression.NodeType != ExpressionType.MemberAccess))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                var result = memberExpression.Member;

                var fieldInfo = memberExpression.Member as FieldInfo;
                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (fieldInfo == null && propertyInfo == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                if ((result.DeclaringType == null) || !result.DeclaringType.IsAssignableFrom(objectType))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                if (memberExpression.Expression == null)
                {
                    if (propertyInfo != null)
                    {
                        var accessor = propertyInfo.GetGetMethod(true) ?? propertyInfo.GetSetMethod(true);
                        if ((accessor == null) || !accessor.IsStatic || (result.ReflectedType != objectType))
                        {
                            throw new ArgumentException(
                                string.Format(
                                    InvalidExpressionMessageFormat,
                                    objectType.GetFullName(),
                                    memberGetterExpression),
                                "memberGetterExpression");
                        }
                    }
                }
                else
                {
                    var parameterExpression = memberExpression.Expression as ParameterExpression;
                    if ((parameterExpression == null) || (parameterExpression.NodeType != ExpressionType.Parameter) ||
                        (parameterExpression.Type != typeof(TObject)))
                    {
                        throw new ArgumentException(
                            string.Format(
                                InvalidExpressionMessageFormat,
                                objectType.GetFullName(),
                                memberGetterExpression),
                            "memberGetterExpression");
                    }
                }

                return result;
            }

            #endregion
        }
    }
}