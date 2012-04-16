using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.DataStructures.Distributed;

namespace Presto.DataStructures {
    /// <summary>
    /// The DataStructure Factory. Create, Startup, and Manager Presto Specific DataStructures.
    /// </summary>
    public static class DataStructures {        

        /// <summary>
        /// Startup a map if one wiht the name specefied already exists in the cluster, otherwise
        /// create a new map with that name.
        /// </summary>
        /// <typeparam name="T">The type of data stored in the value column of the map.</typeparam>
        /// <param name="name">The name of the map, will be used to link the map cluster wide.</param>
        public static ClusterDictionary<T> LoadCreateClusterDictionary<T>(string name) where T : struct {
            ClusterDictionary<T> newDictionary = new ClusterDictionary<T> ();
            return newDictionary;
        }
    }
}
