﻿using System;
using Presto.Common;
using Presto.Common.Net;

namespace Presto {

    /// <summary>
    /// Attatches into the Application TCP server to recieve assembly load requests and
    /// pass the loaded assembly into the Executor.
    /// </summary>
    public static class Loader {

        /// <summary>
        /// Initializes the Loader and attatches it to the TCPServer.
        /// </summary>
        public static void Initialize()
        {
            Application.ControlServer.RegisterDispatchAction(MessageType.ASSEMBLY_TRANSFER_MASTER, recieveAssemblyMaster);
            Application.ControlServer.RegisterDispatchAction(MessageType.ASSEMBLY_TRANSFER_SLAVE, recieveAssemblySlave);
        }

        /// <summary>
        /// A dispatch event to be rasied upon reception of an assembly from the Presto client application.
        /// </summary>
        /// <param name="state">The server state object recieved along with this event.</param>
        private static void recieveAssemblyMaster(ServerState state)
        {
            //send back assembly transfer complete message
            state.WriteAndClose(MessageType.ASSEMBLY_TRANSFER_COMPLETE);
            //Instantiate a new assembly wrapper
            AssemblyWrapper assemblyWrapper = new AssemblyWrapper(state.GetDataArray(), Application.Cluster);
            //add assembly to assembly store
            AssemblyStore.Add(assemblyWrapper);
            //push assembly onto executor to be executed
            Executor.ExecuteModule(assemblyWrapper);
        }

        /// <summary>
        /// A dispatch event to be rasied upon reception of an assembly from another Presto server..
        /// </summary>
        /// <param name="state">The server state object recieved along with this event.</param>
        private static void recieveAssemblySlave(ServerState state) 
        {
            //send back assembly transfer complete message, we don't close here because for cluster operations we leave
            //the connection open
            state.Write(MessageType.ASSEMBLY_TRANSFER_COMPLETE);
            //Instantiate a new assembly wrapper
            AssemblyWrapper assemblyWrapper = new AssemblyWrapper(state.GetDataArray(), Application.Cluster);
            //add assembly to assembly store
            AssemblyStore.Add(assemblyWrapper);
        }
    }
}
