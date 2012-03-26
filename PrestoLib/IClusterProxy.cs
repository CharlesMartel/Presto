using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto {
    public interface IClusterProxy {
        void Execute(string assemblyName, string typeName, string methodName, byte[] param, string contextid, string domainKey);
    }
}
