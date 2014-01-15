using System;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the mutable container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    [Serializable]
    public sealed class ValueContainer<T>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        public ValueContainer(T value)
        {
            this.Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueContainer{T}"/> class
        ///     using the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public ValueContainer()
            : this(default(T))
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the contained value.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        #endregion
    }
}