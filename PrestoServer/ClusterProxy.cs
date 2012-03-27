using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.Transfers;

namespace Presto {

    /// <summary>
    /// Proxies requests out of the app domain and into the cluster.
    /// </summary>
    class ClusterProxy : MarshalByRefObject, IClusterProxy {

        /// <summary>
        /// Deploys an execution into the cluster.
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly.</param>
        /// <param name="typeName">The full name of the type.</param>
        /// <param name="methodName">The full name of the method.</param>
        /// <param name="param">the serialized parameter object.</param>
        /// <param name="contextid">The context id of this execution.</param>
        /// <param name="domainKey">The domain key of this domain.</param>
        public void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey) {
            ExecutionContext context = new ExecutionContext(assemblyName, typeName, methodName, param, contextid, domainKey);
            Nodes.BestNode().Execute(context);
        }
    }
}
