using System;
using System.Threading;

namespace Presto.DataStructures.Distributed
{
    /// <summary>
    /// Result object for a data request from the cluster dictionary.
    /// </summary>
    public class ClusterDictionaryResult<T> where T : struct
    {

        public object AsyncState
        {
            get { return AsyncState; }
            internal set { AsyncState = value;}
        }


        public bool LocallyObtained
        {
            get { return LocallyObtained; }
            internal set { LocallyObtained = value; }
        }

        public bool IsCompleted
        {
            get { return IsCompleted; }
            internal set { IsCompleted = value; }
        }

        public T Value
        {
            get {
                return Value;
            }
            internal set { Value = value; }
        }

        public AsyncResultErrorState ResultErrorState
        {
            get { return ResultErrorState; }
            internal set { ResultErrorState = value; }
        }
  
        public ClusterDictionaryResult (){

        }
    }
}
