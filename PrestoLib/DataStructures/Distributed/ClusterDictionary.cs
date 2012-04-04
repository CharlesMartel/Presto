using System;
using System.Collections.Concurrent;
using Presto.Managers;
using Presto.Common;
using Presto.DataStructures.Distributed.Policies;

namespace Presto.DataStructures.Distributed {
    
    /// <summary>
    /// A multi-purpose clustered dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the values held in the dictionary, all keys must be strings.</typeparam>
    public class ClusterDictionary<T> where T : struct {

        /// <summary>
        /// The conflict resolution policy for conflicting data.
        /// </summary>
        public readonly ConflictResolution ConflictResolutionPolicy;

        /// <summary>
        /// The distribution method for the map.
        /// </summary>
        public readonly DistributionMethod ClusterDistributionMethod;

        /// <summary>
        /// The update interval for which data is broadcast to subscribing nodes.
        /// </summary>
        public readonly UpdateInterval UpdateIntervalPolicy;

        /// <summary>
        /// The initialization policy for deciding how data gets loaded up initialization.
        /// </summary>
        public readonly Initialization InitializationPolicy;

        /// <summary>
        /// The cluster dictionary is more or less a simple wrapper for a standard dictionary that maintains state across many machines.
        /// This is the internal dictionary.
        /// </summary>
        private ConcurrentDictionary<string, ClusterDictionaryValue<T>> map = new ConcurrentDictionary<string,ClusterDictionaryValue<T>>();

        /// <summary>
        /// A listing of the callbacks associated with internetwork reads and writes.
        /// </summary>
        private ConcurrentDictionary<string, Action<IAsyncResult>> callbacks = new ConcurrentDictionary<string, Action<IAsyncResult>> ();

        /// <summary>
        /// The function assigned to act as the setter for the map.
        /// </summary>
        private Action<string, T> setter;

        /// <summary>
        /// The function assigned to act as the getter for the map.
        /// </summary>
        private Action<string, Action<Object, T>, Object> getter;

        /// <summary>
        /// Create a new cluster map, only called internally.
        /// </summary>
        internal ClusterDictionary (ConflictResolution conflictResolutionPolicy = ConflictResolution.CHRONOLOGICAL,
                                    DistributionMethod clusterDistributionMethod = DistributionMethod.FULLY_DISTRIBUTED,
                                    UpdateInterval updateIntervalPolicy = UpdateInterval.IMMEDIATE,
                                    Initialization initializationPolicy = Initialization.LAZY) {
            ConflictResolutionPolicy = conflictResolutionPolicy;
            ClusterDistributionMethod = clusterDistributionMethod;
            UpdateIntervalPolicy = updateIntervalPolicy;
            InitializationPolicy = initializationPolicy;
            initialize ();
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        private void initialize () {
            switch (ClusterDistributionMethod) {
                case DistributionMethod.FULLY_DISTRIBUTED:
                    getter = getLocal;
                    break;
                case DistributionMethod.CENTRALIZED:
                    break;
            }
        }   

        public void Get (string key, Action<Object, T> callback, Object state) {
            getter.Invoke (key, callback, state);
        }

        public void Set (string key, T value) {
            setter.Invoke (key, value);
        }

        //----------------------The get and set functions----------------------//
        //This is kind of weird, I admit, but I had to think of an efficient way to manage this. Each different policy added gives
        //an exponential increase in the number of code paths, so what we do is just split it all up into different compact possibilities.
        //This way, there is only one code path, and that code path is simply created during initialization of the instance.

        private void getLocal (string key, Action<Object, T> callback, Object state) {
            
        }

        
    }
}
