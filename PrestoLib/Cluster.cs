using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto
{
    /// <summary>
    /// The Cluster class is a static class that extends functionality that allows for interaction with the Presto cluster to the Module developer
    /// </summary>
    public static class Cluster
    {

        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        public static void Execute(Func<IPrestoParameter, IPrestoResult> function){
            
        }
    }
}
