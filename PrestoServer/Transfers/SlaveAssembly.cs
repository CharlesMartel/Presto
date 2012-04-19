using System;

namespace Presto.Transfers {

    /// <summary>
    /// An assembly transfer structure.
    /// </summary>
    [Serializable]
    struct SlaveAssembly {
        /// <summary>
        /// The coff based image as a byte array.
        /// </summary>
        public byte[] AssemblyImage;
        /// <summary>
        /// The key of the domain this assembly belongs to.
        /// </summary>
        public string DomainKey;
        /// <summary>
        /// The full name of the assembly.
        /// </summary>
        public string AssemblyName;
        /// <summary>
        /// The node ID sending the assembly.
        /// </summary>
        public string NodeID;

        /// <summary>
        /// Create a new Slave Assembly structure.
        /// </summary>
        /// <param name="assemblyImage">The COFF based image of the assembly.</param>
        /// <param name="domainKey">The domain Key generated for the assembly.</param>
        /// <param name="assemblyName">The full name of the assembly.</param>
        /// <param name="nodeID">The node id from whence this assembly has come. Dun dun Dun</param>
        public SlaveAssembly(byte[] assemblyImage, string domainKey, string assemblyName, string nodeID) {
            AssemblyImage = assemblyImage;
            DomainKey = domainKey;
            AssemblyName = assemblyName;
            NodeID = nodeID;
        }
    }
}
