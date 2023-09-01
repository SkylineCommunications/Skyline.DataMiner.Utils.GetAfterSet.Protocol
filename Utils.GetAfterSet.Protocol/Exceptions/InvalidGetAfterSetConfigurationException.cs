namespace Utils.GetAfterSet.Protocol.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error thrown when a given configuration is not valid.
    /// </summary>
    [Serializable]
    public class InvalidGetAfterSetConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGetAfterSetConfigurationException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        public InvalidGetAfterSetConfigurationException() : base("The given GetAfterSetConfig object is not valid.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGetAfterSetConfigurationException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidGetAfterSetConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGetAfterSetConfigurationException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public InvalidGetAfterSetConfigurationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidGetAfterSetConfigurationException"/>, an error thrown when trying to access a Queue that has no values stored.
        /// </summary>
        /// <param name="info"> The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The System.Runtime.Serialization.StreamingContext that contains contextual information about the source or destination.</param>
        protected InvalidGetAfterSetConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
