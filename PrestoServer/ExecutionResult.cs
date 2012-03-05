using System;

namespace Presto {

    [Serializable()]
    public struct ExecutionResult {

        public PrestoResult Result;
        public string ContextID;

        public ExecutionResult(PrestoResult result, string contextid) {
            Result = result;
            ContextID = contextid;
        }
    }
}
