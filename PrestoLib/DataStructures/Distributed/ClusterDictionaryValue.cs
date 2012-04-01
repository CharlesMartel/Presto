using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed {
    /// <summary>
    /// A value holder for cluster dictionary values. Also keeps data stored alongside the value sch as timestamp.
    /// </summary>
    class ClusterDictionaryValue<T> where T : struct {

        T Value;
        long TimeStamp;

        public ClusterDictionaryValue (T value, long timeStamp) {
            Value = value;
            TimeStamp = timeStamp;
        }
    }
}
