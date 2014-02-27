using System;
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
            var constraintCache = new Dictionary<Type, IMemberConstraint>();

            Factotum.ProcessRecursively(
                current,
                GetMembers,
                obj => ValidateInternal(instance, obj, constraintCache, validationErrors));

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

            var parentExpression = parentMemberData.Expression;

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
                            Attributes = obj.GetCustomAttributes<BaseMemberConstraintAttribute>(true)
                        })
                .Where(obj => obj.Attributes.Length != 0)
                .ToArray();

            var memberDatas = internalMemberDatas
                .Select(
                    obj => new MemberData(
                        Expression.MakeMemberAccess(parentExpression, obj.Member),
                        GetMemberValue(instance, obj.Member),
                        obj.Attributes))
                .ToList();

            //// TODO [vmcl] Support IEnumerable (not only 1-dimensional array)

            if (parentExpression.Type.IsArray && parentExpression.Type.GetArrayRank() == 1)
            {
                var array = (Array)instance;
                for (var index = 0; index < array.Length; index++)
                {
                    var item = array.GetValue(index);

                    var expression = Expression.ArrayIndex(
                        parentExpression,
                        Expression.Constant(index));

                    var itemData = new MemberData(expression, item, parentMemberData.Attributes);
                    memberDatas.Add(itemData);
                }
            }

            return memberDatas;
        }

        private static void ValidateInternal(
            object root,
            MemberData memberData,
            IDictionary<Type, IMemberConstraint> constraintCache,
            ICollection<MemberConstraintValidationError> outputErrors)
        {
            #region Argument Check

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (memberData == null)
            {
                throw new ArgumentNullException("memberData");
            }

            if (constraintCache == null)
            {
                throw new ArgumentNullException("constraintCache");
            }

            if (outputErrors == null)
            {
                throw new ArgumentNullException("outputErrors");
            }

            #endregion

            if (memberData.Attributes == null || memberData.Attributes.Length == 0)
            {
                return;
            }

            BaseMemberConstraintAttribute[] attributes;
            switch (memberData.Expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    attributes = memberData.Attributes.Where(obj => obj is MemberConstraintAttribute).ToArray();
                    break;

                case ExpressionType.ArrayIndex:
                    attributes = memberData.Attributes.Where(obj => obj is MemberItemConstraintAttribute).ToArray();
                    break;

                default:
                    throw memberData.Expression.NodeType.CreateEnumValueNotSupportedException();
            }

            foreach (var constraintAttribute in attributes)
            {
                var constraint = constraintCache.GetValueOrCreate(
                    constraintAttribute.ConstraintType,
                    constraintType => (IMemberConstraint)Activator.CreateInstance(constraintType));

                var context = new MemberConstraintValidationContext(root, memberData.Expression);

                var validationError = constraint.Validate(context, memberData.Value);
                if (validationError != null)
                {
                    outputErrors.Add(validationError);
                }
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
            /// <param name="attributes">
            ///     The constraint attributes.
            /// </param>
            internal MemberData(
                [NotNull] Expression expression,
                [CanBeNull] object value,
                [CanBeNull] BaseMemberConstraintAttribute[] attributes)
            {
                #region Argument Check

                if (expression == null)
                {
                    throw new ArgumentNullException("expression");
                }

                #endregion

                this.Expression = expression;
                this.Value = value;
                this.Attributes = attributes;
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
            ///     Gets the constraint attributes.
            /// </summary>
            public BaseMemberConstraintAttribute[] Attributes
            {
                get;
                private set;
            }

            #endregion
        }

        #endregion
    }
}