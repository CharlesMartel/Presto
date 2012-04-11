﻿using System;
using System.Collections.Generic;
using Presto.Remote;
using Presto.Transfers;

namespace Presto.Managers {

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
        /// <param name="domainKey">The domain Key of this domain.</param>
        public void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey) {
            ExecutionContext context = new ExecutionContext(assemblyName, typeName, methodName, param, contextid, domainKey);
            Nodes.BestNode().Execute(context);
        }

        /// <summary>
        /// Signal to the controlling presto server that the currently running module has finished its work
        /// and is ready to be disposed.
        /// </summary>
        public void SignalComplete(string domainKey) {
            DomainManager.DestroyDomain(domainKey, true);
        }

        /// <summary>
        /// Send a message to the node with the specified ID.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="domainKey"> The Key of the domain to deliver the message to.</param>
        public void SendMessage(string nodeID, string message, string domainKey) {
            Nodes.SendMessage(nodeID, message, domainKey);
        }

        /// <summary>
        /// Get the IDs of all nodes available to this application or module.
        /// </summary>
        /// <param name="domainKey">The domain Key associated with the requesting domain.</param>
        /// <returns>List of all node IDs available to this application or module.</returns>
        public string[] GetAvailableNodes(string domainKey) {
            Node[] associatedNodes = Nodes.GetAssociatedNodes(domainKey);
            List<string> NodeIDs = new List<string>();
            foreach (Node node in associatedNodes) {
                if (node.Available) {
                    NodeIDs.Add(node.NodeID);
                }
            }
            return NodeIDs.ToArray();
        }
    }
}
