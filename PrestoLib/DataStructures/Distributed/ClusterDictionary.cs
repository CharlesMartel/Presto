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
        private ConflictResolution ConflictResolutionPolicy = ConflictResolution.CHRONOLOGICAL;            

        /// <summary>
        /// The distribution method for the dictionary.
        /// </summary>
        private DistributionMethod ClusterDistributionMethod = DistributionMethod.FULLY_DISTRIBUTED;

        /// <summary>
        /// The update interval for which data is broadcast to subscribing nodes.
        /// </summary>
        private UpdateInterval UpdateIntervalPolicy = UpdateInterval.IMMEDIATE;

        /// <summary>
        /// The cluster dictionary is more or less a simple wrapper for a standard dictionary. This is the internal dictionary.
        /// </summary>
        private Dictionary<string, T> dictionary = new Dictionary<string,T>();

        /// <summary>
        /// Create a new cluster dictionary, only called internally.
        /// </summary>
        internal ClusterDictionary () {
        }

        /// <summary>
        /// Gets or sets the value associated with a specefied key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns></returns>
        public T this[string key] {
            
            get {
                return getValue(key);
            }

            set {
            }

        }

        /// <summary>
        /// Get the value at a particular key.
        /// </summary>
        /// <param name="key">The key to obtain the value from.</param>
        /// <returns>The value held at the specefied key or null.</returns>
        private T getValue(string key) {

        }

    }
}
