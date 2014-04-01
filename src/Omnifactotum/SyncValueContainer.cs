using System;
using System.Diagnostics;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the mutable thread-safe container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    [Serializable]
    public sealed class SyncValueContainer<T>
    {
        #region Constants and Fields

        private T _value;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}" /> class
        ///     using the specified value and synchronization object.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <param name="syncObject">
        ///     The synchronization object used for thread-safe access.
        ///     Cannot be <b>null</b> and must be of a reference type.
        /// </param>
        public SyncValueContainer(T value, object syncObject)
        {
            #region Argument Check

            if (syncObject == null)
            {
                throw new ArgumentNullException("syncObject");
            }

            if (syncObject.GetType().IsValueType)
            {
                throw new ArgumentException("The synchronization object cannot be a value type object.");
            }

            #endregion

            this.SyncObject = syncObject;
            _value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        public SyncValueContainer(T value)
            : this(value, new object())
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public SyncValueContainer()
            : this(default(T))
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the synchronization object used for thread-safe access.
        /// </summary>
        public object SyncObject
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the contained value.
        /// </summary>
        public T Value
        {
            [DebuggerNonUserCode]
            get
            {
                lock (this.SyncObject)
                {
                    return _value;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (this.SyncObject)
                {
                    _value = value;
                }
            }
        }

        #endregion
    }
}