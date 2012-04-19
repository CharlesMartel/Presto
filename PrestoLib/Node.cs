using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto {

    /// <summary>
    /// A specific node in the cluster.
    /// </summary>
    public class Node {

        /// <summary>
        /// The string ID of this node.
        /// </summary>
        public string NodeID {
            get;
            internal set;
        }

        /// <summary>
        /// The DNS resolved hostname of this node.
        /// </summary>
        public string HostName {
            get;
            internal set;
        }

        /// <summary>
        /// Send a message to this node.
        /// </summary>
        /// <param name="payload">The message to be sent.</param>
        public void SendMessage(string payload) {
            Cluster.SendMessage(this, payload);
        }










        /// <summary>
        /// Creates a new Node Object.
        /// </summary>
        internal Node(string id, string hostname) {
            NodeID = id;
            HostName = hostname;
        }

        public static Node GetNodeByID(string id, string hostname) {
            return new Node(id, hostname);
        }
    }
}
