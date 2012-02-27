using System;
using System.Reflection;

namespace Presto {

    [Serializable()]
    public class ExecutionContext {

        public delegate IPrestoResult ExecutionDelegate(IPrestoParameter param);
        ExecutionDelegate Function;
        public IPrestoParameter Parameter;

        public ExecutionContext(ExecutionDelegate function, IPrestoParameter param) 
        {
            Function = function;
            Parameter = param;
        }
    }
}
