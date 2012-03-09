using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Presto.Common;
using Presto.Common.Machine;
using Presto.Common.Net;

namespace Presto {
    /// <summary>
    /// The Cluster class is a static class that extends functionality that allows for interaction with the Presto cluster to the Module developer
    /// </summary>
    public class Cluster : ClusterBase {

        //we keep a list of all jobs currently out for processing
        private Dictionary<string, OutboundJob> outboundJobs = new Dictionary<string, OutboundJob>();
        private ManualResetEvent jobCompletionEvent = new ManualResetEvent(true);
        /// <summary>
        /// Initialize the servers cluster instance.
        /// </summary>
        public void Initialize() {
            Application.ControlServer.RegisterDispatchAction(MessageType.VERIFY, verifyResponse);
        }

        /// <summary>
        /// This server needs to verify itself and send back a Verfication object.
        /// </summary>
        /// <param name="state">The server state object.</param>
        private void verifyResponse(ServerState state) {
            Verification verification = new Verification("name", DPI.GetDPI());
            state.Write(MessageType.VERIFICATION_RESPONSE, SerializationEngine.Serialize(verification).ToArray());
        }

        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed to the function.</param>
        /// <param name="callback">The callback that is to be fired upon completion of the job.</param>
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
            ExecutionContext context = new ExecutionContext(method, parameter, Generator.RandomAlphaNumeric(Config.UIDLength));
            //add the job to the scheduled jobs
            OutboundJob job = new OutboundJob(context.ContextID, now, callback);
            outboundJobs.Add(context.ContextID, job);
            //Pass the execution context to the node best fit to serve it
            Nodes.BestNode().Execute(context);
        }

        /// <summary>
        /// Returns an execution job to the cluster object to be dispatched to the calling module.
        /// </summary>
        /// <param name="result">The execution result object.</param>
        public void ReturnExecution(ExecutionResult result) {
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
