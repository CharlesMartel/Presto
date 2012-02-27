using System;
using Presto.Common.Net;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.IO;

namespace Presto
{
    /// <summary>
    /// The Cluster class is a static class that extends functionality that allows for interaction with the Presto cluster to the Module developer
    /// </summary>
    [Serializable()]
    public class Cluster : ICluster
    {

        /// <summary>
        /// Deploys an execution job into the cluster.
        /// </summary>
        /// <param name="function">The function to be executed.</param>
        /// <param name="parameter">The parameter to be passed to the function.</param>
        void ICluster.Execute(Func<IPrestoParameter, IPrestoResult> function, IPrestoParameter parameter){
            //a test
            string  host1 = Config.GetHosts()[0];            
            TCPClient cli = new TCPClient(host1, 2500);
            MethodInfo method = function.Method;
            ExecutionContext context = new ExecutionContext(method, parameter);
            cli.Connect();
            SoapFormatter soap = new SoapFormatter();
            MemoryStream stream = new MemoryStream();
            soap.Serialize(stream, context);
            cli.setDispatchAction(MessageType.EXECUTION_COMPLETE, recieve);
            cli.Write(MessageType.EXECUTION_BEGIN, stream.ToArray());
        }

        private void recieve(ClientState state) {
            SoapFormatter soap = new SoapFormatter();
            ExecutionResult er = (ExecutionResult)soap.Deserialize(new MemoryStream(state.GetDataArray()));
            IPrestoResult result = er.Result;
            Console.WriteLine("Returned");
        }
    }
}
