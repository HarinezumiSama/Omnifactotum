namespace Omnifactotum.Abstractions
{
    /// <summary>
    ///     Represents the interface of a mutable container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    public interface IValueContainer<T>
    {
        /// <summary>
        ///     Gets or sets the encapsulated value.
        /// </summary>
        T Value { get; set; }
    }
}