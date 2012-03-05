using Presto.Common.Machine;
using Presto.Common.Net;

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
        /// The PrestoServer Internal Cluster instance.
        /// </summary>
        public static Cluster Cluster = new Cluster();



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
            Loader.Initialize();
            Executor.Initialize();
            Cluster.Initialize();
            //Start the server listening thread
            ControlServer.Start();
            //Initialize the Node Listing
            Nodes.Initialize();
        }

    }
}
