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
        /// <param id="assemblyWrapper">The assembly wrapper for the assembly.</param>
        public static void Add(AssemblyWrapper assemblyWrapper) {
            //make sure assembly does not already exist, we overwrite if it does
            if (assemblies.ContainsKey(assemblyWrapper.GetAssemblyName())) {
                assemblies[assemblyWrapper.GetAssemblyName()] = assemblyWrapper;
                return;
            }
            assemblies.Add(assemblyWrapper.GetAssemblyName(), assemblyWrapper);
        }

        /// <summary>
        /// Remove the assembly according to the provided assembly id. It should be noted that... it is not
        /// currently possible to remove an assembly from an app domain without destroying the domain itself.
        /// This method simply rids the assembly from the assembly store.
        /// </summary>
        /// <param id="assemblyFullName">The full id of the assembly to be removed.</param>
        public static void Remove(string assemblyFullName) {
            assemblies.Remove(assemblyFullName);
        }

        /// <summary>
        /// Remove the assembly according to the provided assembly wrapper. It should be noted that... it is not
        /// currently possible to remove an assembly from an app domain without destroying the domain itself.
        /// This method simply rids the assembly from the assembly store.
        /// </summary>
        /// <param id="assemblyWrapper">The assembly wrapper of the assembly to be removed.</param>
        public static void Remove(AssemblyWrapper assemblyWrapper) {
            assemblies.Remove(assemblyWrapper.GetAssemblyName());
        }

        /// <summary>
        /// Get an assembly wrapper for an assembly using the assembl full id as the index
        /// </summary>
        /// <param id="assemblyFullName">The full id of the assembly.</param>
        /// <returns>The assembly wrapper instance for that assembly.</returns>
        public static AssemblyWrapper Get(string assemblyFullName) {
            return assemblies[assemblyFullName];
        }
    }
}
