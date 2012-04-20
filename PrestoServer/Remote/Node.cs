using System;
using System.Collections.Generic;
using System.Timers;
using Presto.Common;
using Presto.Managers;
using Presto.Net;
using Presto.Transfers;

namespace Presto.Remote {
    /// <summary>
    /// Holds each instance of connected Presto Servers, the connection to them, and information about the instance.
    /// </summary>
    public class Node : ClusterNode {

        /// <summary>
        /// Boolean telling whether or not this node is available for distribution.
        /// </summary>
        public bool Available
        {
            get {
                return client.IsConnected();
            }
        }

        /// <summary>
        /// The number of logical cpus in this node.
        /// </summary>
        public int CPUCount
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of running jobs on this node.
        /// </summary>
        public int RunningJobs
        {
            get;
            private set;
        }

        /// <summary>
        /// The time (milliseconds) this node is away from the current one.
        /// </summary>
        public int Distance
        {
            get;
            private set;
        }

        /// <summary>
        /// The total amount of memory in this node.
        /// </summary>
        public long TotalMemory {
            get;
            private set;
        }

        /// <summary>
        /// A number defining how busy with work this node is. A factor dependant on Number of jobs,
        /// DPI, and total cpu count.
        /// </summary>
        public double Saturation
        {
            get
            {
                return (RunningJobs / CPUCount) / DPI;
            }
        }

        private List<string> loadedAssemblies = new List<string>();
        private List<string> loadedDomains = new List<string>();
        private TCPClient client;
        private Timer pingTimer;
        private System.Threading.ManualResetEvent assemblyLoadReset = new System.Threading.ManualResetEvent(true);

