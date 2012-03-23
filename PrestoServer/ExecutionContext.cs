using System;
using System.Reflection;

namespace Presto {

    /// <summary>
    /// Internal serializable structure defining the properties of a remote execution.
    /// </summary>
    [Serializable()]
    public struct ExecutionContext {

        /// <summary>
        /// The full name of the assembly this execution is to run in.
        /// </summary>
        public string AssemblyName;
        /// <summary>
        /// The name of the class type in the assembly that this execution is to run in.
        /// </summary>
        public string TypeName;
        /// <summary>
        /// The name of the method that is to be run as the execution.
        /// </summary>
        public string MethodName;
        /// <summary>
        /// The PrestoParameter to be passed into the execution function.
        /// </summary>
        public PrestoParameter Parameter;
        /// <summary>
        /// The Gernerated ID of this distributed execution.
        /// </summary>
        public string ContextID;
        /// <summary>
        /// The key of the domain this execution will run in.
        /// </summary>
        public string DomainKey;

        /// <summary>
        /// Creates a new ExecutionContext to be distributed across the cluster.
        /// </summary>
        /// <param name="method">The method to be executed.</param>
        /// <param name="param">The PrestoParameter to be passed into the method.</param>
        /// <param name="contextid">The Gernerated ID of this distributed execution.</param>
        public ExecutionContext(MethodInfo method, PrestoParameter param, string contextid, string domainKey) {
            AssemblyName = method.DeclaringType.Assembly.FullName;
            TypeName = method.DeclaringType.FullName;
            MethodName = method.Name;
            Parameter = param;
            ContextID = contextid;
            DomainKey = domainKey;
        }
    }
}
