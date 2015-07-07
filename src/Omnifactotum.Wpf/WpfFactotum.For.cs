using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using Omnifactotum.Annotations;

namespace Omnifactotum.Wpf
{
    public static partial class WpfFactotum
    {
        /// <summary>
        ///     Provides a convenient access to helper methods for the specified type.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type that the helper methods are provided for.
        /// </typeparam>
        public static class For<TObject>
        {
            #region Constants and Fields

            private const string InconsistencyBetweenPropertyExpressionAndDeclaringObjectTypeMessage =
                @"Inconsistency between property expression and declaring object type.";

            #endregion

            #region Public Methods

            /// <summary>
            ///     Registers a dependency property using the specified property expression defining
            ///     the property information and using the specified property metadata and the value
            ///     validation callback for the property.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="propertyMetadata">
            ///     The property metadata for the dependency property, or <c>null</c> if not specified.
            /// </param>
            /// <param name="validateValueCallback">
            ///     The validate value callback, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property identifier used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para><paramref name="propertyMetadata"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para>
            ///         <paramref name="validateValueCallback"/> invalidated the default value
            ///         specified by <paramref name="propertyMetadata"/>.
            ///     </para>
            /// </exception>
            public static DependencyProperty RegisterDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] PropertyMetadata propertyMetadata,
                [CanBeNull] ValidateValueCallback validateValueCallback)
            {
                var propertyInfo = Factotum.For<TObject>.GetPropertyInfo(propertyGetterExpression);

                if (propertyInfo.DeclaringType != typeof(TObject))
                {
                    throw new ArgumentException(
                        InconsistencyBetweenPropertyExpressionAndDeclaringObjectTypeMessage,
                        "propertyGetterExpression");
                }

                return DependencyProperty.Register(
                    propertyInfo.Name,
                    propertyInfo.PropertyType,
                    propertyInfo.DeclaringType.EnsureNotNull(),
                    propertyMetadata,
                    validateValueCallback);
            }

            /// <summary>
            ///     Registers a dependency property using the specified property expression defining
            ///     the property information and using the specified property metadata.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="propertyMetadata">
            ///     The property metadata for the dependency property, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property identifier used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para><paramref name="propertyMetadata"/> is not valid.</para>
            /// </exception>
            public static DependencyProperty RegisterDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] PropertyMetadata propertyMetadata)
            {
                return RegisterDependencyProperty(propertyGetterExpression, propertyMetadata, null);
            }

            /// <summary>
            ///     Registers a dependency property using the specified property expression defining
            ///     the property information and using the specified value validation callback for
            ///     the property.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="validateValueCallback">
            ///     The validate value callback, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property identifier used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            /// </exception>
            public static DependencyProperty RegisterDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] ValidateValueCallback validateValueCallback)
            {
                return RegisterDependencyProperty(propertyGetterExpression, null, validateValueCallback);
            }

            /// <summary>
            ///     Registers a dependency property using the specified property expression defining
            ///     the property information.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <returns>
            ///     A dependency property identifier used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            /// </exception>
            public static DependencyProperty RegisterDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                return RegisterDependencyProperty(propertyGetterExpression, null, null);
            }

            /// <summary>
            ///     Registers a read-only dependency property using the specified property expression defining
            ///     the property information and using the specified property metadata and the value
            ///     validation callback for the property.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="propertyMetadata">
            ///     The property metadata for the dependency property, or <c>null</c> if not specified.
            /// </param>
            /// <param name="validateValueCallback">
            ///     The validate value callback, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property key used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para><paramref name="propertyMetadata"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para>
            ///         <paramref name="validateValueCallback"/> invalidated the default value
            ///         specified by <paramref name="propertyMetadata"/>.
            ///     </para>
            /// </exception>
            public static DependencyPropertyKey RegisterReadOnlyDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] PropertyMetadata propertyMetadata,
                [CanBeNull] ValidateValueCallback validateValueCallback)
            {
                var propertyInfo = Factotum.For<TObject>.GetPropertyInfo(propertyGetterExpression);

                if (propertyInfo.DeclaringType != typeof(TObject))
                {
                    throw new ArgumentException(
                        InconsistencyBetweenPropertyExpressionAndDeclaringObjectTypeMessage,
                        "propertyGetterExpression");
                }

                return DependencyProperty.RegisterReadOnly(
                    propertyInfo.Name,
                    propertyInfo.PropertyType,
                    propertyInfo.DeclaringType.EnsureNotNull(),
                    propertyMetadata,
                    validateValueCallback);
            }

            /// <summary>
            ///     Registers a read-only dependency property using the specified property expression defining
            ///     the property information and using the specified property metadata.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="propertyMetadata">
            ///     The property metadata for the dependency property, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property key used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            ///     <para>-or-</para>
            ///     <para><paramref name="propertyMetadata"/> is not valid.</para>
            /// </exception>
            public static DependencyPropertyKey RegisterReadOnlyDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] PropertyMetadata propertyMetadata)
            {
                return RegisterReadOnlyDependencyProperty(propertyGetterExpression, propertyMetadata, null);
            }

            /// <summary>
            ///     Registers a read-only dependency property using the specified property expression defining
            ///     the property information and using the value validation callback for the property.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <param name="validateValueCallback">
            ///     The validate value callback, or <c>null</c> if not specified.
            /// </param>
            /// <returns>
            ///     A dependency property key used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            /// </exception>
            public static DependencyPropertyKey RegisterReadOnlyDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression,
                [CanBeNull] ValidateValueCallback validateValueCallback)
            {
                return RegisterReadOnlyDependencyProperty(propertyGetterExpression, null, validateValueCallback);
            }

            /// <summary>
            ///     Registers a read-only dependency property using the specified property expression defining
            ///     the property information.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The property getter expression in the following form: <c>obj =&gt; obj.Property</c>.
            /// </param>
            /// <returns>
            ///     A dependency property key used to reference the dependency property later
            ///     for operations such as setting its value programmatically or obtaining metadata.
            /// </returns>
            /// <exception cref="ArgumentException">
            ///     <para><paramref name="propertyGetterExpression"/> is not valid.</para>
            /// </exception>
            public static DependencyPropertyKey RegisterReadOnlyDependencyProperty<TProperty>(
                [NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                return RegisterReadOnlyDependencyProperty(propertyGetterExpression, null, null);
            }

            #endregion
        }
    }
}