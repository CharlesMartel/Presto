using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto;

namespace Basic
{
    [Serializable]
    class FunctionOutput : PrestoResult
    {
        public int value;
    }
}
