namespace Omnifactotum.Validation.Constraints;

internal static class ValidationMessages
{
    public static readonly ValidationErrorDetails CannotBeNull = "The value cannot be null.";
    public static readonly ValidationErrorDetails StringCannotBeNullOrEmpty = "The value must not be null or an empty string.";
    public static readonly ValidationErrorDetails StringCannotBeEmpty = "The value must not be an empty string.";
    public static readonly ValidationErrorDetails StringCannotBeNullOrBlank = "The string value must not be null or blank.";
    public static readonly ValidationErrorDetails StringCannotBeBlank = "The string value must not be blank.";
    public static readonly ValidationErrorDetails CollectionCannotBeEmpty = "The collection cannot be empty.";
}