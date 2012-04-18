using Presto.Common;
using Presto.Machine;
using Presto.Managers;
using Presto.Net;
using Presto.Remote;

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
            ControlServer.RegisterDispatchAction(MessageType.STATUS_TERMINAL, status);
            //Initialize the Node Listing
            Nodes.Initialize();
        }

        /// <summary>
        /// The dispatch action called when a status request is signaled.
        /// </summary>
        /// <param name="state">The server state object of the network connection.</param>
        public static void status (ServerState state){
            ServerStatus status = new ServerStatus();
            status.NodeCount = Nodes.GetTotalAvailableNodeCount();
            status.CPUCount = Nodes.GetCPUCount();
            status.CDPI = Nodes.GetCDPI();
            status.TotalMemory = Nodes.GetTotalMemory();
            SerializationEngine serializer = new SerializationEngine();
            state.WriteAndClose(MessageType.STATUS_TERMINAL, serializer.Serialize(status));
        }

    }
}