        /// <summary>
        /// Instantiate a new Node object with the connection to the specefied Address
        /// </summary>
        /// <param id="connectionAddress">The connection Address of the node</param>
        public Node(string connectionAddress) {
            //setup node
            Address = connectionAddress;
            int port = int.Parse(Config.GetParameter("SERVER_PORT"));
            client = new TCPClient(Address, port);

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
        /// Get the connection Address of the node.
        /// </summary>
        /// <returns>the connection Address string</returns>
        public string GetAddress() {
            return Address;
        }

        /// <summary>
        /// Deliver an assembly to this node.
        /// </summary>
        private void deliverAssembly(string assemblyFullName, byte[] assemblyArray, string domainKey) {
            if(!HasAssembly(assemblyFullName)){
                loadedAssemblies.Add(assemblyFullName);
            }
            if (!loadedDomains.Contains(domainKey)) {
                loadedDomains.Add(domainKey);
            }

            //make sure we dont deliver the same assembly to ourselves
            if (NodeID != ClusterManager.NodeID) {
                SlaveAssembly slavePackage = new SlaveAssembly(assemblyArray, domainKey, assemblyFullName, ClusterManager.NodeID);
                SerializationEngine serializer = new SerializationEngine ();
                client.Write(MessageType.ASSEMBLY_TRANSFER_SLAVE, serializer.Serialize(slavePackage));
                assemblyLoadReset.Reset();
            }
        }

        /// <summary>
        /// Trigger an execution on this node.
        /// </summary>
        /// <param id="executionContext">The execution context of the job.</param>
        public bool Execute(ExecutionContext executionContext) {
            RunningJobs++;
            //first be sure that this node has the appropriate assembly loaded
            if (!HasAssembly(executionContext.AssemblyName)) {
                //get the assembly
                byte[] assembly = DomainManager.GetAssemblyStream(executionContext.AssemblyName);
                deliverAssembly(executionContext.AssemblyName, assembly, executionContext.DomainKey);
            }
            assemblyLoadReset.WaitOne();
            //since we know that the other machine has the assembly loaded we can 
            //serialize the execution context and transport
            SerializationEngine serializer = new SerializationEngine ();
            client.Write(MessageType.EXECUTION_BEGIN, serializer.Serialize(executionContext));            
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
        /// <param name="domainKey">The Key of the specefied domain.</param>
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
        /// Will remove a domain from the node according to the domain Key.
        /// </summary>
        /// <param name="domainKey">The domain Key of the domain.</param>
        /// <param name="assemblies">A string array of all the assemblies to be removed with the domain.</param>
        public void UnloadDomain(string domainKey, string[] assemblies) {
            if (loadedDomains.Contains(domainKey)) {
                client.Write(MessageType.DOMAIN_UNLOAD, domainKey);
                loadedDomains.Remove(domainKey);
            }
            foreach (string assem in assemblies) {
                if (loadedAssemblies.Contains(assem)) {
                    loadedAssemblies.Remove(assem);
                }
            }
        }


        /// <summary>
        /// Internal function to ping the recieving server and verify that there is an active connection to it.
        /// </summary>
        private void verify() {
            if (Available) {
                client.Write(MessageType.VERIFY);
            } else {
                client.ReConnect();
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
            if (NodeID.Equals(ClusterManager.NodeID)) {
                return true;
            }
            return false;
        }

        //----------------------Response Functions-----------------------//

        /// <summary>
        /// The sent message is of unknown type.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void unknowMessageType(ClientState state) {
            //TODO: handle unknown message type
        }

        /// <summary>
        /// A distributed job has completed succesfully and this is the response.
        /// </summary>
        /// <param id="state">The state object of the response.</param>
        private void returnExecution(ClientState state) {
            SerializationEngine serializer = new SerializationEngine ();
            ExecutionResult res = (ExecutionResult)serializer.Deserialize(state.GetDataArray());
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
            SerializationEngine serializer = new SerializationEngine ();
            Verification vResponse = (Verification)serializer.Deserialize(state.GetDataArray());
            NodeID = vResponse.NodeID;
            HostName = vResponse.HostName;
            DPI = vResponse.DPI;
            CPUCount = vResponse.CPUCount;
            RunningJobs = vResponse.JobCount;
            TotalMemory = vResponse.TotalMemory;
            //make sure the assembly and domain listing is good
            foreach (string assembly in loadedAssemblies)
            {
                if(!loadedAssemblies.Contains(assembly)){
                    loadedAssemblies.Add(assembly);
                }
            }
            foreach (string domain in loadedDomains)
            {
                if (!loadedDomains.Contains(domain))
                {
                    loadedDomains.Add(domain);
                }
            }
        }

        /// <summary>
        /// Send a message to the node with the specified ID.
        /// </summary>
        /// <param name="message">The user message struct to be sent.</param>
        public void SendMessage(UserMessage message) {
            if (!HasDomain(message.DomainKey))
            {
                //get the assemblies
                Dictionary<string, byte[]> assemblies = DomainManager.GetDomainAssemblies(message.DomainKey);
                foreach (KeyValuePair<string, byte[]> assem in assemblies)
                {
                    deliverAssembly(assem.Key, assem.Value, message.DomainKey);
                }
            }
            assemblyLoadReset.WaitOne();
            SerializationEngine serializer = new SerializationEngine ();
            client.Write(MessageType.USER_MESSAGE, serializer.Serialize(message));
        }

        /// <summary>
        /// Adds an assembly name to the list of loaded assemblies for this node.
        /// </summary>
        /// <param name="assemblyName">The assembly name to add.</param>
        public void SetLoadedAssembly(string assemblyName)
        {
            if (!loadedAssemblies.Contains(assemblyName))
            {
                loadedAssemblies.Add(assemblyName);
            }
        }

        /// <summary>
        /// Adds a domain to the list of loaded domains for this node.
        /// </summary>
        /// <param name="domainKey">The key of the domain to be added.</param>
        public void SetLoadedDomain(string domainKey)
        {
            if (!loadedDomains.Contains(domainKey))
            {
                loadedDomains.Add(domainKey);
            }
        }
    }
}
