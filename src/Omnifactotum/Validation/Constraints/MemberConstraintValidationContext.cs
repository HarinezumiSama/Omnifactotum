using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Represents the context of member constraint validation.
    /// </summary>
    public sealed class MemberConstraintValidationContext
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberConstraintValidationContext"/> class.
        /// </summary>
        /// <param name="root">
        ///     The root object that is being, or was, checked.
        /// </param>
        /// <param name="expression">
        ///     The lambda expression describing the path to the value from the root object.
        /// </param>
        internal MemberConstraintValidationContext([NotNull] object root, [NotNull] MemberExpression expression)
        {
            #region Argument Check

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            #endregion

            this.Root = root;
            this.Expression = expression;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the root object that is being, or was, checked.
        /// </summary>
        public object Root
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the lambda expression describing the path to the value from the root object.
        /// </summary>
        public MemberExpression Expression
        {
            get;
            private set;
        }

        #endregion
    }
}