using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Provides functionality to recursively validate objects annotated
    ///     with <see cref="MemberConstraintAttribute"/>.
    /// </summary>
    public static class ObjectValidator
    {
        #region Constants and Fields

        /// <summary>
        ///     The root object parameter name (used in expressions).
        /// </summary>
        internal const string RootObjectParameterName = "instance";

        #endregion

        #region Public Methods

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
        public static ObjectValidationResult Validate<T>(T instance)
        {
            #region Argument Check

            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }

            #endregion

            var parameterExpression = Expression.Parameter(instance.GetType(), RootObjectParameterName);
            var validationErrors = new List<MemberConstraintValidationError>();

            var current = new MemberData(parameterExpression, instance, null);

            Factotum.ProcessRecursively(
                current,
                GetMembers,
                obj => ValidateInternal(instance, obj, validationErrors));

            return new ObjectValidationResult(validationErrors);
        }

        #endregion

        #region Private Methods

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
            #region Argument Check

            if (parentMemberData == null)
            {
                throw new ArgumentNullException("parentMemberData");
            }

            #endregion

            var instance = parentMemberData.Value;
            var instanceType = instance.GetTypeSafely();

            if (instance == null || IsSimpleTypeInternal(instanceType))
            {
                return Enumerable.Empty<MemberData>();
            }

            //// TODO [vmaklai] Cache type's validatable members

            var allDataMembers = instanceType.FindMembers(
                MemberTypes.Field | MemberTypes.Property,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                (info, criteria) => info is FieldInfo || IsReadableProperty(info),
                null);

            var internalMemberDatas = allDataMembers
                .Select(
                    obj =>
                        new
                        {
                            Member = obj,
                            Attribute = obj.GetSingleOrDefaultCustomAttribute<MemberConstraintAttribute>()
                        })
                .Where(obj => obj.Attribute != null)
                .ToArray();

            var memberDatas = internalMemberDatas
                .Select(
                    obj => new MemberData(
                        Expression.MakeMemberAccess(parentMemberData.Expression, obj.Member),
                        GetMemberValue(instance, obj.Member),
                        obj.Attribute))
                .ToArray();

            return memberDatas;
        }

        private static void ValidateInternal(
            object root,
            MemberData memberData,
            ICollection<MemberConstraintValidationError> outputErrors)
        {
            #region Argument Check

            if (memberData == null)
            {
                throw new ArgumentNullException("memberData");
            }

            if (outputErrors == null)
            {
                throw new ArgumentNullException("outputErrors");
            }

            #endregion

            var memberExpression = memberData.Expression as MemberExpression;
            if (memberExpression == null)
            {
                return;
            }

            //// TODO [vmaklai] Cache constraint instances
            var constraint = (IMemberConstraint)Activator.CreateInstance(memberData.Attribute.ConstraintType);

            var context = new MemberConstraintValidationContext(root, memberExpression);

            var validationError = constraint.Validate(context, memberData.Value);
            if (validationError != null)
            {
                outputErrors.Add(validationError);
            }
        }

        #endregion

        #region MemberData Class

        /// <summary>
        ///     Represents the member data.
        /// </summary>
        private sealed class MemberData
        {
            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="MemberData"/> class.
            /// </summary>
            /// <param name="expression">
            ///     The expression.
            /// </param>
            /// <param name="value">
            ///     The member value.
            /// </param>
            /// <param name="attribute">
            ///     The constraint attribute.
            /// </param>
            internal MemberData(
                [NotNull] Expression expression,
                [CanBeNull] object value,
                [CanBeNull] MemberConstraintAttribute attribute)
            {
                #region Argument Check

                if (expression == null)
                {
                    throw new ArgumentNullException("expression");
                }

                #endregion

                this.Expression = expression;
                this.Value = value;
                this.Attribute = attribute;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the expression.
            /// </summary>
            public Expression Expression
            {
                get;
                private set;
            }

            /// <summary>
            ///     Gets the value.
            /// </summary>
            public object Value
            {
                get;
                private set;
            }

            /// <summary>
            ///     Gets the attribute.
            /// </summary>
            public MemberConstraintAttribute Attribute
            {
                get;
                private set;
            }

            #endregion
        }

        #endregion
    }
}