using System;
using System.Collections.Generic;
using Presto.Common;
using Presto.Managers;
using Presto.Transfers;

namespace Presto.Remote {

    /// <summary>
    /// A static class holding a listing of all node instances and methods to manipulate them
    /// </summary>
    public static class Nodes {

        /// <summary>
        /// Private listing of all nodes in the cluster, regardless of state.
        /// </summary>
        private static List<Node> nodes = new List<Node>();

        /// <summary>
        /// The total number of nodes in the cluster.
        /// </summary>
        private static int Count = 0;
        /// <summary>
        /// The Cluster Distribution performance indicator. This value is constantly being generated according to the
        /// verification interval.
        /// </summary>
        private static long CDPI = 0;

        /// <summary>
        /// Initializes the node listing.
        /// </summary>
        public static void Initialize() {
            //get the list of nodes and attempt a connection to them by passing the Address into the Node Object
            string[] hosts = Config.GetHosts();
            foreach (string host in hosts) {
                nodes.Add(new Node(host));
                Count++;
            }
        }

        /// <summary>
        /// Gets the node that is best for a new cluster job. If there are no other nodes, or all other nodes are not available
        /// or possibly even not connected, then the default self node is returned.
        /// </summary>
        /// <param name="domainKey">The domain key to find a node for.</param>
        /// <returns>The best node to take on a cluster job.</returns>
        public static Node BestNode(string domainKey) {
            //get all nodes associated with this domain
            Node[] listing = GetAssociatedNodes(domainKey);
            //the node with the lowest saturation will be the best candidate for distribution.
            Node bestNode = listing[0];
            foreach (Node current in listing)
            {
                if (bestNode.Saturation > current.Saturation)
                {
                    bestNode = current;
                }
            }
            return bestNode;
        }

        /// <summary>
        /// Gets the number of nodes in the cluster.
        /// </summary>
        /// <returns>The number of nodes in the cluster.</returns>
        public static int GetNodeCount() {
            return Count;
        }

        /// <summary>
        /// Gets the number of active nodes in the cluster.
        /// </summary>
        /// <returns>The number of available nodes in the cluster.</returns>
        public static int GetTotalAvailableNodeCount() {
            int counter = 0;
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].Available) {
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Gets the Cluster Distribution Performance Indicator for this cluster.
        /// This value is constantly being updated according to the verification interval
        /// as network speed affects its value.
        /// </summary>
        /// <returns>The CDPI of the cluster.</returns>
        public static long GetCDPI() {
            return CDPI;
        }

        /// <summary>
        /// Signal to all nodes who have the specefied domain loaded to unload that domain and purge all 
        /// resources associated with it.
        /// </summary>
        /// <param name="domainKey">The Key of the domain to be unloaded.</param>
        /// <param name="assemblyNames">The names of the assemblies loaded for this domain.</param>
        public static void UnloadDomain(string domainKey, string[] assemblyNames) {
            foreach (Node currentNode in nodes) {
                if (currentNode.HasDomain(domainKey)) {
                    currentNode.UnloadDomain(domainKey, assemblyNames);
                }
            }
        }

        /// <summary>
        /// Send a message to the node with the specified ID.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        /// <param name="domainKey">The domain Key to deliver the message to.</param>
        public static void SendMessage(string nodeID, string message, string domainKey) {
            Node remoteNode = getNodeByID(nodeID);
            if (remoteNode != null) {
                UserMessage mesg = new UserMessage(message, ClusterManager.NodeID, domainKey);
                remoteNode.SendMessage(mesg);
            } else {
                //No node could be found matching the specefied id, log error and lost message
                Log.Error("No node with ID: " + nodeID + " could be found to dispatch message: \"" + message + "\" to.");
            }
        }

        /// <summary>
        /// Retrieve a node by node id, returns null if no node matches the specified id.
        /// </summary>
        /// <param name="id">The id of the node to retrieve.</param>
        /// <returns></returns>
        private static Node getNodeByID(string id) {
            foreach (Node node in nodes) {
                if (node.NodeID == id) {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all nodes a particular cluster is allowed to access. Since the functionality for partial clusters has not yet been implemented,
        /// returns all nodes.
        /// </summary>
        /// <param name="domainKey">The domain Key to get associated nodes for.</param>
        /// <returns>Array of nodes available to a single instance.</returns>
        public static Node[] GetAssociatedNodes(string domainKey){
            return nodes.ToArray();
        }

        /// <summary>
        /// Retrieve the node with the specified id.
        /// </summary>
        /// <param name="id">The id of the node to retrieve.</param>
        /// <returns>The node with the specefied id.</returns>
        public static Node GetNodeByID(string id){
            foreach (Node node in nodes)
            {
                if (node.NodeID == id)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Get all available nodes in the cluster.
        /// </summary>
        /// <returns>An array of all available nodes in the cluster.</returns>
        public static Node[] GetAllAvailableNodes() {
            List<Node> listing = new List<Node>();
            foreach (Node node in nodes) {
                if (node.Available) {
                    listing.Add(node);
                }
            }
            return listing.ToArray();
        }

        /// <summary>
        /// Get the total number of logical cpus in the cluster.
        /// </summary>
        /// <returns>The total number of logical cpus in the cluster.</returns>
        public static int GetCPUCount() {
            Node[] listing = GetAllAvailableNodes();
            int cpuCount = 0;
            foreach (Node node in listing) {
                cpuCount += node.CPUCount;
            }
            return cpuCount;
        }

        /// <summary>
        /// Get the total amount of memory in the cluster.
        /// </summary>
        /// <returns>The total amount of memory in the cluster.</returns>
        public static long GetTotalMemory() {
            Node[] listing = GetAllAvailableNodes();
            long total = 0;
            foreach (Node node in listing) {
                total += node.TotalMemory;
            }
            return total;
        }
    }
}
