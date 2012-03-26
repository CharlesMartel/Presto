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
        /// Basic execution request.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        /// <param name="contextid"></param>
        /// <param name="domainKey"></param>
        public void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey) {
            ExecutionContext context = new ExecutionContext(assemblyName, typeName, methodName, param, contextid, domainKey);
            Nodes.BestNode().Execute(context);
        }
    }
}
