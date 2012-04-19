using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Common {

    /// <summary>
    /// A struct holding information about the server and the cluster as a whole.
    /// </summary>
    [Serializable]
    public struct ServerStatus {
        /// <summary>
        /// The number of currently active nodes in the cluster.
        /// </summary>
        public int NodeCount;
        /// <summary>
        /// The total accesible memory in the cluster.
        /// </summary>
        public long TotalMemory;
        /// <summary>
        /// The Cluster Distribution Performance Indicator.
        /// </summary>
        public long CDPI;
        /// <summary>
        /// The total number of logical CPUs in the cluster.
        /// </summary>
        public int CPUCount;
    }
}
