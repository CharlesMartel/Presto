using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.DataStructures.Distributed.Policies;

namespace Presto.DataStructures.Distributed {
    
    public class ClusterDictionary<T> where T : struct {

        /// <summary>
        /// The conflict resolution policy for conflicting data.
        /// </summary>
        public ConflictResolution ConflictResolutionPolicy = ConflictResolution.CHRONOLOGICAL;

        /// <summary>
        /// The distribution method for the dictionary.
        /// </summary>
        public DistributionMethod ClusterDistributionMethod = DistributionMethod.FULLY_DISTRIBUTED;

        /// <summary>
        /// The update interval for which data is broadcast to subscribing nodes.
        /// </summary>
        public UpdateInterval UpdateIntervalPolicy = UpdateInterval.IMMEDIATE;

        /// <summary>
        /// Create a new cluster dictionary, only called internally.
        /// </summary>
        internal ClusterDictionary () {
        }
    }
}
