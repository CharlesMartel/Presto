using System;
using System.Collections.Generic;
using Presto.Common;
using Presto.Remote;
using Presto.Transfers;
using Presto.Managers;

namespace Presto.Managers {

    /// <summary>
    /// Stores and manages all created domains.
    /// </summary>
    public static class DomainManager {

        /// <summary>
        /// Internal hashmap of domains according to their generated id's.
        /// </summary>
        private static Dictionary<string, AppDomain> domains = new Dictionary<string, AppDomain>();
        /// <summary>
        /// Internal hashmap of domains and their proxy instances.
        /// </summary>
        private static Dictionary<string, DomainProxy> proxies = new Dictionary<string, DomainProxy>();
        /// <summary>
        /// COFF images of assemblies according to their full names.
        /// </summary>
        private static Dictionary<string, byte[]> assemblies = new Dictionary<string, byte[]>();

        /// <summary>
        /// Creates a new application domain, and adds it to the store. If the domain already exists, does nothing.
        /// </summary>
        public static void CreateDomain(string domainKey) {
            //if we already have the domain it doesnt need to be created.
            if (domains.ContainsKey(domainKey)) {
                return;
            }
            AppDomain newDomain = AppDomain.CreateDomain(domainKey);
            //preload assemblies
            foreach (string item in Config.DomainPreloads) {
                newDomain.Load(item);
            }
            //create the domain proxy
            DomainProxy proxy = (DomainProxy)newDomain.CreateInstanceAndUnwrap(typeof(DomainProxy).Assembly.FullName, typeof(DomainProxy).FullName);
            //create the cluster proxy
            ClusterProxy cproxy = new ClusterProxy();
            proxy.ConfigureCluster(cproxy, domainKey);
            //add the data to the lookup tables
            proxies.Add(domainKey, proxy);
            domains.Add(domainKey, newDomain);
        }

        /// <summary>
        /// Startup an assembly into the specefied domain.
        /// </summary>
        /// <param name="domainKey">The string Key of the domain to be loaded into.</param>
        /// <param name="assemblyStream">The COFF byte array of the assembly.</param>
        public static void LoadAssemblyIntoDomain(string domainKey, byte[] assemblyStream) {
            DomainProxy proxy = proxies[domainKey];
            string assemblyName = proxy.LoadAssembly(assemblyStream);
            if (assemblies.ContainsKey(assemblyName)) {
                assemblies.Remove(assemblyName);
            }
            assemblies.Add(assemblyName, assemblyStream);
        }

        /// <summary>
        /// Unloads and destroys the domain with the specified Key. Also deletes any assemblies associated with the domain.
        /// </summary>
        /// <param name="domainKey">The Key of the domain to be destroyed.</param>
        /// <param name="instanceSignal">Specefies whether or not the Destroy domain signal came from a local instance.</param>
        public static void DestroyDomain(string domainKey, bool instanceSignal = false) {
            if (!domains.ContainsKey(domainKey)) {
                return;
            }
            AppDomain domain = domains[domainKey];
            DomainProxy proxy = proxies[domainKey];
            if (instanceSignal) {
                Nodes.UnloadDomain(domainKey, proxy.GetAssemblyNames());
                return;
            }
            foreach (string assem in proxy.GetAssemblyNames()) {
                if (assemblies.ContainsKey(assem)) {
                    assemblies.Remove(assem);
                }
            }
            proxies.Remove(domainKey);
            domains.Remove(domainKey);
            Action<AppDomain> final = finalUnload;
            final.BeginInvoke(domain, null, null);
        }

        /// <summary>
        /// The final destruction of a domain followed by a garbage collection.
        /// </summary>
        /// <param name="domain">The domain to be destroyed.</param>
        private static void finalUnload(AppDomain domain){
            AppDomain.Unload(domain);
            GC.Collect ();
        }

