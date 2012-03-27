using Presto.Machine;
using Presto.Net;

namespace Presto {
    /// <summary>
    /// The static application class holds all top level and static instances for the application. This architecture is used to allow a very functional approach to the
    /// application design.
    /// </summary>
    public static class Application {
        /// <summary>
        /// The control server is the server that messaging, jobs, and file transfers occur on.
        /// </summary>
        public static TCPServer ControlServer = new TCPServer(2500);



        /// <summary>
        /// Initializes the application.
        /// </summary>
        public static void Initialize() {
            //Initialize configuration
            Config.Initialize();
            //Initialize Counters
            CPU.GetUsage();
            Memory.GetTotalSize();
            DPI.CalculateDPI();
            //Initialize subsystems
            ClusterManager.Initialize();
            Loader.Initialize();
            Executor.Initialize();
            //Start the server listening thread
            ControlServer.Start();
            //Initialize the Node Listing
            Nodes.Initialize();
        }

    }
}
