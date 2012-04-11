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
        /// instance will not actively sync its state but wait for updates.
        /// </summary>
        LAZY,
        /// <summary>
        /// Blocks the thread creating the cluster data structure and forces a complete download of the entire structure. 
        /// Only syncs if a fully distributed cluster model is chosen.
        /// </summary>
        SYNC
    }
}
