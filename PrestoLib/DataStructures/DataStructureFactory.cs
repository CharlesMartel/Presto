using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.DataStructures.Distributed;

namespace Presto.DataStructures {
    /// <summary>
    /// The DataStructure Factory. Create, Load, and Manager Presto Specific DataStructures.
    /// </summary>
    public static class DataStructureFactory {

        /// <summary>
        /// Load a dictionary if one wiht the name specefied already exists in the cluster, otherwise
        /// create a new dictionary with that name.
        /// </summary>
        /// <typeparam name="T">The type of data stored in the value column of the dictionary.</typeparam>
        /// <param name="name">The name of the dictionary, will be used to link the dictionary cluster wide.</param>
        public static ClusterDictionary<T> LoadCreateDictionary<T>(string name) where T : struct {
            ClusterDictionary<T> newDictionary = new ClusterDictionary<T> ();
            return newDictionary;
        }
    }
}
