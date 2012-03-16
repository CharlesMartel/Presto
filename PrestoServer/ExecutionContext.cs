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
        public string ContextID;

        /// <summary>
        /// Creates a new ExecutionCOntext
        /// </summary>
        /// <param id="method">The method to be executed.</param>
        /// <param id="param">The PrestoParameter to be passed into the method.</param>
        public ExecutionContext(MethodInfo method, PrestoParameter param, string contextid) {
            AssemblyName = method.DeclaringType.Assembly.FullName;
            TypeName = method.DeclaringType.FullName;
            MethodName = method.Name;
            Parameter = param;
            ContextID = contextid;
        }
    }
}
