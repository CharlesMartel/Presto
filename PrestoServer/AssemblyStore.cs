using System;
using System.Collections.Generic;
using Presto.Common;

namespace Presto {
    /// <summary>
    /// Stores loaded assemblys and allows for easy retrieval and removal.
    /// </summary>
    public static class AssemblyStore {

        /// <summary>
        /// The internal hashmap of loaded assemblies
        /// </summary>
        private static Dictionary<string, AssemblyWrapper> assemblies = new Dictionary<string, AssemblyWrapper>();

        /// <summary>
        /// Adds a new assembly to the store.
        /// </summary>
        /// <param name="assemblyWrapper">The assembly wrapper for the assembly.</param>
        public static void Add(AssemblyWrapper assemblyWrapper)
        {
            if(assemblies.ContainsKey(assemblyWrapper.GetAssemblyName()){
                assemblies[assemblyWrapper.GetAssemblyName()] = assemblyWrapper;
                return;
            }
            assemblies.Add(assemblyWrapper.GetAssemblyName(), assemblyWrapper);
        }

        /// <summary>
        /// Remove the assembly according to the provided assembly name.
        /// </summary>
        /// <param name="assemblyFullName">The full name of the assembly to be removed.</param>
        public static void Remove(string assemblyFullName) 
        {
            assemblies.Remove(assemblyFullName);
        }

        /// <summary>
        /// Remove the assembly according to the provided assembly wrapper.
        /// </summary>
        /// <param name="assemblyWrapper">The assembly wrapper of the assembly to be removed.</param>
        public static void Remove(AssemblyWrapper assemblyWrapper) 
        {
            assemblies.Remove(assemblyWrapper.GetAssemblyName());
        }
    }
}
