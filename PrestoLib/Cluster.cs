﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Runtime.CompilerServices;
using Presto.Common;
using System.IO;

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
        public delegate void MessageReceivedHandler(string payload, string sender);
        /// <summary>
        /// The events called whenever a message is received from an outside node.
        /// </summary>
        public static event MessageReceivedHandler MessageRecieved;

        //------------Properties--------------------//

        /// <summary>
        /// CDPI or "Cluster Distribution Performance Indicator" is the total distribution performance for the entire cluster.
        /// This value is not relative, it is based on the individual DPI of each node, the total processing power of the cluster, the number of individual
        /// processes runnable, and the performance of the clusters communications channel. It is a "score" for the cluster as 
        /// a whole.
        /// </summary>
        public static long CDPI;

        /// <summary>
        /// A proxy out of the current domain and into the Presto server. This should not be overwritten or even accessed unless you
        /// are fully aware of the consequences.
        /// </summary>
        public static IClusterProxy ClusterProxy;

        //we keep a list of all jobs currently out for processing
        private static Dictionary<string, Action<PrestoResult>> outboundJobs = new Dictionary<string, Action<PrestoResult>>();
        private static ManualResetEvent jobCompletionEvent = new ManualResetEvent(true);
        internal static string key;

        //------------Methods-----------------------//

        /// <summary>
        /// Deploys an execution job into the cluster. 
        /// 
        /// The method passed in as "function" MUST be static. If it is not, an error will be thrown and added to the error log.
        /// Instance data is not preserved outside of the running ApplicationDomain and indeed all data not instantiated within the 
        /// method or not globally synchronized by the SynchronizationFactory is considered volatile, mutable, inconsitent and untrustworthy.
        /// 
        /// DO NOT write code that will depend on instance or static class variables in order to do processing
        /// unless those variables are declared constant.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed into the executed function.</param>
        /// <param name="callback">The callback to be executed when the function completes.</param>
        public static void Execute(Func<PrestoParameter, PrestoResult> function, PrestoParameter parameter, Action<PrestoResult> callback) {
            //set the event to non signaled
            jobCompletionEvent.Reset();

            if (!function.Method.IsStatic) {
                //I should really make some presto specific exceptions... i will add that to the todo.
                throw new Exception("Function is not static");
            }
            //Get a DateTime to mark the beginning of an execution
            DateTime now = DateTime.Now;
            string contextID = Generator.RandomAlphaNumeric(Config.UIDLength);
            //add the job to the scheduled jobs
            outboundJobs.Add(contextID, callback);
            //execute
            byte[] stream = SerializationEngine.Serialize(parameter);
            ClusterProxy.Execute(function.Method.DeclaringType.Assembly.FullName, function.Method.DeclaringType.FullName, function.Method.Name, stream, contextID, key);
        }

        /// <summary>
        /// Returns an execution job to the cluster object to be dispatched to the calling module.
        /// </summary>
        /// <param id="result">The execution result object.</param>
        public static void ReturnExecution(string contextID, PrestoResult result) {
            outboundJobs[contextID].Invoke(result);
            outboundJobs.Remove(contextID);
            if (outboundJobs.Count < 1) {
                jobCompletionEvent.Set();
            }
        }

        /// <summary>
        /// Blocks the currently running thread until all execution jobs return from the cluster. The thread will resume once all jobs return succesful.
        /// </summary>
        public static void Wait() {
            jobCompletionEvent.WaitOne();
        }

        /// <summary>
        /// Every presto module or application is assigned a unique identifier to separate itself from other applications or
        /// modules also loaded into the cluster. This identifier is given to the Cluster object upon at its creation and will 
        /// not change for the lifetime of this module or application, nor will it change between cluster nodes.
        /// 
        /// Consequently, this is the same ID given to the app domain under which this module or application will run.
        /// 
        /// </summary>
        public static string GetInstanceKey(){
            return key;
        }

        /// <summary>
        /// Send a message to the node with the specified ID. The message is UTF8 encoded on transport and is delivered to 
        /// the receiving node calling MessageReceived event.
        /// </summary>
        /// <param name="nodeID">The node ID of the node to send the message to.</param>
        /// <param name="message">The message to be sent. This message is UTF8 encoded on transport.</param>
        public static void SendMessage(string nodeID, string message) {
            ClusterProxy.SendMessage(nodeID, message, key);
        }

        /// <summary>
        /// Deliver the message from a remote node.
        /// </summary>
        /// <param name="payload">The message sent.</param>
        /// <param name="sender"> The Node ID of the sending node.</param>
        internal static void DeliverMessage(string payload, string sender){
            MessageRecieved(payload, sender);
        }
    }
}
