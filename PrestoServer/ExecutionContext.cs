using System;
using System.Reflection;

namespace Presto {

    /// <summary>
    /// Internal serializable structure defining the properties of a remote execution.
    /// </summary>
    [Serializable()]
    struct ExecutionContext {

        public string AssemblyName;
        public string TypeName;
        public string MethodName;
        public IPrestoParameter Parameter;

        /// <summary>
        /// Creates a new ExecutionCOntext
        /// </summary>
        /// <param name="method">The method to be executed.</param>
        /// <param name="param">The IPrestoParameter to be passed into the method.</param>
        public ExecutionContext(MethodInfo method, IPrestoParameter param) 
        {
            AssemblyName = method.DeclaringType.Assembly.FullName;
            TypeName = method.DeclaringType.FullName;
            MethodName = method.Name;
            Parameter = param;
        }
    }
}
