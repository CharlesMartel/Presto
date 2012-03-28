using System;
namespace Presto {
    /// <summary>
    /// All modules that plug in to Presto must have one and only one class that implements the IPrestoModule interface. This interface serves as the starting point for
    /// execution and will be instantiated upon execution of the module.
    /// </summary>
    [Serializable()]
    public abstract class PrestoModule {
        /// <summary>
        /// After a new instance of the module is created. The Load method is immediately called to begin processing.
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// After the module has finished executing, the Unload method gets called to allow the module to clean up behind itself before being destroyed.
        /// </summary>
        public void Unload() { }
        /// <summary>
        /// Signal to the controlling presto server that the currently running module has finished its work
        /// and is ready to be disposed.
        /// </summary>
        public void SignalComplete() {
            Unload();
            Cluster.ClusterProxy.SignalComplete(Cluster.GetInstanceKey());
        }
    }
}
