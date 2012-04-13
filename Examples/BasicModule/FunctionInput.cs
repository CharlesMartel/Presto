using System;
using Presto;

namespace BasicModule {
    [Serializable]
    class FunctionInput : PrestoParameter {
        public int value = 0;
    }
}
