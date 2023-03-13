namespace Skyline.DataMiner.Utils.GetAfterSet.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error thrown when trying to access a Queue that has no values stored.
    /// </summary>
    [Serializable]
    public class QueueIsEmptyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueIsEmptyException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        public QueueIsEmptyException() : base("The queue your trying to access is empty.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueIsEmptyException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public QueueIsEmptyException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueIsEmptyException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public QueueIsEmptyException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueIsEmptyException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="info"> The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected QueueIsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
