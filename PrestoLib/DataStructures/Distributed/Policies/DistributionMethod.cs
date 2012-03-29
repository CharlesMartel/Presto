using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed.Policies {
    
    
    public enum DistributionMethod {
        /// <summary>
        /// Every node maintains a full copy of the data. Broadcasts are sent to update other nodes
        /// with changed data according to the UpdateFrequency policy.
        /// </summary>
        FULLY_DISTRIBUTED
    }
}
