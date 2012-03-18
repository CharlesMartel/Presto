using System;

namespace Presto {

    /// <summary>
    /// The result structure returned from a succesful distributed execution.
    /// </summary>
    [Serializable()]
    public struct ExecutionResult {

        /// <summary>
        /// The PrestoResult returned from the execution function.
        /// </summary>
        public PrestoResult Result;
        /// <summary>
        /// The generated context ID belonging to this distributed execution.
        /// </summary>
        public string ContextID;

        /// <summary>
        /// Creates a Execution Result with the specefied PrestoResult and contextID.
        /// </summary>
        /// <param name="result">The PrestoResult object returned from the function executed.</param>
        /// <param name="contextid">The context ID of this distributed execution.</param>
        public ExecutionResult(PrestoResult result, string contextid) {
            Result = result;
            ContextID = contextid;
        }
    }
}
