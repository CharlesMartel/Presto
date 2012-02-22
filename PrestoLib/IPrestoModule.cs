using System;

namespace Presto
{
    /// <summary>
    /// All modules that plug in to Presto must have one and only one class that implements the IPrestoModule interface. This interface serves as the starting point for
    /// execution and will be instantiated upon execution of the module.
    /// </summary>
    public interface IPrestoModule
    {
        /// <summary>
        /// After a new instance of the module is created. The Load method is immediately called to begin processing.
        /// </summary>
        void Load();
        /// <summary>
        /// After the module has finished executing, the Unload method gets called to allow the module to clean up behind itself before being destroyed.
        /// </summary>
        void Unload();
    }
}
