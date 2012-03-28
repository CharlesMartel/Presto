﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Timers;
using System.Reflection;
using Presto.Common;
using Presto.Net;
using Presto.Transfers;
using Presto.Managers;

namespace Presto.Remote {
    /// <summary>
    /// Holds each instance of connected Presto Servers, the connection to them, and information about the instance.
    /// </summary>
    public class Node {

        /// <summary>
        /// Boolean telling whether or not this node is available for distribution.
        /// </summary>
        public bool Available = false;

        /// <summary>
        /// The generated id of this node.
        /// </summary>
        public string NodeID;

        /// <summary>
        /// The calculated DPI of this node.
        /// </summary>
        public double DPI;

        /// <summary>
        /// The number of logical cpus in this node.
        /// </summary>
        public int CPUCount;

        /// <summary>
        /// The number of running jobs on this node.
        /// </summary>
        public int RunningJobs;

        private List<string> loadedAssemblies = new List<string>();
        private List<string> loadedDomains = new List<string>();
        private TCPClient client;
        private string address;
        private Timer pingTimer;
        private System.Threading.ManualResetEvent assemblyLoadReset = new System.Threading.ManualResetEvent(true);

        /// <summary>
        /// Instantiate a new Node object with the connection to the specefied address
        /// </summary>
        /// <param id="connectionAddress">The connection address of the node</param>
        public Node(string connectionAddress) {
            //setup node
            address = connectionAddress;
            int port = int.Parse(Config.GetParameter("SERVER_PORT"));
            client = new TCPClient(address, port);

            //attatch network events
            client.SetDispatchAction(MessageType.UNKOWN, unknowMessageType);
            client.SetDispatchAction(MessageType.EXECUTION_COMPLETE, returnExecution);
            client.SetDispatchAction(MessageType.EXECUTION_DENIED, deniedExecution);
            client.SetDispatchAction(MessageType.ASSEMBLY_TRANSFER_COMPLETE, assemblyLoaded);
            client.SetDispatchAction(MessageType.VERIFICATION_RESPONSE, verificationResponse);

            //connect
            client.Connect();

            //Send first verification.
            verify();

            //start verification timer
            double hvi = Double.Parse(Config.GetParameter("NODE_VERIFY_INTERVAL"));
            pingTimer = new Timer(hvi * 1000);
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
        public void DeliverAssembly(string assemblyFullName, byte[] assemblyArray, string domainKey) {
            loadedAssemblies.Add(assemblyFullName);
            if(!loadedDomains.Contains(domainKey)){
                loadedDomains.Add(domainKey);
            }

            if (NodeID != ClusterManager.NodeID) {
                SlaveAssembly slavePackage = new SlaveAssembly(assemblyArray, domainKey);
                client.Write(MessageType.ASSEMBLY_TRANSFER_SLAVE, SerializationEngine.Serialize(slavePackage));
                assemblyLoadReset.Reset();
            }
        }

        /// <summary>
        /// Trigger an execution on this node.
        /// </summary>
        /// <param id="executionContext">The execution context of the job.</param>
        public bool Execute(ExecutionContext executionContext) {
            //first be sure that this node has the appropriate assembly loaded
            if (!loadedAssemblies.Contains(executionContext.AssemblyName)) {
                //get the assembly
                byte[] assembly = DomainManager.GetAssemblyStream(executionContext.AssemblyName);
                DeliverAssembly(executionContext.AssemblyName, assembly, executionContext.DomainKey);
            }
            assemblyLoadReset.WaitOne();
            //since we know that the other machine has the assembly loaded we can 
            //serialize the execution context and transport
            client.Write(MessageType.EXECUTION_BEGIN, SerializationEngine.Serialize(executionContext));
            RunningJobs++;
            return true;
        }

        /// <summary>
        /// Tells whether or not this node has the specified assembly.
        /// </summary>
        /// <param id="assemblyFullName">The full id of the assembly.</param>
        /// <returns>Whether or not the node has the assembly.</returns>
        public bool HasAssembly(string assemblyFullName) {
            return loadedAssemblies.Contains(assemblyFullName);
        }

        /// <summary>
        /// Tells whether or not this node has the specefied domain loaded.
        /// </summary>
        /// <param name="domainKey">The key of the specefied domain.</param>
        /// <returns>Whether or not the node has the domain.</returns>
        public bool HasDomain(string domainKey) {
            return loadedDomains.Contains(domainKey);
        }

        /// <summary>
        /// Whether or not the node is connected.
        /// </summary>
        /// <returns>True if connected, false if otherwise.</returns>
        public bool IsConnected() {
            return client.IsConnected();
        }

        /// <summary>
        /// Will remove a domain from the node according to the domain key.
        /// </summary>
        /// <param name="domainKey">The domain key of the domain.</param>
        /// <param name="assemblies">A string array of all the assemblies to be removed with the domain.</param>
        public void UnloadDomain(string domainKey, string[] assemblies) {
            if(loadedDomains.Contains(domainKey)){
                client.Write(MessageType.DOMAIN_UNLOAD, domainKey);
                loadedDomains.Remove(domainKey);
            }
            foreach (string assem in assemblies) {
                if(loadedAssemblies.Contains(assem)){
                    loadedAssemblies.Remove(assem);
                }
            }
        }


        /// <summary>
        /// Internal function to ping the recieving server and verify that there is an active connection to it.
        /// </summary>
        private void verify() {
            if (client.IsConnected()) {
                client.Write(MessageType.VERIFY);
            }
            else {
                Available = false;
                bool connected = client.ReConnect();
                if (connected)
                {
                    Available = true;
                }
            }
        }

        /// <summary>
        /// calls the verification function based on the ping timers interval
        /// </summary>
        /// <param id="sender"></param>
        /// <param id="e"></param>
        private void pingTimer_Elapsed(object sender, ElapsedEventArgs e) {
            verify();
        }

        /// <summary>
        /// Boolean telling if this node is the current machine.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool IsLocal() {
            if(NodeID.Equals(ClusterManager.NodeID)){
                return true;
            }
            return false;
        }

