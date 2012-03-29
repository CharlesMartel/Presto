using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed.Policies {
    
    /// <summary>
    /// Specifies how data in the distributed data structure is spanned across the cluster.
    /// </summary>
    public enum DistributionMethod {

        /// <summary>
        /// Every node maintains a full copy of the data. Broadcasts are sent to update other nodes
        /// with changed data according to the UpdateFrequency policy. This method loosely couples the data to provide
        /// a major performance benefit, it does not, however, provide data atomicity, and relies on the Conflict Resolution
        /// Policy to dictate what data is valid when a conflict occurs.
        /// </summary>
        FULLY_DISTRIBUTED,

        /// <summary>
        /// Data is centralized to a single instance. The instance for which the data is stored is decided by a performance
        /// heuristic and can change at runtime. This method guarantees data atomicity but adds significant overhead as
        /// every request to the data is required to traverse the cluster.
        /// </summary>
        CENTRALIZED
    }
}
