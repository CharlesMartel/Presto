using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Reflection.Emit;
using Presto.Common;
using Presto.Common.Net;

namespace Presto {

    /// <summary>
    /// Processes all Module startups and Cluster jobs. These jobs can be recieved either internally as when a job is to be
    /// executed using a previously loaded assembly, or externally as when the loader starts up a new assembly recieved from 
    /// the Presto client application.
    /// </summary>
    public static class Executor {

        /// <summary>
        /// Initializes the Executor.
        /// </summary>
        public static void Initialize() 
        {
            Application.ControlServer.RegisterDispatchAction(MessageType.EXECUTION_BEGIN, ExecutionBegin);
        }

        /// <summary>
        /// Starts up the execution of a new module, making this instance the master.
        /// </summary>
        /// <param name="assemblyWrapper">An AssemblyWrapper around the assembly of the module to be executed.</param>
        public static void ExecuteModule(AssemblyWrapper assemblyWrapper) {
            //finally execute the user module
            PrestoModule module = assemblyWrapper.GetModuleInstance();
            module.Load();
        }

        /// <summary>
        /// Executes a particular function in a module, according to the passed in parameters, acting as a slave server
        /// </summary>
        /// <param name="state">The server state object of the request</param>
        public static void ExecutionBegin(ServerState state) 
        {
            //finally execute the function defined in the transfer
            //get the execution context
            SoapFormatter soap = new SoapFormatter();            
            ExecutionContext context = (ExecutionContext)soap.Deserialize(state.GetDataMemoryStream());
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly[] assemblies = currentDomain.GetAssemblies();
            Type type = null;
            MethodInfo method = null;
            foreach(Assembly a in assemblies){
                if(a.FullName == context.AssemblyName){
                    type = a.GetType(context.TypeName, false, true);
                    method = type.GetMethod(context.MethodName);
                    break;
                }
            }
            
            PrestoResult res = (PrestoResult)method.Invoke(null, new object[] { context.Parameter });
            ExecutionResult result = new ExecutionResult(res);
            MemoryStream stream = new MemoryStream();
            soap.Serialize(stream, result);
            state.WriteAndClose(MessageType.EXECUTION_COMPLETE, stream.ToArray());
        }

        private static void asyncExecute(ServerState state, ExecutionContext context) {

        }
    }
}