        /// <summary>
        /// Estimates the current load on the node using the verifcation response data.
        /// </summary>
        /// <returns>The estimated load on the node.</returns>
        public float EstimatedLoad() {
            return (float)RunningJobs / CPUCount;
        }


        //----------------------Response Functions-----------------------//

        /// <summary>
        /// The sent message is of unknown type.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void unknowMessageType(ClientState state) {
        }

        /// <summary>
        /// A distributed job has completed succesfully and this is the response.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void returnExecution(ClientState state) {
            ExecutionResult res = (ExecutionResult)SerializationEngine.Deserialize(state.GetDataArray());
            DomainManager.ReturnExecution(res);
        }

        /// <summary>
        /// Called when a denied execution response is sent.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void deniedExecution(ClientState state) {
            //TODO: handle denied execution.
        }

        /// <summary>
        /// called when an assembly load message is returned
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void assemblyLoaded(ClientState state) {
            assemblyLoadReset.Set();
        }

        /// <summary>
        /// A verification response has been returned.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void verificationResponse(ClientState state) {
            Verification vResponse = (Verification)SerializationEngine.Deserialize(state.GetDataArray());
            NodeID = vResponse.NodeID;
            DPI = vResponse.DPI;
            CPUCount = vResponse.CPUCount;
            RunningJobs = vResponse.JobCount;
        }

        /// <summary>
        /// Send a message to the node with the specified ID. The message is UTF8 encoded on transport and is delivered to 
        /// the receiving node calling MessageReceived event.
        /// </summary>
        /// <param name="message">The user message struct to be sent.</param>
        public void SendMessage(UserMessage message) {
            client.Write(MessageType.USER_MESSAGE, SerializationEngine.Serialize(message));
        }
    }
}