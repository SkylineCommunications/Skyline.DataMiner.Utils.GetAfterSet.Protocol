namespace Utils.GetAfterSet.Protocol
{
    using System;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.GetAfterSet;
    using Utils.GetAfterSet.Protocol.Exceptions;

    public static class Extensions
    {
        /// <summary>
        /// Sets the parameter with the specified ID to the specified value, with an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="config">The set configuration. Including which parameter, the value, the number of retries.</param>
        /// <param name="addBufferPid">The parameter holding the check requests.</param>
        /// <returns>HRESULT value. A value of 0 (S_OK) indicates the set succeeded.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • In case multiple parameters need to be set, it is preferred to use a single
        /// SetParameters method call in order to reduce the inter-process communication
        /// between the SLScripting and SLProtocol processes.
        /// </para>
        /// <para>
        /// • The method SetParameter(int parameterID, object value, DateTime timestamp)
        /// acts a wrapper method for a NotifyProtocol type 256 NT_SET_PARAMETER_WITH_HISTORY call.
        /// </para>
        /// </remarks>
        public static int SetParameter(this SLProtocol protocol, GetAfterSetQueue config, int addBufferPid)
        {
            if (config.BaseRequest.IsTableParameter)
                throw new InvalidGetAfterSetConfigurationException("The given GetAfterSetConfig object is invalid. Trying to set a standalone parameter with a table parameter config.");

            var result = protocol.SetParameter(config.BaseRequest.ParameterID, config.BaseRequest.DesiredValue);
            protocol.SetParameter(addBufferPid, Convert.ToString(config));
            return result;
        }

        /// <summary>
        /// Sets the parameter with the specified ID to the specified value, with an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="config">The set configuration. Including which parameter, the value, the number of retries.</param>
        /// <param name="addBufferPid">The parameter holding the check requests.</param>
        /// <param name="timeInfo">The timestamp.</param>
        /// <returns>HRESULT value. A value of 0 (S_OK) indicates the set succeeded.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • In case multiple parameters need to be set, it is preferred to use a single
        /// SetParameters method call in order to reduce the inter-process communication
        /// between the SLScripting and SLProtocol processes.
        /// </para>
        /// <para>
        /// • The method SetParameter(int parameterID, object value, DateTime timestamp)
        /// acts a wrapper method for a NotifyProtocol type 256 NT_SET_PARAMETER_WITH_HISTORY call.
        /// </para>
        /// </remarks>
        public static int SetParameter(this SLProtocol protocol, GetAfterSetQueue config, int addBufferPid, ValueType timeInfo)
        {
            if (config.BaseRequest.IsTableParameter)
                throw new InvalidGetAfterSetConfigurationException("The given GetAfterSetConfig object is invalid. Trying to set a standalone parameter with a table parameter config.");

            var result = protocol.SetParameter(config.BaseRequest.ParameterID, config.BaseRequest.DesiredValue, timeInfo);
            protocol.SetParameter(addBufferPid, Convert.ToString(config), timeInfo);
            return result;
        }

        /// <summary>
        /// Sets the parameter with the specified ID to the specified value, with an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="parameterId">The PID of the parameter you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        /// <param name="addBufferPid">The PID of the parameter that will trigger the QAction that adds a new get after set request.</param>
        /// <returns>HRESULT value. A value of 0 (S_OK) indicates the set succeeded.</returns>
        /// <remarks>
        /// <para>
        /// • In case multiple parameters need to be set, it is preferred to use a single
        /// SetParameters method call in order to reduce the inter-process communication
        /// between the SLScripting and SLProtocol processes.
        /// </para>
        /// <para>
        /// • The method SetParameter(int parameterID, object value, DateTime timestamp)
        /// acts a wrapper method for a NotifyProtocol type 256 NT_SET_PARAMETER_WITH_HISTORY call.
        /// </para>
        /// </remarks>
        public static int SetParameter(this SLProtocol protocol, int parameterId, object desiredValue, int triggerId, int retries, int addBufferPid)
        {
            return protocol.SetParameter(
                new GetAfterSetQueue(
                    new GetAfterSetConfig(
                        parameterId,
                        triggerId,
                        desiredValue),
                        retries),
                        addBufferPid);
        }

        /// <summary>
        /// Sets the parameter with the specified ID to the specified value, with an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="parameterId">The PID of the parameter you want to check.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        /// <param name="addBufferPid">The PID of the parameter that will trigger the QAction that adds a new get after set request.</param>
        /// <param name="timeInfo">The timestamp.</param>
        /// <returns>HRESULT value. A value of 0 (S_OK) indicates the set succeeded.</returns>
        /// <remarks>
        /// <para>
        /// • In case multiple parameters need to be set, it is preferred to use a single
        /// SetParameters method call in order to reduce the inter-process communication
        /// between the SLScripting and SLProtocol processes.
        /// </para>
        /// <para>
        /// • The method SetParameter(int parameterID, object value, DateTime timestamp)
        /// acts a wrapper method for a NotifyProtocol type 256 NT_SET_PARAMETER_WITH_HISTORY call.
        /// </para>
        /// </remarks>
        public static int SetParameter(this SLProtocol protocol, int parameterId, object desiredValue, int triggerId, int retries, int addBufferPid, ValueType timeInfo)
        {
            return protocol.SetParameter(
                new GetAfterSetQueue(
                    new GetAfterSetConfig(
                        parameterId,
                        triggerId,
                        desiredValue),
                        retries),
                        addBufferPid,
                        timeInfo);
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value and an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="config">The set configuration. Including which parameter, the value, the number of retries.</param>
        /// <param name="addBufferPid">The parameter holding the check requests.</param>
        /// <returns>Whether the cell value has changed. true indicates change; otherwise, false.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • The primary key can never be updated.
        /// </para>
        /// <para>
        /// • This method acts as a wrapper for a NotifyProtocol type 121 call ("NT_PUT_PARAMETER_INDEX").
        /// </para>
        /// </remarks>
        public static bool SetParameterIndexByKey(this SLProtocol protocol, GetAfterSetQueue config, int addBufferPid)
        {
            if (!config.BaseRequest.IsTableParameter)
                throw new InvalidGetAfterSetConfigurationException("The given GetAfterSetConfig object is invalid. Trying to set a table parameter with a standalone parameter config.");

            var result = protocol.SetParameterIndexByKey(config.BaseRequest.TableId, config.BaseRequest.RowKey, config.BaseRequest.ColumnIdx, config.BaseRequest.DesiredValue);
            protocol.SetParameter(addBufferPid, Convert.ToString(config));
            return result;
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value and an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="config">The set configuration. Including which parameter, the value, the number of retries.</param>
        /// <param name="addBufferPid">The parameter holding the check requests.</param>
        /// <param name="timeInfo">The timestamp.</param>
        /// <returns>Whether the cell value has changed. true indicates change; otherwise, false.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • The primary key can never be updated.
        /// </para>
        /// <para>
        /// • This method acts as a wrapper for a NotifyProtocol type 121 call ("NT_PUT_PARAMETER_INDEX").
        /// </para>
        /// </remarks>
        public static object SetParameterIndexByKey(this SLProtocol protocol, GetAfterSetQueue config, int addBufferPid, ValueType timeInfo)
        {
            if (!config.BaseRequest.IsTableParameter)
                throw new InvalidGetAfterSetConfigurationException("The given GetAfterSetConfig object is invalid. Trying to set a table parameter with a standalone parameter config.");

            var result = protocol.SetParameterIndexByKey(config.BaseRequest.TableId, config.BaseRequest.RowKey, config.BaseRequest.ColumnIdx, config.BaseRequest.DesiredValue, timeInfo);
            protocol.SetParameter(addBufferPid, Convert.ToString(config), timeInfo);
            return result;
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value and an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="tablePid">The PID of the table.</param>
        /// <param name="rowKey">The unique id of the row.</param>
        /// <param name="columnIdx">The 1-based column position of the column.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        /// <param name="addBufferPid">The PID of the parameter that will trigger the QAction that adds a new get after set request.</param>
        /// <returns>Whether the cell value has changed. true indicates change; otherwise, false.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • The primary key can never be updated.
        /// </para>
        /// <para>
        /// • This method acts as a wrapper for a NotifyProtocol type 121 call ("NT_PUT_PARAMETER_INDEX").
        /// </para>
        /// </remarks>
        public static object SetParameterIndexByKey(this SLProtocol protocol, int tablePid, string rowKey, int columnIdx, object desiredValue, int triggerId, int retries, int addBufferPid)
        {
            return protocol.SetParameterIndexByKey(
                new GetAfterSetQueue(
                    new GetAfterSetConfig(
                        tablePid,
                        rowKey,
                        columnIdx,
                        triggerId,
                        desiredValue),
                        retries), 
                        addBufferPid);
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value and an extra check if the value was set.
        /// </summary>
        /// <param name="protocol">The link to the DataMiner system.</param>
        /// <param name="tablePid">The PID of the table.</param>
        /// <param name="rowKey">The unique id of the row.</param>
        /// <param name="columnIdx">The 1-based column position of the column.</param>
        /// <param name="desiredValue">The desired value the parameter should be.</param>
        /// <param name="triggerId">The PID of the trigger that will repoll the specific parameter.</param>
        /// <param name="retries">How many times the request should be checked.</param>
        /// <param name="addBufferPid">The PID of the parameter that will trigger the QAction that adds a new get after set request.</param>
        /// <param name="timeInfo">The timestamp.</param>
        /// <returns>Whether the cell value has changed. true indicates change; otherwise, false.</returns>
        /// <exception cref="InvalidGetAfterSetConfigurationException"></exception>
        /// <remarks>
        /// <para>
        /// • The primary key can never be updated.
        /// </para>
        /// <para>
        /// • This method acts as a wrapper for a NotifyProtocol type 121 call ("NT_PUT_PARAMETER_INDEX").
        /// </para>
        /// </remarks>
        public static object SetParameterIndexByKey(this SLProtocol protocol, int tablePid, string rowKey, int columnIdx, object desiredValue, int triggerId, int retries, int addBufferPid, ValueType timeInfo)
        {
            return protocol.SetParameterIndexByKey(
                new GetAfterSetQueue(
                    new GetAfterSetConfig(
                        tablePid,
                        rowKey,
                        columnIdx,
                        triggerId,
                        desiredValue),
                        retries),
                        addBufferPid,
                        timeInfo);
        }
    }
}
