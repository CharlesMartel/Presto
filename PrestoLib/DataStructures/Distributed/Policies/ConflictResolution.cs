using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed.Policies {
    
    /// <summary>
    /// Policies determining the correct way to handle distributed data conflicts.
    /// </summary>
    public enum ConflictResolution {
        /// <summary>
        /// According to this policy, data that has the latest timestamp, or was changed last in chronological order (i.e. 3:00 is before 3:01),
        /// is the accepted correct state of the data. 
        /// </summary>
        CHRONOLOGICAL
    }
}
