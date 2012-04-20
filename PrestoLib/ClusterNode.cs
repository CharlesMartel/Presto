using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto {

    /// <summary>
    /// A specific node in the cluster.
    /// </summary>
    public abstract class ClusterNode : MarshalByRefObject {

        /// <summary>
        /// The string ID of this node.
        /// </summary>
        public string NodeID {
            get;
            protected set;
        }

        /// <summary>
        /// The DNS resolved hostname of this node.
        /// </summary>
        public string HostName {
            get;
            protected set;
        }

        /// <summary>
        /// The calculated DPI of this node.
        /// </summary>
        public double DPI
        {
            get;
            protected set;
        }

        /// <summary>
        /// The string address of this node on the network.
        /// </summary>
        public string Address
        {
            get;
            protected set;
        }

        /// <summary>
        /// Retrieve a node by its specified id.
        /// </summary>
        /// <param name="id">The string id of the node to retrieve.</param>
        /// <returns>The specified node or null if none match the specified id.</returns>
        public static ClusterNode GetNodeByID(string id) {
            return Cluster.ClusterProxy.GetNodeByID(id);
        }
    }
}
