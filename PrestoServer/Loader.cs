using Presto.Common;
using Presto.Common.Net;
using Presto.Transfers;

namespace Presto {

    /// <summary>
    /// Attatches into the Application TCP server to recieve assembly load requests and
    /// pass the loaded assembly into the Executor.
    /// </summary>
    public static class Loader {

        /// <summary>
        /// Initializes the Loader and attatches it to the TCPServer.
        /// </summary>
        public static void Initialize() {
            Application.ControlServer.RegisterDispatchAction(MessageType.ASSEMBLY_TRANSFER_MASTER, recieveAssemblyMaster);
            Application.ControlServer.RegisterDispatchAction(MessageType.ASSEMBLY_TRANSFER_SLAVE, recieveAssemblySlave);
            Application.ControlServer.RegisterDispatchAction(MessageType.DOMAIN_UNLOAD, unloadDomain);
        }

        /// <summary>
        /// A dispatch event to be rasied upon reception of an assembly from the Presto client application.
        /// </summary>
        /// <param id="state">The server state object recieved along with this event.</param>
        private static void recieveAssemblyMaster(ServerState state) {
            //close the socket
            state.CloseSocket();
            //create a new domain and add the assembly to it
            string domainkey = Generator.RandomAlphaNumeric(Config.UIDLength);
            DomainManager.CreateDomain(domainkey);
            DomainManager.LoadAssemblyIntoDomain(domainkey, state.GetDataArray());
            //push assembly onto executor to be executed
            Executor.ExecuteModule(domainkey);
        }

        /// <summary>
        /// A dispatch event to be rasied upon reception of an assembly from another Presto server..
        /// </summary>
        /// <param id="state">The server state object recieved along with this event.</param>
        private static void recieveAssemblySlave(ServerState state) {
            //get the slave assembly struct
            SlaveAssembly slaveAssembly = (SlaveAssembly)SerializationEngine.Deserialize(state.GetDataArray());
            //create the domain and add the assembly to it
            DomainManager.CreateDomain(slaveAssembly.DomainKey);
            DomainManager.LoadAssemblyIntoDomain(slaveAssembly.DomainKey, slaveAssembly.AssemblyImage);
            //send back assembly transfer complete message
            state.Write(MessageType.ASSEMBLY_TRANSFER_COMPLETE);
        }

        /// <summary>
        /// Unload the assembly according to the assembly id;
        /// </summary>
        /// <param id="state">Server state object for request.</param>
        private static void unloadDomain(ServerState state) {
            //get the key of the domain
            string domainKey = state.GetDataASCIIString();
            //tell the domain manager to delete the domain
            DomainManager.DestroyDomain(domainKey);
        }
    }
}