        /// <summary>
        /// Whether or not a domain with a given Key exists.
        /// </summary>
        /// <param name="domainKey">The Key of the domain to check for.</param>
        /// <returns></returns>
        public static bool HasDomain(string domainKey) {
            if (domains.ContainsKey(domainKey)) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tells whether the assembly with the given name is loaded into the domain with the given Key.
        /// Will return false if the given domain Key does not correspond to a currently loaded domain.
        /// </summary>
        /// <param name="domainKey">The Key of the domain to check in.</param>
        /// <param name="assemblyName">The name of the assembly to check for.</param>
        /// <returns>Whether or not the domain has the assembly.</returns>
        public static bool DomainHasAssembly(string domainKey, string assemblyName) {
            if (!HasDomain(domainKey)) {
                return false;
            }
            DomainProxy proxy = proxies[domainKey];
            return proxy.HasAssembly(assemblyName);
        }

        /// <summary>
        /// Get the COFF-based image of an assembly in a byte array.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>The COFF-based image of an assembly in a byte array.</returns>
        public static byte[] GetAssemblyStream(string assemblyName) {
            return assemblies[assemblyName];
        }

        /// <summary>
        /// Execute the startup procedure on the presto instance in the specefied domain.
        /// </summary>
        /// <param name="domainKey">The Key of the domain to execute in.</param>
        public static void ExecuteStartup(string domainKey) {
            DomainProxy proxy = proxies[domainKey];
            proxy.ExecuteInstance();
        }

        /// <summary>
        /// Execute an incoming job according to the passed in execution context.
        /// </summary>
        /// <param name="context">The context of the job.</param>
        /// <returns>The result of the job serialized for transport.</returns>
        public static byte[] ExecuteIncoming(ExecutionContext context) {
            DomainProxy proxy = proxies[context.DomainKey];
            return proxy.ExecuteIncoming(context.MethodName, context.TypeName, context.AssemblyName, context.Parameter);
        }

        /// <summary>
        /// Return an execution from the cluster and give it back to the correct domain.
        /// </summary>
        /// <param name="result">The Result object of the execution.</param>
        public static void ReturnExecution(ExecutionResult result) {
            DomainProxy proxy = proxies[result.DomainKey];
            Node executor = Nodes.GetNodeByID(result.ExecutingNodeID);
            proxy.ReturnExecution(result.ContextID, executor, result.Result);
        }

        /// <summary>
        /// Delivers a user sent message to the correct domain. If the correct domain is not valid, an error is logged.
        /// </summary>
        /// <param name="message">The UserMessage object for the message.</param>
        public static void DeliverMessage(UserMessage message) {
            if (domains.ContainsKey(message.DomainKey)) {
                DomainProxy proxy = proxies[message.DomainKey];
                Node node = Nodes.GetNodeByID(message.Sender);
                proxy.DeliverMessage(message.Message, node);
            } else {
                Log.Error("No domain with Key: " + message.DomainKey + " was found to deliver message: \"" + message.Message + "\" from: NodeID: " + message.Sender);
            }
        }

        /// <summary>
        /// Retrieve the COFF based images of all assemblies loaded into a domain along with the assemblies full names as keys.
        /// </summary>
        /// <param name="domainKey">The domain key associated with the requested domain.</param>
        /// <returns>The COFF based images of all assemblies loaded into a domain along with their full names as keys.</returns>
        public static Dictionary<string, byte[]> GetDomainAssemblies(string domainKey)
        {
            Dictionary<string, byte[]> assemblyListing = new Dictionary<string, byte[]>();
            if (!HasDomain(domainKey))
            {
                return assemblyListing;
            }
            DomainProxy proxy = proxies[domainKey];
            string[] assemblyNames = proxy.GetAssemblyNames();
            foreach (string name in assemblyNames)
            {
                if (assemblies.ContainsKey(name)){
                    assemblyListing.Add(name, assemblies[name]);
                }
            }
            return assemblyListing;
        }

        /// <summary>
        /// Get the name of all assemblies currently loaded into this instance.
        /// </summary>
        /// <returns>The name of all assemblies currently loaded into this instance.</returns>
        public static string[] GetAllAssemblyNames()
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, byte[]> assembly in assemblies)
            {
                keys.Add(assembly.Key);
            }
            return keys.ToArray();
        }

        /// <summary>
        /// Get the keys of all domains loaded into this node.
        /// </summary>
        /// <returns>The keys of all domains loaded into this node.</returns>
        public static string[] GetAllDomainKeys()
        {
            List<string> keys = new List<string>();
            foreach (KeyValuePair<string, AppDomain> domain in domains)
            {
                keys.Add(domain.Key);
            }
            return keys.ToArray();
        }
    }
}
