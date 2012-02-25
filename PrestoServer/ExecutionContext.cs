using System;
using System.Reflection;

namespace Presto {

    [Serializable()]
    struct ExecutionContext {

        public Func<IPrestoParameter, IPrestoResult> Function;
        public IPrestoParameter Parameter;

        public ExecutionContext(Func<IPrestoParameter, IPrestoResult> function, IPrestoParameter param) 
        {
            Function = function;
            Parameter = param;
        }
    }
}
