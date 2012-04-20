using System;
using System.Collections.Concurrent;
using System.Threading;
using Presto.Common;
using Presto.Managers;

namespace Presto {
    /// <summary>
    /// The cluster object that allows for interaction with the Presto cluster.
    /// </summary>  
    public static class Cluster {
        //-----------Events-------------------------//

        /// <summary>
        /// Event handler for whenever a message from an outside node is sent to the current one.
        /// </summary>
        /// <param name="payload">The message sent.</param>
        /// <param name="sender"> The node ID of the sending Node.</param>
        public delegate void MessageReceivedHandler(string payload, ClusterNode sender);
        /// <summary>
        /// The events called whenever a message is received from an outside node.
        /// </summary>
        public static event MessageReceivedHandler MessageRecieved;

        /// <summary>
        /// Event handler triggered when a Module or Application is being deconstructed and removed from the cluster.
        /// </summary>
        public delegate void UnloadingHandler();
        /// <summary>
        /// Event triggered directly before the deconstruction and removal of a module or application from the cluster.
        /// </summary>
        public static event UnloadingHandler Unloading;

        //------------Properties--------------------//

        /// <summary>
        /// CDPI or "Cluster Distribution Performance Indicator" is the total distribution performance for the entire cluster.
        /// This value is not relative, it is based on the individual DPI of each node, the total processing power of the cluster, the number of individual
        /// processes runnable, and the performance of the clusters communications channel. It is a "score" for the cluster as 
        /// a whole.
        /// </summary>
        public static long CDPI {
            get {
                return CDPI;
            }
            internal set {
                CDPI = value;
            }
        }

        /// <summary>
        /// A proxy out of the current domain and into the Presto server. 
        /// </summary>
        internal static IClusterProxy ClusterProxy;

        //we keep a list of all jobs currently out for processing
        private static ConcurrentDictionary<string, Action<PrestoResult>> outboundJobs = new ConcurrentDictionary<string, Action<PrestoResult>>();
        private static ConcurrentDictionary<string, ManualResetEvent> waits = new ConcurrentDictionary<string, ManualResetEvent>();
        private static ManualResetEvent jobCompletionEvent = new ManualResetEvent(true);

        /// <summary>
        /// Every presto module or application is assigned a unique identifier to separate itself from other applications or
        /// modules also loaded into the cluster. This identifier is given to the Cluster object upon at its creation and will 
        /// not change for the lifetime of this module or application, nor will it change between cluster nodes.
        /// 
        /// Consequently, this is the same ID given to the app domain under which this module or application will run.
        /// 
        /// </summary>
        public static string Key {
            get;
            internal set;
        }

        //------------Methods-----------------------//

        /// <summary>
        /// Deploys an execution job into the cluster. 
        /// 
        /// The method passed in as "function" MUST be static. If it is not, an error will be thrown and added to the error log.
        /// Instance data is not preserved outside of the running ApplicationDomain and indeed all data not instantiated within the 
        /// method or not globally synchronized using a cluster data structure is considered volatile, mutable, inconsitent and untrustworthy.
        /// 
        /// DO NOT write code that will depend on instance or static class variables in order to do processing
        /// unless those variables are declared constant from the start of the module. Write functions as if they are black boxes,
        /// the only thing you see is input and output.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed into the executed function.</param>
        /// <param name="callback">The callback to be executed when the function completes.</param>
        /// <returns>The execution ID of this particular execution.</returns>
        public static string Execute(Func<PrestoParameter, PrestoResult> function, PrestoParameter parameter, Action<PrestoResult> callback) {
            //set the event to non signaled
            jobCompletionEvent.Reset();

            if (!function.Method.IsStatic) {
                //I should really make some presto specific exceptions... i will add that to the todo.
                throw new Exception("Function is not static");
            }
            //Get a DateTime to mark the beginning of an execution
            DateTime now = DateTime.Now;
            string contextID = Generator.RandomAlphaNumeric(Config.UIDLength);
            //Create a reset event and add it to the dictionary for this job
            ManualResetEvent mre = new ManualResetEvent(false);
            waits[contextID] = mre;
            //add the job to the scheduled jobs
            outboundJobs[contextID] = callback;
            //execute
            SerializationEngine serializer = new SerializationEngine ();
            byte[] stream = serializer.Serialize(parameter);
            ClusterProxy.Execute(function.Method.DeclaringType.Assembly.FullName, function.Method.DeclaringType.FullName, function.Method.Name, stream, contextID, Key);
            return contextID;
        }

        /// <summary>
        /// Returns an execution job to the cluster object to be dispatched to the calling module.
        /// </summary>
        /// <param id="result">The execution result object.</param>
        internal static void ReturnExecution(string contextID, PrestoResult result) {
            //get the callback
            Action<PrestoResult> callback;
            outboundJobs.TryRemove(contextID, out callback);
            //invoke the callback
            callback(result);
            returnCallbackCleanup(contextID);
        }

        private static void returnCallbackCleanup(string contextID) {
            //get the respective reset event
            ManualResetEvent mre;
            waits.TryRemove(contextID, out mre);
            mre.Set();
            //and the generic reset event
            if (outboundJobs.Count < 1) {
                jobCompletionEvent.Set();
            }
        }

        /// <summary>
        /// Blocks the currently running thread until all execution jobs return from the cluster. The thread will resume once all jobs return succesful. And their
        /// callbacks have been run.
        /// </summary>
        public static void Wait() {
            jobCompletionEvent.WaitOne();
        }

        /// <summary>
        /// Blocks the currently running thread until the execution with the specefied execution id returns from the cluster. Will release
        /// the thread once the executions callback has been processed.
        /// </summary>
        /// <param name="executionID">The id of the execution to wait on.</param>
        public static void Wait(string executionID) {
            if(waits.ContainsKey(executionID)){
                waits[executionID].WaitOne();
            }
        }

        /// <summary>
        /// Send a message to the node with the specified ID. The message is delivered to 
        /// the receiving node and calls the MessageReceived event.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent.</param>
        public static void SendMessage(ClusterNode node, string message) {
            ClusterProxy.SendMessage(node.NodeID, message, Key);
        }

        /// <summary>
        /// Deliver the message from a remote node.
        /// </summary>
        /// <param name="payload">The message sent.</param>
        /// <param name="sender"> The Node ID of the sending node.</param>
        internal static void DeliverMessage(string payload, ClusterNode sender) {
            MessageRecieved(payload, sender);
        }

        /// <summary>
        /// Trigger the unload event as a module or application has issued the Completion Signal.
        /// </summary>
        internal static void TriggerUnloading() {
            if (Unloading != null) {
                Unloading();
            }
        }

        /// <summary>
        /// Get the IDs of all nodes available to this application or module.
        /// </summary>
        /// <param name="includeSelf">Whether or not to include the local node id in the listing.</param>
        /// <returns>List of all node IDs available to this application or module.</returns>
        public static ClusterNode[] GetAvailableNodes(bool includeSelf = true) {
            return ClusterProxy.GetAvailableNodes(Key);
        }
    }
}
