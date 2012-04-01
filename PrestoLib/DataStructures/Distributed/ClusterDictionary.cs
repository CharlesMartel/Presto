﻿using System;
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

        private ConcurrentDictionary<string, Action<IAsyncResult>> callbacks = new ConcurrentDictionary<string, Action<IAsyncResult>> ();

        /// <summary>
        /// The function assigned to act as the setter for the map.
        /// </summary>
        private Action<T> setter;

        /// <summary>
        /// The function assigned to act as the getter for the map.
        /// </summary>
        private Action<string> getter;

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
        }

        public void Get (string key, Action<IAsyncResult> callback) {

        }

        public void Set (string key, T value, Action<IAsyncResult> callback) {
            
        }

        
    }
}
