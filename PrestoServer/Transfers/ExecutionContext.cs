using System;

namespace Presto.Transfers {

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
        /// The PrestoParameter to be passed into the execution function. Serialized inside app domain.
        /// </summary>
        public byte[] Parameter;
        /// <summary>
        /// The Gernerated ID of this distributed execution.
        /// </summary>
        public string ContextID;
        /// <summary>
        /// The Key of the domain this execution will run in.
        /// </summary>
        public string DomainKey;

        /// <summary>
        /// Creates a new ExecutionContext to be distributed across the cluster.
        /// </summary>
        /// <param name="assemblyName">The full name of the containing assembly.</param>
        /// <param name="typeName"> The full name of the containing type.</param>
        /// <param name="methodName"> The full name of the method to be executed.</param>
        /// <param name="param">The PrestoParameter to be passed into the method.</param>
        /// <param name="contextid">The Gernerated ID of this distributed execution.</param>
        /// <param name="domainKey">The domain Key of the executing domain.</param>
        public ExecutionContext(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey) {
            AssemblyName = assemblyName;
            TypeName = typeName;
            MethodName = methodName;
            Parameter = param;
            ContextID = contextid;
            DomainKey = domainKey;
        }
    }
}
