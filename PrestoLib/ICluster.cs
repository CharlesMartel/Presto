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
        /// 
        /// The method passed in as "function" MUST be static. If it is not, an error will be thrown and added to the error log.
        /// Instance data is not preserved outside of the running ApplicationDomain and indeed all data not instantiated within the 
        /// method or not globally synchronized by the SynchronizationFactory is considered volatile, mutable, inconsitent and untrustworthy.
        /// 
        /// DO NOT write code that will depend on instance or static class variables in order to do processing
        /// unless those variables are declared constant.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        void Execute(Func<IPrestoParameter, IPrestoResult> function, IPrestoParameter parameter, Action<IPrestoResult> callback);

    }
}
