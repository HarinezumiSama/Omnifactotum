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

            var rootMemberData = new MemberData(parameterExpression, null, instance, null, null);
            var constraintCache = new Dictionary<Type, IMemberConstraint>();

            Factotum.ProcessRecursively(
                rootMemberData,
                GetMembers,
                obj => ValidateInternal(instance, parameterExpression, obj, constraintCache, validationErrors));

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

        private static BaseMemberConstraintAttribute[] FilterBy<TAttribute>(
            this IEnumerable<BaseValidatableMemberAttribute> attributes)
            where TAttribute : BaseMemberConstraintAttribute
        {
            return attributes == null
                ? null
                : attributes.Where(obj => obj is TAttribute).Cast<BaseMemberConstraintAttribute>().ToArray();
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

        private static Expression ConvertTypeAuto(Expression expression, object value)
        {
            if (value == null)
            {
                return expression;
            }

            var valueType = value.GetType();
            var expressionType = expression.Type.GetCollectionElementType() ?? expression.Type;

            return expressionType == valueType ? expression : Expression.Convert(expression, valueType);
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
                            Attributes = obj.GetCustomAttributes<BaseValidatableMemberAttribute>(true)
                        })
                .Where(obj => obj.Attributes.Length != 0)
                .ToArray();

            var memberDatas = internalMemberDatas
                .Select(
                    obj => new MemberData(
                        Expression.MakeMemberAccess(ConvertTypeAuto(parentExpression, instance), obj.Member),
                        instance,
                        GetMemberValue(instance, obj.Member),
                        obj.Attributes,
                        obj.Attributes.FilterBy<MemberConstraintAttribute>()))
                .ToList();

            //// TODO [vmcl] Support IEnumerable (not only 1-dimensional array)

            if (parentExpression.Type.IsArray && parentExpression.Type.GetArrayRank() == 1)
            {
                var array = (Array)instance;
                for (var index = 0; index < array.Length; index++)
                {
                    var item = array.GetValue(index);

                    var expression = ConvertTypeAuto(
                        Expression.ArrayIndex(parentExpression, Expression.Constant(index)),
                        item);

                    var itemData = new MemberData(
                        expression,
                        instance,
                        item,
                        parentMemberData.Attributes,
                        parentMemberData.Attributes.FilterBy<MemberItemConstraintAttribute>());

                    memberDatas.Add(itemData);
                }
            }

            return memberDatas;
        }

        private static void ValidateInternal(
            object root,
            ParameterExpression parameterExpression,
            MemberData memberData,
            IDictionary<Type, IMemberConstraint> constraintCache,
            ICollection<MemberConstraintValidationError> outputErrors)
        {
            #region Argument Check

            root.EnsureNotNull();
            parameterExpression.EnsureNotNull();
            memberData.EnsureNotNull();
            constraintCache.EnsureNotNull();
            outputErrors.EnsureNotNull();

            #endregion

            var effectiveAttributes = memberData.EffectiveAttributes;
            if (effectiveAttributes == null || effectiveAttributes.Length == 0)
            {
                return;
            }

            foreach (var constraintAttribute in effectiveAttributes)
            {
                var constraint = constraintCache.GetValueOrCreate(
                    constraintAttribute.ConstraintType,
                    constraintType => (IMemberConstraint)Activator.CreateInstance(constraintType));

                var context = new MemberConstraintValidationContext(
                    root,
                    memberData.Container,
                    memberData.Expression,
                    Expression.Lambda(memberData.Expression, parameterExpression));

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
            /// <param name="container">
            ///     The object containing the value that is being validated. Can be <b>null</b>.
            /// </param>
            /// <param name="value">
            ///     The member value.
            /// </param>
            /// <param name="attributes">
            ///     The constraint attributes.
            /// </param>
            /// <param name="effectiveAttributes">
            ///     The effective constraint attributes.
            /// </param>
            internal MemberData(
                [NotNull] Expression expression,
                [CanBeNull] object container,
                [CanBeNull] object value,
                [CanBeNull] BaseValidatableMemberAttribute[] attributes,
                [CanBeNull] BaseMemberConstraintAttribute[] effectiveAttributes)
            {
                #region Argument Check

                if (expression == null)
                {
                    throw new ArgumentNullException("expression");
                }

                #endregion

                this.Expression = expression;
                this.Container = container;
                this.Value = value;
                this.Attributes = attributes;
                this.EffectiveAttributes = effectiveAttributes;
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
            ///     Gets the object containing the value that is being, or was, validated.
            /// </summary>
            public object Container
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
            public BaseValidatableMemberAttribute[] Attributes
            {
                get;
                private set;
            }

            /// <summary>
            ///     Gets the effective constraint attributes.
            /// </summary>
            public BaseMemberConstraintAttribute[] EffectiveAttributes
            {
                get;
                private set;
            }

            #endregion
        }

        #endregion
    }
}