using System;
namespace Presto {
    /// <summary>
    /// All modules that plug in to Presto must have one and only one class that implements the IPrestoModule interface. This interface serves as the starting point for
    /// execution and will be instantiated upon execution of the module.
    /// </summary>
    [Serializable()]
    public abstract class PrestoModule {
        /// <summary>
        /// The cluster that allows for interaction with the Presto cluster. When the PrestoModule is loaded, the PrestoServer assigns its internal cluster instance to this object. It is advised 
        /// to not override or overwrite this object.
        /// </summary>
        public ClusterBase Cluster = null;
        /// <summary>
        /// After a new instance of the module is created. The Load method is immediately called to begin processing.
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// After the module has finished executing, the Unload method gets called to allow the module to clean up behind itself before being destroyed.
        /// </summary>
        public abstract void Unload();

    }
}
