using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Transfers {

    /// <summary>
    /// An assembly transfer structure.
    /// </summary>
    struct SlaveAssembly {
        public byte[] AssemblyImage;
        public string DomainKey;

        /// <summary>
        /// Create a new Slave Assembly structure.
        /// </summary>
        /// <param name="assemblyImage">The COFF based image of the assembly.</param>
        /// <param name="domainKey">The domain key generated for the assembly.</param>
        public SlaveAssembly(byte[] assemblyImage, string domainKey) {
            AssemblyImage = assemblyImage;
            DomainKey = domainKey;
        }
    }
}
