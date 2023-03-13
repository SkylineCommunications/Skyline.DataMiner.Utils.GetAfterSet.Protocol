namespace Skyline.DataMiner.Utils.GetAfterSet.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class QueueIsEmptyException : Exception
    {
        public QueueIsEmptyException() : base("The queue your trying to access is empty.") { }

        public QueueIsEmptyException(string message) : base(message) { }

        public QueueIsEmptyException(string message, Exception innerException) : base(message, innerException) { }

        protected QueueIsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
