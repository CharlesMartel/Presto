using System;

namespace Presto.Transfers {

    /// <summary>
    /// The result structure returned from a succesful distributed execution.
    /// </summary>
    [Serializable()]
    public struct ExecutionResult {

        /// <summary>
        /// The PrestoResult returned from the execution function.
        /// </summary>
        public byte[] Result;
        /// <summary>
        /// The generated context ID belonging to this distributed execution.
        /// </summary>
        public string ContextID;
        /// <summary>
        /// The domain Key associated with the execution.
        /// </summary>
        public string DomainKey;
        /// <summary>
        /// The node that this execution was run on.
        /// </summary>
        public string ExecutingNodeID;

        /// <summary>
        /// Creates a Execution Result with the specefied PrestoResult and contextID.
        /// </summary>
        /// <param name="result">The PrestoResult object returned from the function executed.</param>
        /// <param name="contextid">The context ID of this distributed execution.</param>
        /// <param name="domainKey"> The domain Key to deliver the result to.</param>
        /// <param name="nodeId">The node id that produced the result.</param>
        public ExecutionResult(byte[] result, string contextid, string domainKey, string nodeId) {
            Result = result;
            ContextID = contextid;
            DomainKey = domainKey;
            ExecutingNodeID = nodeId;
        }
    }
}
