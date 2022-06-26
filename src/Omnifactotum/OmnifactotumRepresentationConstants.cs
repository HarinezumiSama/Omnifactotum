#nullable enable

namespace Omnifactotum
{
    /// <summary>
    ///     Contains the constant string values used to render the UI representation of objects.
    /// </summary>
    public static class OmnifactotumRepresentationConstants
    {
        /// <summary>
        ///     The UI representation of the <see langword="null"/> value.
        /// </summary>
        public const string NullValueRepresentation = @"null";

        /// <summary>
        ///     The UI representation of the <see langword="null"/> collection.
        /// </summary>
        internal const string NullCollectionRepresentation = @"<" + NullValueRepresentation + @">";

        /// <summary>
        ///     The string value used to separate items in the collection in its UI representation.
        /// </summary>
        internal const string CollectionItemSeparator = @", ";
    }
}