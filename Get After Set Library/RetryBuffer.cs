namespace Skyline.DataMiner.Utils.GetAfterSet
{
    using System.Collections.Generic;
    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// A list containing <see cref="GetAfterSetQueue"/>'s that need to be processed.
    /// </summary>
    public class RetryBuffer : List<GetAfterSetQueue>
    {
        private readonly int addTriggerPid;

        private readonly int checkTriggerPid;

        private readonly List<GetAfterSetQueue> parameterQueues = new List<GetAfterSetQueue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryBuffer"/> class, that holds a buffer of all the <see cref="GetAfterSetQueue" />'s that still don't have their request completed.
        /// </summary>
        /// <param name="addParameterPid">The parameter that holds new <see cref="GetAfterSetQueue" /> that need to be added to the buffer. This parameter will trigger an instant QAction that runs the <see cref="RetryBuffer.Process(SLProtocol)" /> function.</param>
        /// <param name="checkParameterPid">The parameter that triggers an instant QAction that that runs the <see cref="RetryBuffer.Process(SLProtocol)" /> function to run a check on the buffer requests once.</param>
        public RetryBuffer(int addParameterPid, int checkParameterPid)
        {
            this.addTriggerPid = addParameterPid;
            this.checkTriggerPid = checkParameterPid;
        }

        /// <summary>
        /// Processes all the buffered <see cref="GetAfterSetQueue" />'s that still not have the desired value for their given standalone parameter/table cell.
        /// </summary>
        /// <param name="protocol"><see cref="SLProtocol" /> instance used to communicate with DataMiner.</param>
        public void Process(SLProtocol protocol)
        {
            int trigger = protocol.GetTriggerParameter();

            if (trigger == checkTriggerPid)
            {
                CheckParameterQueues(protocol);
            }
            else if (trigger == addTriggerPid)
            {
                AddParameterQueue(protocol);
            }
            else
            {
                protocol.Log("QA" + protocol.QActionID + "|Process|Trigger not supported", LogType.Error, LogLevel.NoLogging);
            }
        }

        private void CheckParameterQueues(SLProtocol protocol)
        {
            // If there is nothing in the queue directly leave this QAction
            if (parameterQueues.Count <= 0)
            {
                return;
            }

            foreach (GetAfterSetQueue queue in parameterQueues)
            {
                queue.DequeueWithCheck(protocol);
            }

            parameterQueues.RemoveAll((queue) => queue.Count <= 0);
        }

        private void AddParameterQueue(SLProtocol protocol)
        {
            var request = GetAfterSetQueue.LoadRequestQueueFromParameter(protocol, addTriggerPid);
            parameterQueues.Add(request);
        }
    }
}
