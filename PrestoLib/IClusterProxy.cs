using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto {
    /// <summary>
    /// Provides a proxy out of the appdomain for cluster specific funtionality. This object gets tracked by the domain manager.
    /// </summary>
    public interface IClusterProxy {
        /// <summary>
        /// Deploys an execution into the cluster.
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly.</param>
        /// <param name="typeName">The full name of the type.</param>
        /// <param name="methodName">The full name of the method.</param>
        /// <param name="param">the serialized parameter object.</param>
        /// <param name="contextid">The context id of this execution.</param>
        /// <param name="domainKey">The domain key of this domain.</param>
        void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey);
        
        /// <summary>
        /// Signal to the controlling presto server that the currently running module has finished its work
        /// and is ready to be disposed.
        /// </summary>
        void SignalComplete(string domainKey);
    }
}
