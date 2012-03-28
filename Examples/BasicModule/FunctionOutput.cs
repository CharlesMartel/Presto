using System;
using Presto;

namespace BasicModule {
    [Serializable]
    class FunctionOutput : PrestoResult {
        public int value;
    }
}
