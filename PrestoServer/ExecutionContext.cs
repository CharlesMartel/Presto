using System;
using System.Reflection;

namespace Presto {

    /// <summary>
    /// Internal serializable structure defining the properties of a remote execution.
    /// </summary>
    [Serializable()]
    public struct ExecutionContext {

        public string AssemblyName;
        public string TypeName;
        public string MethodName;
        public PrestoParameter Parameter;

        /// <summary>
        /// Creates a new ExecutionCOntext
        /// </summary>
        /// <param name="method">The method to be executed.</param>
        /// <param name="param">The PrestoParameter to be passed into the method.</param>
        public ExecutionContext(MethodInfo method, PrestoParameter param) 
        {
            AssemblyName = method.DeclaringType.Assembly.FullName;
            TypeName = method.DeclaringType.FullName;
            MethodName = method.Name;
            Parameter = param;
        }
    }
}
