namespace Omnifactotum.Validation;

public sealed partial class ValidationErrorDetails
{
    /// <summary>
    ///     Contains the predefined <see cref="ValidationErrorDetails"/> instances.
    /// </summary>
    public static class Predefined
    {
        /// <summary>
        ///     The value must not be <see langword="null"/>.
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.NotNullConstraint"/>
        public static readonly ValidationErrorDetails MustNotBeNull = "The value must not be null.";

        /// <summary>
        ///     The string value must not be <see langword="null"/> or <see cref="string.Empty"/>.
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.NotNullAndNotEmptyStringConstraint"/>
        public static readonly ValidationErrorDetails StringMustNotBeNullOrEmpty = "The string value must not be null or empty.";

        /// <summary>
        ///     The string value may be <see langword="null"/>, but otherwise must not be <see cref="string.Empty"/>.
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.OptionalNotEmptyStringConstraint"/>
        public static readonly ValidationErrorDetails StringMustNotBeEmpty = "The string value, when not null, must not be empty.";

        /// <summary>
        ///     The string value must not be <see langword="null"/> or blank
        ///     (that is, must not be <see cref="string.Empty"/> and must not consist only of whitespace characters).
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.NotNullAndNotBlankStringConstraint"/>
        public static readonly ValidationErrorDetails StringMustNotBeNullOrBlank = "The string value must not be null or blank.";

        /// <summary>
        ///     The string value may be <see langword="null"/>, but otherwise must not be blank
        ///     (that is, must not be <see cref="string.Empty"/> and must not consist only of whitespace characters).
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.OptionalNotBlankStringConstraint"/>
        public static readonly ValidationErrorDetails StringMustNotBeBlank = "The string value, when not null, must not be blank.";

        /// <summary>
        ///     The collection must not be <see langword="null"/> or empty (that is, the collection must contain at least one item).
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.NotNullAndNotEmptyCollectionConstraint"/>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.NotNullAndNotEmptyCollectionConstraint`1"/>
        public static readonly ValidationErrorDetails CollectionMustNotBeNullOrEmpty = "The collection must not be null or empty.";

        /// <summary>
        ///     The collection may be <see langword="null"/>, but otherwise must not be empty (that is, the collection must contain at least one item).
        /// </summary>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.OptionalNotEmptyCollectionConstraint"/>
        /// <seealso cref="T:Omnifactotum.Validation.Constraints.OptionalNotEmptyCollectionConstraint`1"/>
        public static readonly ValidationErrorDetails CollectionMustNotBeEmpty = "The collection, when not null, must not be empty.";
    }
}