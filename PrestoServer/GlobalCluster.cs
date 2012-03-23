using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto.Common;
using Presto.Common.Net;
using Presto.Common.Machine;

namespace Presto {

    /// <summary>
    /// Each loaded module or application will have its own cluster object assigned to it. This global cluster allows for management of those cluster objects and an
    /// "aerial" view of the cluster as a whole.
    /// </summary>
    public static class GlobalCluster {

        //internal hashmap of clusters and their domain keys
        private static Dictionary<string, Cluster> clusters = new Dictionary<string, Cluster>();

        /// <summary>
        /// The generated node id of this node.
        /// </summary>
        public static string NodeID = Generator.RandomAlphaNumeric(Config.UIDLength);

        /// <summary>
        /// Initialize the servers cluster instance.
        /// </summary>
        public static void Initialize() {
            Application.ControlServer.RegisterDispatchAction(MessageType.VERIFY, verifyResponse);
        }

        /// <summary>
        /// Creates a cluster object according to a provided domain key. This cluster object is then trackable by this domain key which is shared
        /// amongst all cluster nodes. If a cluster object already exists for the specefied key, that cluster object is returned.
        /// </summary>
        /// <param name="domainKey">The domain key of the cluster.</param>
        public static Cluster CreateCluster(string domainKey){
            if(clusters.ContainsKey(domainKey)){
                return clusters[domainKey];
            }
            Cluster newCluster = new Cluster(domainKey);
            return newCluster;
        }

        /// <summary>
        /// Will destory the cluster object found at the provided domain key. If one is not found at the specefied key, does nothing.
        /// </summary>
        /// <param name="domainKey">The key of the domain the cluster object belongs to.</param>
        public static void DestroyCluster(string domainKey){
            if (clusters.ContainsKey(domainKey)) {
                clusters.Remove(domainKey);
            }
        }

        /// <summary>
        /// Gets the cluster object with the corresponding domain key. If one does not exist, returns null.
        /// </summary>
        /// <param name="domainKey">The domain key associated with the cluster.</param>
        /// <returns>The cluster object or null if not one.</returns>
        public static Cluster GetCluster(string domainKey) {
            if (clusters.ContainsKey(domainKey)) {
                return clusters[domainKey];
            }
            return null;
        }

        /// <summary>
        /// This server needs to verify itself and send back a Verfication object.
        /// </summary>
        /// <param id="state">The server state object.</param>
        private static void verifyResponse(ServerState state) {
            Transfers.Verification verification = new Transfers.Verification(NodeID, DPI.GetDPI(), CPU.GetCount(), Executor.RunningJobs());
            state.Write(MessageType.VERIFICATION_RESPONSE, SerializationEngine.Serialize(verification).ToArray());
        }




    }
}
