using System;
using System.Reflection;

namespace Presto.Common {
    /// <summary>
    /// The assembly wrapper provides basic functionality for loading, managing, and verifying assemblies.
    /// </summary>
    public class AssemblyWrapper {
        //The internal assembly
        private Assembly assembly;
        private PrestoModule module;
        private byte[] assemblyArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="Presto.Common.AssemblyWrapper"/> class.
        /// Initializes with an assembly URL which is the assemblys location on the file system.
        /// </summary>
        /// <param name='assemblyURL'>The assembly's file system URL</param>
        public AssemblyWrapper(string assemblyURL, ClusterBase clusterInstance) {
            //TODO: Account for bad assemblies or unreachable files
            //load the assembly into the assembly internal assembly instance
            assembly = Assembly.Load(assemblyURL);
            createModuleInstance(clusterInstance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Presto.Common.AssemblyWrapper"/> class.
        /// Initializes with an assembly byte array. 
        /// </summary>
        /// <param name='assemblyBinaryArray'>
        /// Assembly byte array.
        /// </param>
        public AssemblyWrapper(byte[] assemblyBinaryArray, ClusterBase clusterInstance) {
            //TODO: Account for bad assemblies
            //load the assembly into the internal assembly instance
            assembly = Assembly.Load(assemblyBinaryArray);
            assemblyArray = assemblyBinaryArray;
            createModuleInstance(clusterInstance);
        }

        /// <summary>
        /// Gets the assembly.that this assembly wrapper wraps.
        /// </summary>
        /// <returns>
        /// The assembly.
        /// </returns>
        public Assembly GetAssembly() {
            return assembly;
        }

        /// <summary>
        /// Validate that the assembly is usable by Presto..
        /// </summary>
        public static bool Validate(Assembly assembly) {
            //get all types housed in the assembly
            Type[] assemblyTypes = assembly.GetTypes();
            //count the number of types that derive from PrestoModule
            int count = 0;
            foreach (Type type in assemblyTypes) {
                if (type.IsSubclassOf(typeof(PrestoModule))) {
                    count++;
                }
            }
            // if there is exactly one class that derives PrestoModule then the assembly is valid
            if (count == 1) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the name of the assembly.
        /// </summary>
        /// <returns></returns>
        public string GetAssemblyName() {
            return assembly.GetName().FullName;
        }

        /// <summary>
        /// Get the PrestoModule instance associated with this assembly
        /// </summary>
        /// <returns></returns>
        public PrestoModule GetModuleInstance() {
            return module;
        }

        /// <summary>
        /// Initializes the presto module instance 
        /// </summary>
        /// <param name="clusterInstance"></param>
        private void createModuleInstance(ClusterBase clusterInstance) {
            //get all types housed in the assembly
            Type[] assemblyTypes = assembly.GetTypes();
            //create an instance of the PrestoModule
            foreach (Type type in assemblyTypes) {
                if (type.IsSubclassOf(typeof(PrestoModule))) {
                    module = (PrestoModule)Activator.CreateInstance(type);
                    module.Cluster = clusterInstance;
                    break;
                }
            }
        }

        /// <summary>
        /// Get the byte array of the assembly.
        /// </summary>
        /// <returns>The byte array of the assembly.</returns>
        public byte[] GetAssemblyArray() {
            return assemblyArray;
        }

    }
}

