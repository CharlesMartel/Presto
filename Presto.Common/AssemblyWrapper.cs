using System;
using System.Reflection;
using Presto;

namespace Presto.Common {
    /// <summary>
    /// The assembly wrapper provides basic functionality for loading, managing, and verifying assemblies.
    /// </summary>
    public class AssemblyWrapper {
        //The internal assembly
        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="Presto.Common.AssemblyWrapper"/> class.
        /// Initializes with an assembly URL which is the assemblys location on the file system.
        /// </summary>
        /// <param name='assemblyURL'>
        /// The assembl's file system URL
        /// </param>
        public AssemblyWrapper(string assemblyURL) {
            //TODO: Account for bad assemblies or unreachable files
            //load the assembly into the assembly internal assembly instance
            assembly = Assembly.LoadFrom(assemblyURL);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Presto.Common.AssemblyWrapper"/> class.
        /// Initializes with an assembly byte array. 
        /// </summary>
        /// <param name='assemblyBinaryArray'>
        /// Assembly byte array.
        /// </param>
        public AssemblyWrapper(byte[] assemblyBinaryArray) {
            //TODO: Account for bad assemblies
            //load the assembly into the internal assembly instance
            assembly = Assembly.Load(assemblyBinaryArray);
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
        public bool Validate() {
            //get all types housed in the assembly
            Type[] assemblyTypes = assembly.GetTypes();
            //count the number of types that derive from PrestoModule
            int count = 0;
            foreach(Type type in assemblyTypes)
            {
                if (type.IsSubclassOf(typeof(PrestoModule)))
                {
                    count++;
                }
            }
            // if there is exactly one class that derives PrestoModule then the assembly is valid
            if(count == 1){
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

    }
}

