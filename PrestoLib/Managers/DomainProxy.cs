using System;
using System.Collections.Generic;
using System.Reflection;
using Presto.Common;

namespace Presto.Managers {

    /// <summary>
    /// Allows for proxy access into a module or applications domain from the controlling Presto instance.
    /// </summary>
    public class DomainProxy : MarshalByRefObject {

        /// <summary>
        /// A listing of all the names of assemblies currently loaded into this domain.
        /// </summary>
        private List<string> assemblyNames = new List<string>();

        /// <summary>
        /// A listing of all assemblies in this domain.
        /// </summary>
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        /// <summary>
        /// The module instance inside this domain.
        /// </summary>
        private PrestoModule moduleInstance = null;

        /// <summary>
        /// Direct the surrounding app domain to load a new assembly.
        /// </summary>
        /// <param name="assemblyImage">The COFF based image of the assembly to be loaded.</param>
        public string LoadAssembly(byte[] assemblyImage) {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            Assembly newAssembly = currentDomain.Load(assemblyImage);
            assemblyNames.Add(newAssembly.FullName);
            assemblies.Add(newAssembly.FullName, newAssembly);
            createPrestoInstance(newAssembly.FullName);
            return newAssembly.FullName;
        }

        /// <summary>
        /// Creates the presto instance housed in assembly with the given name.
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly that the Presto object resides in.</param>
        private void createPrestoInstance(string assemblyName) {
            //do not create an instance if one already exists
            if (moduleInstance != null)
            {
                return;
            }

            Assembly assembly = assemblies[assemblyName];
            //get all types housed in the assembly
            Type[] assemblyTypes = assembly.GetTypes();
            //create an instance of the PrestoModule
            PrestoModule module = null;
            foreach (Type type in assemblyTypes) {
                if (type.IsSubclassOf(typeof(PrestoModule))) {
                    module = (PrestoModule)Activator.CreateInstance(type);
                    module.Init();
                    break;
                }
            }
            moduleInstance = module;
        }

        /// <summary>
        /// Whether or not this domain contains a specified assembly
        /// </summary>
        /// <param name="assemblyName">The full name of the assembly.</param>
        /// <returns>boolean - whether or not the domain has the specefied assembly.</returns>
        public bool HasAssembly(string assemblyName) {
            if (assemblyNames.Contains(assemblyName)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Execute the load procedure for this domains residing instance.
        /// </summary>
        public void ExecuteInstance() {
            Action moduleLoad = new Action(moduleInstance.Startup);
            moduleLoad.BeginInvoke(null, null);
        }


        /// <summary>
        /// Execute an incoming job.
        /// </summary>
        /// <param name="methodName">The name of the procedure to be executed.</param>
        /// <param name="typeName">The name of the type held within the assembly for with the procedure to be executed resides.</param>
        /// <param name="assemblyName">The name of the assembly the procedure resides in.</param>
        /// <param name="parameter">The parameter passed to the executed procedure.</param>
        /// <returns>The result of the execution serialized for transport.</returns>
        public byte[] ExecuteIncoming(string methodName, string typeName, string assemblyName, byte[] parameter) {
            SerializationEngine serializer = new SerializationEngine ();
            PrestoParameter param = (PrestoParameter)serializer.Deserialize(parameter);
            Assembly assembly = assemblies[assemblyName];
            Type type = assembly.GetType(typeName, false, true);
            MethodInfo method = type.GetMethod(methodName);
            PrestoResult res = (PrestoResult)method.Invoke(null, new object[] { param });
            return serializer.Serialize(res);
        }

        /// <summary>
        /// Set up the cluster proxy instance for the domain and tell the domain what its Key is.
        /// </summary>
        /// <param name="clusterProxy">The cluster proxy instance to be set up.</param>
        /// <param name="domainKey">The domain Key of the domain.</param>
        public void ConfigureCluster(IClusterProxy clusterProxy, string domainKey) {
            Cluster.Key = domainKey;
            Cluster.ClusterProxy = clusterProxy;
        }


        /// <summary>
        /// Return an execution back into the domain.
        /// </summary>
        /// <param name="contextID">The context ID of the execution.</param>
        /// <param name="nodeID">The node ID where the execution was run.</param>
        /// <param name="result">The serialized result of the excution.</param>
        public void ReturnExecution(string contextID, ClusterNode node, byte[] result) {
            SerializationEngine serializer = new SerializationEngine ();
            PrestoResult resultObj = (PrestoResult)serializer.Deserialize(result);
            resultObj.ExecutionNode = node;
            Cluster.ReturnExecution(contextID, resultObj);
        }

        /// <summary>
        /// Get the names of all user assemblies loaded into this domain.
        /// </summary>
        /// <returns>The names of user assemblies loaded into this domain.</returns>
        public string[] GetAssemblyNames() {
            return assemblyNames.ToArray();
        }

        /// <summary>
        /// Deliver the message sent from an outside node.
        /// </summary>
        /// <param name="payload">The message sent from the outside node.</param>
        /// <param name="sender"> The node object of the sending Node.</param>
        public void DeliverMessage(string payload, ClusterNode sender) {
            Cluster.DeliverMessage(payload, sender);
        }
    }
}
