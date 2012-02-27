using System;
using System.Reflection;

namespace Presto {

    [Serializable()]
    public class ExecutionContext {

        public MethodInfo Function;
        public IPrestoParameter Parameter;

        public ExecutionContext(MethodInfo function, IPrestoParameter param) 
        {
            Function = function;
            Parameter = param;
        }
    }
}
