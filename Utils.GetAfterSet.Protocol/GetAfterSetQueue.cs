namespace Skyline.DataMiner.Utils.GetAfterSet
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.GetAfterSet.Exceptions;

    /// <summary>
    /// Class that contains a queue of <see cref="GetAfterSetConfig"/>'s. You can enqueue and dequeue item from it. It has the extra feature of a dequeue that checks if the <see cref="GetAfterSetConfig"/> was completed.
    /// </summary>
    public class GetAfterSetQueue : IEnumerable<GetAfterSetConfig>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetQueue"/> class.
        /// Created a queue of <see cref="GetAfterSetConfig"/>'s that is used to repoll a standalone parameter/table cell in case it is not yet the desired value.
        /// </summary>
        /// <param name="request">the <see cref="GetAfterSetConfig"/> that this queue needs to contains.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        public GetAfterSetQueue(GetAfterSetConfig request, int retries)
        {
            BaseRequest = request;
            Count = retries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetQueue"/> class.
        /// Created a queue of <see cref="GetAfterSetConfig"/>'s that is used to repoll a standalone parameter/table cell in case it is not yet the desired value.
        /// </summary>
        /// <param name="parameterPid">The PID of the parameter you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        public GetAfterSetQueue(int parameterPid, int triggerId, object desiredValue, int retries)
        {
            BaseRequest = new GetAfterSetConfig(parameterPid, triggerId, desiredValue);
            Count = retries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetQueue"/> class.
        /// Created a queue of <see cref="GetAfterSetConfig"/>'s that is used to repoll a standalone parameter/table cell in case it is not yet the desired value.
        /// </summary>
        /// <param name="tablePid">The PID of the table the cell you want to check is in.</param>
        /// <param name="rowKey">The primary key of the row the cell you want to check is in.</param>
        /// <param name="columnIdx">The column of the cell you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific cell/row/table.</param>
        /// <param name="desiredValue">The desired value the cell should be.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        public GetAfterSetQueue(int tablePid, string rowKey, int columnIdx, int triggerId, object desiredValue, int retries)
        {
            BaseRequest = new GetAfterSetConfig(tablePid, rowKey, columnIdx, triggerId, desiredValue);
            Count = retries;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="GetAfterSetConfig"/> linked to this queue.
        /// </summary>
        public GetAfterSetConfig BaseRequest { get; private set; }

        /// <summary>
        /// Gets how many retries are left.
        /// </summary>
        public int Count { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// Loads a new <see cref="GetAfterSetQueue"/> based of the json that is stored in the given parameter.
        /// </summary>
        /// <param name="protocol"><see cref="SLProtocol" /> instance used to communicate with DataMiner.</param>
        /// <param name="parameterId">The parameterPID where the serialized json queue is stored.</param>
        /// <returns>Returns a new <see cref="GetAfterSetQueue"/> based of the json that is in the given parameter.</returns>
        /// <exception cref="JsonException">When json string is <see langword="null"/> or whitespace.</exception>
        public static GetAfterSetQueue LoadRequestQueueFromParameter(SLProtocol protocol, int parameterId)
        {
            var json = Convert.ToString(protocol.GetParameter(parameterId));
            return LoadRequestQueueFromString(json);
        }

        /// <summary>
        /// Loads a new <see cref="GetAfterSetQueue"/> based on the json.
        /// </summary>
        /// <param name="json">The json string representing the serialized json request.</param>
        /// <returns>Returns a new <see cref="GetAfterSetQueue"/> on the json.</returns>
        /// <exception cref="JsonException">When json string is <see langword="null"/> or whitespace.</exception>
        public static GetAfterSetQueue LoadRequestQueueFromString(string json)
        {
            if (String.IsNullOrWhiteSpace(json)) throw new JsonException("Cannot parse and empty json string.");
            var list = (JObject)JsonConvert.DeserializeObject(json);
            var length = list["length"].Value<int>();
            var request = JsonConvert.DeserializeObject<GetAfterSetConfig>(Convert.ToString(list["request"]));
            return new GetAfterSetQueue(request, length);
        }

        /// <summary>
        /// Removes the <see cref="GetAfterSetConfig"/> at the head of the queue and runs a check on it. If the queue is empty, this will throw a <see cref="QueueIsEmptyException"/>
        /// </summary>
        /// <param name="protocol"><see cref="SLProtocol" /> instance used to communicate with DataMiner.</param>
        /// <returns>
        /// <para><see langword="true"/>: If the standalone parameter/table cell equals the desired value.</para>
        /// <para><see langword="false"/>: If the standalone parameter/table cell doesn't equals the desired value.</para>
        /// </returns>
        /// <exception cref="QueueIsEmptyException"></exception>
        public bool DequeueWithCheck(SLProtocol protocol)
        {
            var request = Dequeue();
            if (request == null) return false;
            if (request.RunCheck(protocol))
            {
                Clear();
                return true;
            }

            if (Count <= 0)
            {
                protocol.Log("QA" + protocol.QActionID + "|" + this.GetType().Name + "|Failed to get the desired value.", LogType.Error, LogLevel.NoLogging);
            }

            return false;
        }

        /// <summary>
        /// Serializes the current <see cref="GetAfterSetQueue"/>.
        /// </summary>
        /// <returns>A json string representing the current <see cref="GetAfterSetQueue"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            using (JsonWriter writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.Formatting = Formatting.None;

                writer.WriteStartObject();
                writer.WritePropertyName("length");
                writer.WriteValue(Count);
                writer.WritePropertyName("request");
                writer.WriteRawValue(JsonConvert.SerializeObject(BaseRequest));
                writer.WriteEndObject();
            }

            return sb.ToString();
        }
        #endregion

        #region Queue Methods

        /// <summary>
        /// Removes the <see cref="GetAfterSetConfig"/> at the head of the queue and returns it. If the queue is empty, this will throw a <see cref="QueueIsEmptyException"/>.
        /// </summary>
        /// <returns>The <see cref="GetAfterSetConfig"/> at the head of the queue.</returns>
        /// <exception cref="QueueIsEmptyException"></exception>
        public GetAfterSetConfig Dequeue()
        {
            if (Count == 0) throw new QueueIsEmptyException();
            Count--;
            return BaseRequest;
        }

        /// <summary>
        /// Adds a <see cref="GetAfterSetConfig"/> to the tail of the queue.
        /// </summary>
        public void Enqueue()
        {
            Count++;
        }

        /// <summary>
        /// Removes all <see cref="GetAfterSetConfig"/>'s from the queue.
        /// </summary>
        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Returns the <see cref="GetAfterSetConfig"/> at the specific index. The <see cref="GetAfterSetConfig"/> remains in the queue. If the queue is empty or the given index is not valid, this method throw an exception.
        /// </summary>
        /// <param name="i">The index of the element.</param>
        /// <returns>The <see cref="GetAfterSetConfig"/> at the given index.</returns>
        /// <exception cref="QueueIsEmptyException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public GetAfterSetConfig GetElement(int i)
        {
            if (Count == 0) throw new QueueIsEmptyException();
            if (i < 0) throw new ArgumentOutOfRangeException(nameof(i), i, "The index cannot be negative.");
            if (Count <= i) throw new ArgumentOutOfRangeException(nameof(i), i, "The index is out of range");
            return Peek();
        }

        /// <summary>
        /// Returns the <see cref="GetAfterSetConfig"/> at the head of the queue. The <see cref="GetAfterSetConfig"/> remains in the queue. If the queue is empty this method throws a <see cref="QueueIsEmptyException"/>.
        /// </summary>
        /// <returns>The <see cref="GetAfterSetConfig"/> at the head of the queue.</returns>
        /// <exception cref="QueueIsEmptyException"></exception>
        public GetAfterSetConfig Peek()
        {
            if (Count == 0) throw new QueueIsEmptyException();

            return BaseRequest;
        }

        /// <summary>
        /// Iterates over the <see cref="GetAfterSetConfig"/> in the queue, returning and array of the <see cref="GetAfterSetConfig"/> in the queue.
        /// </summary>
        /// <returns>An array representation of the <see cref="GetAfterSetQueue"/>.</returns>
        public GetAfterSetConfig[] ToArray()
        {
            var array = new GetAfterSetConfig[Count];
            for (int i = 0; i < Count; i++)
            {
                array[i] = BaseRequest;
            }

            return array;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<GetAfterSetConfig> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return BaseRequest;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
