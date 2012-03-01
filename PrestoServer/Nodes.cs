using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

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
            RecalculateCDPI();
        }

        /// <summary>
        /// Recalculates the CDPI of the cluster.
        /// </summary>
        public static void RecalculateCDPI() {

        }

        /// <summary>
        /// Gets the node that is best for a new cluster job.
        /// </summary>
        /// <returns></returns>
        public static Node BestNode() {
            //TODO: return best node
            return null;
        }
    }
}
