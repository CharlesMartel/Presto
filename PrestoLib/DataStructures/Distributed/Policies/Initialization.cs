using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed.Policies {

    /// <summary>
    /// Determines how the DataStructure's cluster state will be retrieved upon initialization.
    /// </summary>
    public enum Initialization {
        /// <summary>
        /// Upon initialization, no data will be fetched from the cluster, updates will happen lazily, as in, this
        /// instance will not actively sync its state but wait for updates or access attempts.
        /// </summary>
        LAZY
    }
}
