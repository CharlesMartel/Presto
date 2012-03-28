using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.Common;
using Presto.Net;
using Presto.Machine;
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
        public static string NodeID = Generator.RandomAlphaNumeric(Config.UIDLength);

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
            Transfers.Verification verification = new Transfers.Verification(NodeID, DPI.GetDPI(), CPU.GetCount(), Executor.RunningJobs());
            state.Write(MessageType.VERIFICATION_RESPONSE, SerializationEngine.Serialize(verification).ToArray());
        }

        /// <summary>
        /// Recieve a user message from a remote node.
        /// </summary>
        /// <param name="state">The server state object of this transfer.</param>
        private static void receiveMessage(ServerState state) {
            UserMessage message = (UserMessage)SerializationEngine.Deserialize(state.GetDataArray());
            DomainManager.DeliverMessage(message);
        }
    }
}
