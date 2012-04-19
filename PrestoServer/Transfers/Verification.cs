using System;
namespace Presto.Transfers {

    /// <summary>
    /// The verification sent back with a verfication request.
    /// </summary>
    [Serializable()]
    public struct Verification {
        /// <summary>
        /// The Distribution Performance Indicator for this node.
        /// </summary>
        public double DPI;
        /// <summary>
        /// The nodes Generated cluster ID.
        /// </summary>
        public string NodeID;
        /// <summary>
        /// The HostNamee of this node.
        /// </summary>
        public string HostName;
        /// <summary>
        /// The total amount of memory in this node.
        /// </summary>
        public long TotalMemory;
        /// <summary>
        /// The number of logical processors this node has.
        /// </summary>
        public int CPUCount;
        /// <summary>
        /// The number of execution jobs the node currently has processing.
        /// </summary>
        public int JobCount;
        /// <summary>
        /// The domains currently loaded into this node.
        /// </summary>
        public string[] Domains;
        /// <summary>
        /// The assemblies currently loaded into this node.
        /// </summary>
        public string[] Assemblies;

        /// <summary>
        /// Create a new verification structure to be passed back from a verification request.
        /// </summary>
        /// <param name="id">The nodes Generated cluster ID.</param>
        /// <param name="hostname">The hostame of the node responding with this Verfication.</param>
        /// <param name="totalMem">The total amount of memory in this node.</param>
        /// <param name="assemblies">The assemblies currently loaded into this node.</param>
        /// <param name="domains">The domains currently loaded into this node.</param>
        /// <param name="dpi">The Distribution Performance Indicator for this node.</param>
        /// <param name="cpuCount">The number of logical processors this node has.</param>
        /// <param name="jobCount">The number of execution jobs the node currently has processing.</param>
        public Verification(string id, string hostname, double dpi, long totalMem, int cpuCount, int jobCount, string[] domains, string[] assemblies) {
            NodeID = id;
            DPI = dpi;
            TotalMemory = totalMem;
            CPUCount = cpuCount;
            JobCount = jobCount;
            Domains = domains;
            Assemblies = assemblies;
            HostName = hostname;
        }
    }
}
