using System;
using System.Collections.Generic;
using System.Linq;

namespace Presto {

    /// <summary>
    /// A static class holding a listing of all node instances and methods to manipulate them
    /// </summary>
    public static class Nodes {

        private static List<Node> nodes = new List<Node>();

        public static int Count = 0;
        public static long CDPI = 0;

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
    }
}
