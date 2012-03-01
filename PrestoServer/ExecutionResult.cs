using System;

namespace Presto {

    [Serializable()]
    struct ExecutionResult {
        
        public PrestoResult Result;

        public ExecutionResult(PrestoResult result) 
        {
            Result = result;
        }
    }
}
