using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed.Policies {

    /// <summary>
    /// Specifies the intervals for which changed data should be updated throughout the cluster.
    /// </summary>
    public enum UpdateInterval {
        /// <summary>
        /// Update data messages are sent every time a datum is changed.
        /// </summary>
        IMMEDIATE,
        /// <summary>
        /// Update data messages are sent according to a a specified update interval.
        /// </summary>
        INTERVAL
    }
}
