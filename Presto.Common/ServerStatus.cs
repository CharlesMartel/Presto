using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Common {
    [Serializable]
    public struct ServerStatus {
        public int NodeCount;
        public long TotalMemory;
        public long CDPI;
        public int CPUCount;
    }
}
