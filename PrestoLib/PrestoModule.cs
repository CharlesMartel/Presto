using System;

namespace Presto
{
    /// <summary>
    /// All modules that plug in to Presto must have one and only one class that implements the PrestoModule interface. This interface serves as the starting point for
    /// execution and will be instantiated upon execution of the module.
    /// </summary>
    public interface PrestoModule
    {
        /// <summary>
        /// After a new instance of the module is created. The load method is immediately called to begin processing.
        /// </summary>
        void load();
        /// <summary>
        /// After the module has finished executing, the unload method gets called to allow the module to clean up behind itself before being destroyed.
        /// </summary>
        void unload();
    }
}
