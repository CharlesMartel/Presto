using System;

namespace Presto {
    /// <summary>
    /// All returning data to be passed around the cluster must inherit from PrestoResult
    /// </summary>
    [Serializable]
    public abstract class PrestoResult {
        /// <summary>
        /// The node that this result comes from. This node is the node that ran the execution that produced this result.
        /// This value is null during the execution and only set after the execution is run and returned to the origin.
        /// </summary>
        [NonSerialized] public ClusterNode ExecutionNode;
    }
}
