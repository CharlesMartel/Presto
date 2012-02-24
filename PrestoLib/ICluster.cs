using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto
{
    /// <summary>
    /// A defining interface for the PrestoServer internal cluster object. 
    /// </summary>
    public interface ICluster
    {
        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        void Execute(Func<IPrestoParameter, IPrestoResult> function);
    }
}
