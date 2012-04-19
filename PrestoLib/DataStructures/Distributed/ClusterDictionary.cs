using System;
using System.Collections.Concurrent;
using Presto.Managers;
using Presto.Common;
using Presto.DataStructures.Distributed.Policies;

namespace Presto.DataStructures.Distributed {
    
    /// <summary>
    /// A multi-purpose clustered dictionary.
    /// </summary>
    /// <remarks>
    /// The cluster dictionary is a hash table sychronized across many or 1 machine. Either one machine has all of the data
    /// or all machines have all of the data. It is not a distributed table and should 
    /// not be considered one. If you are in need of a distributed table where different areas of the map are on
    /// different physical machines, look into the DistributedDictionary instead. The cluster dictionary is nothing more than 
    /// a single dictionary that Presto constantly watches to maintain data coherency. Carefully guage what data structure your
    /// application will need as more often than not, one will be better than the other for your scenario.
    /// </remarks>
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
        private ConcurrentDictionary<string, Action<ClusterDictionaryResult<T>>> callbacks = new ConcurrentDictionary<string, Action<ClusterDictionaryResult<T>>> ();

        /// <summary>
        /// The function assigned to act as the setter for the map.
        /// </summary>
        private Action<string, T> setter;

        /// <summary>
        /// The function assigned to act as the getter for the map.
        /// </summary>
        private Action<string, Action<ClusterDictionaryResult<T>>, ClusterDictionaryResult<T>> getter;

        /// <summary>
        /// Create a new cluster dictionary, only called internally.
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

        /// <summary>
        /// Retrieve a value from the cluster dictionary. Being that this could invoke a call to another machine over the network, 
        /// the retrieval must be asynchronous and invoke a callback upon completion. If retrieval does not need to go across the network,
        /// the callback is immediately run.
        /// </summary>
        /// <param name="key">The Key of the value to retrieve.</param>
        /// <param name="callback">The callback to be run after the value is retrieved.</param>
        /// <param name="state">A state to carry with the async call and passed into the callback.</param>
        public ClusterDictionaryResult<T> Get (string key, Action<ClusterDictionaryResult<T>> callback, Object state) {
            ClusterDictionaryResult<T> result = new ClusterDictionaryResult<T>();
            result.AsyncState = state;
            result.IsCompleted = false;
            result.ResultErrorState = AsyncResultErrorState.NONE;
            getter.BeginInvoke(key, callback, result, null, null);
            return result;
        }

        /// <summary>
        /// Set a value in the cluster dictionary. If the Key does not exist, one is created and the value added.
        /// </summary>
        /// <param name="key">The Key of the value to set.</param>
        /// <param name="value">The value.</param>
        public void Set (string key, T value) {
            setter.Invoke (key, value);
        }

        //----------------------The get and set functions----------------------//
        //This is kind of weird, I admit, but I had to think of an efficient way to manage this. Each different policy added gives
        //an exponential increase in the number of code paths, so what we do is just split it all up into different compact possibilities.
        //This way, there is only one code path, and that code path is simply created during initialization of the instance.

        private void getLocal (string key, Action<ClusterDictionaryResult<T>> callback, ClusterDictionaryResult<T> result) {
            if (map.ContainsKey(key))
            {
                //The internal map has the value and we can get local, get it immediately and return
                T value = map[key].GetValue();
                result.Value = value;
                result.LocallyObtained = true;
                result.IsCompleted = true;
                result.ResultErrorState = AsyncResultErrorState.NONE;
                if (callback != null) {
                    callback.BeginInvoke(result, null, null);
                }
            }
            else
            {
                //create a callback id and issue a cross network request for data                
            }
        }





        //-------------------------------Network update functions---------------------------------------------------------//
        //These will handle incoming an doutgoing netowrk events.

        private void broadcastUpdate(string key, T value)
        {
            
        }

        private void receiveBroadcast(string key, T value) 
        {

        }

        private void requestValue()
        {

        }

        private void receiveValue()
        {

        }


    }
}
