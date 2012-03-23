﻿using System;
using System.Collections.Generic;
using System.Timers;
using Presto.Common;
using Presto.Common.Net;
using System.Reflection;

namespace Presto {
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
        private TCPClient client;
        private string address;
        private Timer pingTimer;
        private System.Threading.ManualResetEvent assemblyLoadReset = new System.Threading.ManualResetEvent(false);

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
            client.Write(MessageType.ASSEMBLY_TRANSFER_SLAVE, assemblyArray);
            assemblyLoadReset.Reset();
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
            client.Write(MessageType.EXECUTION_BEGIN, SerializationEngine.Serialize(executionContext).ToArray());
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
        /// Whether or not the node is connected.
        /// </summary>
        /// <returns>True if connected, false if otherwise.</returns>
        public bool IsConnected() {
            return client.IsConnected();
        }

        /// <summary>
        /// Will remove an assembly from the node according to the assmebly id.
        /// </summary>
        /// <param id="assemblyFullName">The full id of the assembly.</param>
        public void UnloadAssembly(string assemblyFullName) {
            if(HasAssembly(assemblyFullName)){
                client.Write(MessageType.ASSEMBLY_UNLOAD, assemblyFullName);
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
            if(NodeID.Equals(Application.Cluster.NodeID)){
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
            res.Result.ExecutionNodeID = NodeID;
            Application.Cluster.ReturnExecution(res);
        }

        /// <summary>
        /// Called when a denied execution response is sent.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void deniedExecution(ClientState state) {
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
    }
}
