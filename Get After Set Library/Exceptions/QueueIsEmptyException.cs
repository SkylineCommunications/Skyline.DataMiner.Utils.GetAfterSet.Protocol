using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Skyline.DataMiner.Utils.GetAfterSet.Exceptions
{
    [Serializable]
    public class QueueIsEmptyException : Exception
    {
        public QueueIsEmptyException() : base("The queue your trying to access is empty.") { }

        public QueueIsEmptyException(string message) : base(message) { }

        public QueueIsEmptyException(string message, Exception innerException) : base(message, innerException) { }

        protected QueueIsEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
