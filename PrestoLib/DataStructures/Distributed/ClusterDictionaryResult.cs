using System;
using System.Threading;

namespace Presto.DataStructures.Distributed
{
    /// <summary>
    /// Result object for a data request from the cluster dictionary.
    /// </summary>
    public class ClusterDictionaryResult<T> where T : struct
    {
        private ManualResetEvent completionResetEvent = new ManualResetEvent(false);

        public object AsyncState {
            get;
            internal set;
        }


        public bool LocallyObtained
        {
            get;
            internal set;
        }

        private bool completed = false;
        public bool IsCompleted
        {
            get { return completed;}
            internal set{
                completed = value;
                completionResetEvent.Set();
            }
        }

        public T Value {
            get;
            internal set;
        }

        public AsyncResultErrorState ResultErrorState
        {
            get;
            internal set;
        }
  
        public ClusterDictionaryResult (){

        }

        public void WaitComplete() {
            completionResetEvent.WaitOne();
        }

        public void WaitComplete(int waitMilliseconds) {
            completionResetEvent.WaitOne(waitMilliseconds);
        }
    }
}
