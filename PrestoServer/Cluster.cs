using System;
using Presto.Common.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace Presto
{
    /// <summary>
    /// The Cluster class is a static class that extends functionality that allows for interaction with the Presto cluster to the Module developer
    /// </summary>
    public class Cluster : ClusterBase
    {
        

        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed to the function.</param>
        public override void Execute(Func<PrestoParameter, PrestoResult> function, PrestoParameter parameter, Action<PrestoResult> callback){
            //Create the ExecutionContext for the method.
            MethodInfo method = function.Method;
            ExecutionContext context = new ExecutionContext(method, parameter);
            //Pass the execution context to the node best fit to serve it
            Nodes.BestNode().Execute(context);
        }
    }
}
