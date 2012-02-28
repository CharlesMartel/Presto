using System;
using System.Reflection;

namespace Presto {

    [Serializable()]
    struct ExecutionContext {

        public string AssemblyName;
        public string TypeName;
        public string MethodName;
        public IPrestoParameter Parameter;

        public ExecutionContext(MethodInfo method, IPrestoParameter param) 
        {
            AssemblyName = method.DeclaringType.Assembly.FullName;
            TypeName = method.DeclaringType.FullName;
            MethodName = method.Name;
            Parameter = param;
        }
    }
}
