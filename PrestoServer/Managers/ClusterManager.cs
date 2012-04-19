using System.Linq;
using Presto.Common;
using Presto.Machine;
using Presto.Net;
using Presto.Transfers;

namespace Presto.Managers {

    /// <summary>
    /// Each loaded module or application will have its own cluster object assigned to it. This global cluster allows for management of those cluster objects and an
    /// "aerial" view of the cluster as a whole.
    /// </summary>
    public static class ClusterManager {

        /// <summary>
        /// The generated node id of this node.
        /// </summary>
        public static readonly string NodeID = Generator.RandomAlphaNumeric(Config.UIDLength);

        /// <summary>
        /// The hostname of this node.
        /// </summary>
        public static readonly string HostName = System.Net.Dns.GetHostName();

        /// <summary>
        /// Initialize the servers cluster instance.
        /// </summary>
        public static void Initialize() {
            Application.ControlServer.RegisterDispatchAction(MessageType.VERIFY, verifyResponse);
            Application.ControlServer.RegisterDispatchAction(MessageType.USER_MESSAGE, receiveMessage);
        }

        /// <summary>
        /// This server needs to verify itself and send back a Verfication object.
        /// </summary>
        /// <param id="state">The server state object.</param>
        private static void verifyResponse(ServerState state) {
            Transfers.Verification verification = new Transfers.Verification(NodeID, ClusterManager.HostName, DPI.GetDPI(), Memory.GetTotalSize(), CPU.GetCount(), Executor.RunningJobs(), DomainManager.GetAllDomainKeys(), DomainManager.GetAllAssemblyNames());
            SerializationEngine serializer = new SerializationEngine ();
            state.Write(MessageType.VERIFICATION_RESPONSE, serializer.Serialize(verification).ToArray());
        }

        /// <summary>
        /// Recieve a user message from a remote node.
        /// </summary>
        /// <param name="state">The server state object of this transfer.</param>
        private static void receiveMessage(ServerState state) {
            SerializationEngine serializer = new SerializationEngine ();
            UserMessage message = (UserMessage)serializer.Deserialize(state.GetDataArray());
            DomainManager.DeliverMessage(message);
        }
    }
}
