using System;

namespace Presto {

    [Serializable()]
    struct ExecutionResult {
        
        public IPrestoResult Result;

        public ExecutionResult(IPrestoResult result) 
        {
            Result = result;
        }
    }
}
