using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Runtime.CompilerServices;
using Presto.Common;
using Presto.Common.Net;

namespace Presto {
    /// <summary>
    /// The Cluster class is a static class that extends functionality that allows for interaction with the Presto cluster to the Module developer
    /// </summary>    
    public class Cluster : ClusterBase {

        //we keep a list of all jobs currently out for processing
        private Dictionary<string, Transfers.OutboundJob> outboundJobs = new Dictionary<string, Transfers.OutboundJob>();
        private ManualResetEvent jobCompletionEvent = new ManualResetEvent(true);
        private string key;

        /// <summary>
        /// Create a new cluster object for a particular module or application.
        /// </summary>
        /// <param name="domainKey">The domain key associated with this cluster.</param>
        public Cluster(string domainKey) {
            key = domainKey;
        }

        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param id="function">The function to be executed.</param>
        /// <param id="parameter">The parameter to be passed to the function.</param>
        /// <param id="callback">The callback that is to be fired upon completion of the job.</param>
        public override void Execute(Func<PrestoParameter, PrestoResult> function, PrestoParameter parameter, Action<PrestoResult> callback) {
            //set the event to non signaled
            jobCompletionEvent.Reset();

            if (!function.Method.IsStatic) {
                //I should really make some presto specific exceptions... i will add that to the todo.
                throw new Exception("Function is not static");
            }
            //Get a DateTime to mark the beginning of an execution
            DateTime now = DateTime.Now;
            //Create the ExecutionContext for the method.
            MethodInfo method = function.Method;
            Transfers.ExecutionContext context = new Transfers.ExecutionContext(method, parameter, Generator.RandomAlphaNumeric(Config.UIDLength), key);
            //add the job to the scheduled jobs
            Transfers.OutboundJob job = new Transfers.OutboundJob(context.ContextID, now, callback, key);
            outboundJobs.Add(context.ContextID, job);
            //Pass the execution context to the node best fit to serve it
            Nodes.BestNode().Execute(context);
        }

        /// <summary>
        /// Returns an execution job to the cluster object to be dispatched to the calling module.
        /// </summary>
        /// <param id="result">The execution result object.</param>
        public void ReturnExecution(Transfers.ExecutionResult result) {
            outboundJobs[result.ContextID].Callback.Invoke(result.Result);
            outboundJobs.Remove(result.ContextID);
            if (outboundJobs.Count < 1) {
                jobCompletionEvent.Set();
            }
        }

        /// <summary>
        /// Wait on all currently processing jobs to return. Blocks the currently running thread untill all jobs return succesful.
        /// </summary>
        public override void Wait() {
            jobCompletionEvent.WaitOne();
        }
    }
}
