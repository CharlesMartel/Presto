using System;
namespace Presto {
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
        /// The number of logical processors this node has.
        /// </summary>
        public int CPUCount;
        /// <summary>
        /// The number of execution jobs the node currently has processing.
        /// </summary>
        public int JobCount;

        /// <summary>
        /// Create a new verification structure to be passed back from a verification request.
        /// </summary>
        /// <param name="id">The nodes Generated cluster ID.</param>
        /// <param name="dpi">The Distribution Performance Indicator for this node.</param>
        /// <param name="cpuCount">The number of logical processors this node has.</param>
        /// <param name="jobCount">The number of execution jobs the node currently has processing.</param>
        public Verification(string id, double dpi, int cpuCount, int jobCount) {
            NodeID = id;
            DPI = dpi;
            CPUCount = cpuCount;
            JobCount = jobCount;
        }
    }
}
