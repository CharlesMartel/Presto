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

        /// <summary>
        /// Send a message to the node with the specified ID. The message is UTF8 encoded on transport and is delivered to 
        /// the receiving node calling MessageReceived event.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent. This message is UTF8 encoded on transport.</param>
        /// <param name="domainKey">The key of the domain to deliver the message to.</param>
        void SendMessage(string nodeID, string message, string domainKey);
    }
}
