namespace Skyline.DataMiner.Utils.GetAfterSet
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// Class that contains information about what a certain parameter should be.
    /// </summary>
    [System.Serializable]
    public class GetAfterSetConfig
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetConfig"/> class.
        /// This is used for the json serialization and should never be used otherwise.
        /// </summary>
        public GetAfterSetConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetConfig"/> class.
        /// Creates a configuration that know what a specific parameter's value should be and what he can do if it isn't the desired value.
        /// </summary>
        /// <param name="parameterPid">The PID of the parameter you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        public GetAfterSetConfig(int parameterPid, int triggerId, object desiredValue)
        {
            this.ParameterID = parameterPid;
            this.TriggerId = triggerId;
            this.DesiredValue = desiredValue;
            this.IsTableParameter = false;

            this.TableId = -1;
            this.RowKey = null;
            this.ColumnIdx = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAfterSetConfig"/> class.
        /// Creates a configuration that know what a specific cell in a table's value should be and what he can do if it isn't the desired value.
        /// </summary>
        /// <param name="tablePid">The PID of the table the cell you want to check is in.</param>
        /// <param name="rowKey">The primary key of the row the cell you want to check is in.</param>
        /// <param name="columnIdx">The column of the cell you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific cell/row/table.</param>
        /// <param name="desiredValue">The desired value the cell should be.</param>
        public GetAfterSetConfig(int tablePid, string rowKey, int columnIdx, int triggerId, object desiredValue)
        {
            this.TableId = tablePid;
            this.RowKey = rowKey;
            this.ColumnIdx = columnIdx;
            this.TriggerId = triggerId;
            this.DesiredValue = desiredValue;
            this.IsTableParameter = true;

            this.ParameterID = -1;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the tablePiD or -1 if the config is about a stand alone parameter.
        /// </summary>
        [JsonProperty("table_id")]
        public int TableId { get; private set; }

        /// <summary>
        /// Gets the ColumnIdx or -1 if the config is about a stand alone parameter.
        /// </summary>
        [JsonProperty("column_idx")]
        public int ColumnIdx { get; private set; }

        /// <summary>
        /// Gets the RowKey or <see langword="null"/> if the config is about a standalone parameter.
        /// </summary>
        [JsonProperty("row_key")]
        public string RowKey { get; private set; }

        /// <summary>
        /// Gets the ParameterId or -1 if the config is about a table cell.
        /// </summary>
        [JsonProperty("parameter_id")]
        public int ParameterID { get; private set; }

        /// <summary>
        /// Gets the desired value the stand alone parameter or table cell should be.
        /// </summary>
        [JsonProperty("desired_value")]
        public object DesiredValue { get; private set; }

        /// <summary>
        /// Gets the TriggerPID used to re-poll the standalone parameter or table cell in case it isn't yet the desired value.
        /// </summary>
        [JsonProperty("trigger_id")]
        public int TriggerId { get; private set; }

        /// <summary>
        /// Gets a boolean indicating whether the config is about a standalone parameter or a table cell.
        /// <para><see langword="true"/>: it is about a table cell.</para>
        /// <para><see langword="false"/>: it is about a standalone parameter.</para>
        /// </summary>
        [JsonProperty("is_table_parameter")]
        public bool IsTableParameter { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// Loads a new <see cref="GetAfterSetConfig"/> based of the json that is stored in the given parameter.
        /// </summary>
        /// <param name="protocol"><see cref="SLProtocol" /> instance used to communicate with DataMiner.</param>
        /// <param name="parameterId">The parameterPID where the serialized json config is stored.</param>
        /// <returns>Returns a new <see cref="GetAfterSetConfig"/> based of the json that is in the given parameter.</returns>
        public static GetAfterSetConfig LoadConfigFromParameter(SLProtocol protocol, int parameterId)
        {
            string json = Convert.ToString(protocol.GetParameter(parameterId));
            return LoadConfigFromString(json);
        }

        /// <summary>
        /// Loads a new <see cref="GetAfterSetConfig"/> based on the json.
        /// </summary>
        /// <param name="json">The json string representing the serialized json config.</param>
        /// <returns>Returns a new <see cref="GetAfterSetConfig"/> on the json.</returns>
        /// <exception cref="ArgumentException">When json string is <see langword="null"/> or whitespace.</exception>
        public static GetAfterSetConfig LoadConfigFromString(string json)
        {
            if (String.IsNullOrWhiteSpace(json)) throw new ArgumentException("Cannot parse and empty json string.");
            var data = (JObject)JsonConvert.DeserializeObject(json);
            if (data["is_table_parameter"].Value<bool>())
            {
                return new GetAfterSetConfig(
                    data["table_id"].Value<int>(),
                    data["row_key"].Value<string>(),
                    data["column_idx"].Value<int>(),
                    data["trigger_id"].Value<int>(),
                    data["desired_value"].Value<object>());
            }
            else
            {
                return new GetAfterSetConfig(
                    data["parameter_id"].Value<int>(),
                    data["trigger_id"].Value<int>(),
                    data["desired_value"].Value<object>());
            }
        }

        /// <summary>
        /// Checks if the standalone parameter/table cell is the same as the desired value.
        /// </summary>
        /// <param name="protocol"><see cref="SLProtocol" /> instance used to communicate with DataMiner.</param>
        /// <returns>
        /// <para><see langword="true"/>: standalone parameter/table cell equals the desired value.</para>
        /// <para><see langword="false"/>: standalone parameter/table cell doesn't equals the desired value.</para>
        /// </returns>
        public bool RunCheck(SLProtocol protocol)
        {
            if (Convert.ToString(GetValue(protocol)) == Convert.ToString(DesiredValue))
            {
                return true;
            }
            else
            {
                protocol.CheckTrigger(TriggerId);
                return false;
            }
        }

        /// <summary>
        /// Serializes the current <see cref="GetAfterSetConfig"/>.
        /// </summary>
        /// <returns>A json string representing the current <see cref="GetAfterSetConfig"/>.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        private object GetValue(SLProtocol protocol)
        {
            if (IsTableParameter)
            {
                // To avoid having an ugly log message, saying the row could not be found when we in fact want it that way.
                if (protocol.Exists(TableId, RowKey))
                {
                    return protocol.GetParameterIndexByKey(TableId, RowKey, ColumnIdx + 1);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return protocol.GetParameter(ParameterID);
            }
        }
        #endregion
    }
}

