
namespace Presto.Managers {
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
        /// <param name="domainKey">The domain Key of this domain.</param>
        void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey);

        /// <summary>
        /// Signal to the controlling presto server that the currently running module has finished its work
        /// and is ready to be disposed.
        /// </summary>
        void SignalComplete(string domainKey);

        /// <summary>
        /// Send a message to the node with the specified ID.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="domainKey">The Key of the domain to deliver the message to.</param>
        void SendMessage(string nodeID, string message, string domainKey);

        /// <summary>
        /// Get the IDs of all nodes available to this application or module.
        /// </summary>
        /// <param name="domainKey">The domain Key associated with the requesting domain.</param>
        /// /// <param name="includeSelf">Whether or not to include the local node id in the listing.</param>
        /// <returns>All Nodes available to this application or module.</returns>
        ClusterNode[] GetAvailableNodes(string domainKey, bool includeSelf = true);

        /// <summary>
        /// Retrieve a particular node by its node id.
        /// </summary>
        /// <param name="id">The string id of the node.</param>
        /// <returns>The node with the specified id or null.</returns>
        ClusterNode GetNodeByID(string id);
    }
}
