using System;
using Presto;

namespace Basic {
    [Serializable]
    class FunctionOutput : PrestoResult {
        public int value;
    }
}
