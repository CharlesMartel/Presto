using System;
using System.Collections.Generic;
using System.Linq;

namespace Presto {

    /// <summary>
    /// A static class holding a listing of all node instances and methods to manipulate them
    /// </summary>
    public static class Nodes {

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
            //get the list of nodes and attempt a connection to them by passing the address into the Node Object
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
        /// <returns></returns>
        public static Node BestNode() { 
            //This needs to be smarter, drawing on DPI and such...
            Node bestNode = null;
            float currentLoad = float.MaxValue;
            foreach (Node current in nodes) {
                float estLoad = current.EstimatedLoad();
                if (estLoad < currentLoad) {
                    currentLoad = estLoad;
                    bestNode = current;
                }
            }
            return bestNode;
        }

        /// <summary>
        /// Gets the number of nodes in the cluster.
        /// </summary>
        /// <returns>The number of nodes in the cluster.</returns>
        public static int GetNodeCount(){
            return Count;
        }

        /// <summary>
        /// Gets the number of active nodes in the cluster.
        /// </summary>
        /// <returns>The number of available nodes in the cluster.</returns>
        public static int GetAvailableNodeCount() {
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
    }
}
