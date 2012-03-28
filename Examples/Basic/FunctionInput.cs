using System;
using Presto;

namespace Basic {
    [Serializable]
    class FunctionInput : PrestoParameter {
        public int value = 0;
    }
}
