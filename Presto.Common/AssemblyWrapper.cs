using System;
using System.Reflection;

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
            //load the assembly into the assembly  internal assembly instance
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
        public Assembly getAssembly() {
            return assembly;
        }

        /// <summary>
        /// Validate that the assembly is usable by Presto..
        /// </summary>
        public bool validate() {
            //TODO: validate assembly
            return false;
        }

    }
}

