﻿using System;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Presto.Common.Net;
using Presto.Common;

namespace Presto {
    /// <summary>
    /// Holds each instance of connected Presto Servers, the connection to them, and information about the instance.
    /// </summary>
    public class Node {

        public bool Connected;
        public bool Available;

        private List<string> loadedAssemblies = new List<string>();
        private TCPClient client;
        private string address;
        private Timer pingTimer;
        private List<ExecutionContext> waitingJobs = new List<ExecutionContext>();
        private bool loadInProgress = false;

        /// <summary>
        /// Instantiate a new Node object with the connection to the specefied address
        /// </summary>
        /// <param name="connectionAddress">The connection address of the node</param>
        public Node(string connectionAddress) {
            //setup node
            address = connectionAddress;
            int port = int.Parse(Config.GetParameter("SERVER_PORT"));
            client = new TCPClient(address, port);

            //attatch network events
            client.setDispatchAction(MessageType.UNKOWN, unknowMessageType);
            client.setDispatchAction(MessageType.EXECUTION_COMPLETE, returnExecution);
            client.setDispatchAction(MessageType.EXECUTION_DENIED, deniedExecution);
            client.setDispatchAction(MessageType.CONNECTION_ACCEPTED, connectionAccept);
            client.setDispatchAction(MessageType.ASSEMBLY_TRANSFER_COMPLETE, assemblyLoaded);

            //connect
            client.Connect();     

            //start verification timer
            double hvi = Double.Parse(Config.GetParameter("NODE_VERIFY_INTERVAL"));
            pingTimer = new Timer(hvi*1000);
            pingTimer.AutoReset = true;
            pingTimer.Elapsed += new ElapsedEventHandler(pingTimer_Elapsed);
            pingTimer.Start();
        }

        /// <summary>
        /// Get the connection address of the node.
        /// </summary>
        /// <returns>the connection address string</returns>
        public string GetAddress() {
            return address;
        }

        /// <summary>
        /// Deliver an assembly to this node.
        /// </summary>
        public void DeliverAssembly(string assemblyFullName, byte[] assemblyArray) {
            client.Write(MessageType.ASSEMBLY_TRANSFER_SLAVE, assemblyArray);
            loadedAssemblies.Add(assemblyFullName);
            loadInProgress = true;
        }

        /// <summary>
        /// Trigger an execution on this node.
        /// </summary>
        /// <param name="executionContext">The execution context of the job.</param>
        public bool Execute(ExecutionContext executionContext) {
            //check if an assembly load is in progress and simply add the context to the list if so
            if (loadInProgress) {
                waitingJobs.Add(executionContext);
                return true;
            }
            //first be sure that this node has the appropriate assembly loaded
            if(!loadedAssemblies.Contains(executionContext.AssemblyName)){
                //get the assembly
                AssemblyWrapper assembly = AssemblyStore.Get(executionContext.AssemblyName);
                DeliverAssembly(assembly.GetAssemblyName(), assembly.GetAssemblyArray());
                //add to the list of waiting jobs
                waitingJobs.Add(executionContext);
                return true;
            }
            //since we know that the other machine has the assembly loaded we can 
            //serialize the execution context and transport
            MemoryStream stream = new MemoryStream();
            SoapFormatter serializer = new SoapFormatter();
            serializer.Serialize(stream, executionContext);
            client.Write(MessageType.EXECUTION_BEGIN, stream.ToArray());
            return true;
        }

        /// <summary>
        /// Tells whether or not this node has the specefied assembly.
        /// </summary>
        /// <param name="assemblyFullName">The full name of the assembly.</param>
        /// <returns>Whether or not the node has the assembly.</returns>
        public bool HasAssembly(string assemblyFullName) {
            return loadedAssemblies.Contains(assemblyFullName);
        }

        /// <summary>
        /// Will remove an assembly from the node according to the assmebly name.
        /// </summary>
        /// <param name="assemblyFullName">The full name of the assembly.</param>
        public void UnloadAssembly(string assemblyFullName) {
            //TODO: remove an assembly by name
        }


        /// <summary>
        /// Internal function to ping the recieving server and verify that there is an active connection to it.
        /// </summary>
        private void verify() {
            //TODO write verification method for nodes
        }

        /// <summary>
        /// calls the verification function based on the ping timers interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pingTimer_Elapsed(object sender, ElapsedEventArgs e) {
            verify();
        }

        //----------------------Response Functions-----------------------//
        private void connectionAccept(ClientState state) {
        }
        private void unknowMessageType(ClientState state) {    
        }
        private void returnExecution(ClientState state) {
        }
        private void deniedExecution(ClientState state) {
        }

        /// <summary>
        /// called when an assembly load message is returned
        /// </summary>
        /// <param name="state"></param>
        private void assemblyLoaded(ClientState state) {
            loadInProgress = false;
            foreach (ExecutionContext context in waitingJobs) {
                waitingJobs.Remove(context);
                Execute(context);
            }
        }
    }
}