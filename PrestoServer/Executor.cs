using System.Threading;
using Presto.Common;
using Presto.Managers;
using Presto.Net;

namespace Presto {

    /// <summary>
    /// Processes all Module startups and Cluster jobs. These jobs can be recieved either internally as when a job is to be
    /// executed using a previously loaded assembly, or externally as when the loader starts up a new assembly recieved from 
    /// the Presto client application.
    /// </summary>
    public static class Executor {

        /// <summary>
        /// The number of currently executing or thread pool queued jobs in this instance.
        /// </summary>
        private static int runningJobs = 0;

        private delegate void AsyncMethodCaller(ServerState state);

        /// <summary>
        /// Initializes the Executor.
        /// </summary>
        public static void Initialize() {
            Application.ControlServer.RegisterDispatchAction(MessageType.EXECUTION_BEGIN, ExecutionBegin);
        }

        /// <summary>
        /// Get the number of currently executing jobs on this instance.
        /// </summary>
        /// <returns>The number of currently executing jobs.</returns>
        public static int RunningJobs() {
            return runningJobs;
        }

        /// <summary>
        /// Starts up the execution of a new module, making this instance the master.
        /// </summary>
        /// <param id="domainKey">The domain Key of the domain that this module will be executed in.</param>
        public static void ExecuteModule(string domainKey) {
            //finally execute the user module
            DomainManager.ExecuteStartup(domainKey);
        }

        /// <summary>
        /// Executes a particular function in a module, according to the passed in parameters, acting as a slave server
        /// </summary>
        /// <param id="state">The server state object of the request</param>
        public static void ExecutionBegin(ServerState state) {
            //finally execute the function defined in the transfer
            AsyncMethodCaller execution = new AsyncMethodCaller(execute);
            execution.BeginInvoke(state, null, null);
        }

        /// <summary>
        /// Internal execute function to be called asynchronously.
        /// </summary>
        /// <param id="state">The server state object associated with the execution.</param>
        private static void execute(ServerState state) {
            Interlocked.Increment(ref runningJobs);
            //get the execution context 
            SerializationEngine serializer = new SerializationEngine ();
            Transfers.ExecutionContext context = (Transfers.ExecutionContext)serializer.Deserialize(state.GetDataArray());
            byte[] res = DomainManager.ExecuteIncoming(context);
            Transfers.ExecutionResult result = new Transfers.ExecutionResult(res, context.ContextID, context.DomainKey, ClusterManager.NodeID);
            state.Write(MessageType.EXECUTION_COMPLETE, serializer.Serialize(result));
            Interlocked.Decrement(ref runningJobs);
        }
    }
}
