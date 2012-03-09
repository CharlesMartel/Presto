using System;

namespace Presto {
    /// <summary>
    /// A defining class for the PrestoServer internal cluster object. 
    /// </summary>
    public abstract class ClusterBase {
        //------------Properties--------------------//

        /// <summary>
        /// CDPI or "Cluster Distribution Performance Indicator" is the total distribution performance for the entire cluster.
        /// This value is not relative, it is based on the individual DPI of each node, the total processing power of the cluster, the number of individual
        /// processes runnable, and the performance of the clusters communications channel. It is a "score" for the cluster as 
        /// a whole.
        /// </summary>
        public long CDPI;


        //------------Methods-----------------------//

        /// <summary>
        /// Deploys an execution job into the cluster. 
        /// 
        /// The method passed in as "function" MUST be static. If it is not, an error will be thrown and added to the error log.
        /// Instance data is not preserved outside of the running ApplicationDomain and indeed all data not instantiated within the 
        /// method or not globally synchronized by the SynchronizationFactory is considered volatile, mutable, inconsitent and untrustworthy.
        /// 
        /// DO NOT write code that will depend on instance or static class variables in order to do processing
        /// unless those variables are declared constant.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed into the executed function.</param>
        /// <param name="callback">The callback to be executed when the function completes.</param>
        public abstract void Execute(Func<PrestoParameter, PrestoResult> function, PrestoParameter parameter, Action<PrestoResult> callback);
        /// <summary>
        /// Blocks the currently running thread until all execution jobs return from the cluster. The thread will resume once all jobs return succesful.
        /// </summary>
        public abstract void Wait();

    }
}
