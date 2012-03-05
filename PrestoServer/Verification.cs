using System;
namespace Presto {
    /// <summary>
    /// The verification sent back with a verfication request.
    /// </summary>
    [Serializable()]
    public struct Verification {
        public double DPI;
        public string Name;

        public Verification(string name, double dpi) {
            Name = name;
            DPI = dpi;
        }
    }
}
