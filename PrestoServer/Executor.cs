using System;
using Presto.Common;
using Presto.Common.Net;

namespace Presto {

    /// <summary>
    /// Processes all Module startups and Cluster jobs. These jobs can be recieved either internally as when a job is to be
    /// executed using a previously loaded assembly, or externally as when the loader starts up a new assembly recieved from 
    /// the Presto client application.
    /// </summary>
    public static class Executor {

        /// <summary>
        /// Initializes the Executor.
        /// </summary>
        public static void Initialize() 
        {
        }

        /// <summary>
        /// Starts up the execution of a new module, making this instance the master.
        /// </summary>
        /// <param name="assemblyWrapper">An AssemblyWrapper around the assembly of the module to be executed.</param>
        public static void ExecuteModule(AssemblyWrapper assemblyWrapper) {

        }

    }
}
