using System;
namespace Presto {
    /// <summary>
    /// The verification sent back with a verfication request.
    /// </summary>
    [Serializable()]
    public struct Verification {
        public double DPI;
        public string NodeID;
        public int CPUCount;
        public int JobCount;

        public Verification(string id, double dpi, int cpuCount, int jobCount) {
            NodeID = id;
            DPI = dpi;
            CPUCount = cpuCount;
            JobCount = jobCount;
        }
    }
}
