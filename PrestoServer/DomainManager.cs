using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Presto.Common;

namespace Presto {

    /// <summary>
    /// Stores and manages all created domains.
    /// </summary>
    public static class DomainManager {

        /// <summary>
        /// Internal hashmap of domains according to their generated id's.
        /// </summary>
        private static Dictionary<string, AppDomain> domains = new Dictionary<string, AppDomain>();
        /// <summary>
        /// Internal hashmap of which domains have which assemblies.
        /// </summary>
        private static Dictionary<string, List<Assembly>> domainAssemblies = new Dictionary<string, List<Assembly>>();
        /// <summary>
        /// Internal hashmap of instances of the presto instances.
        /// </summary>
        private static Dictionary<string, PrestoModule> domainInstance = new Dictionary<string, PrestoModule>();
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
            newDomain.Load("LibPresto");
            domains.Add(domainKey, newDomain);
            domainAssemblies.Add(domainKey, new List<Assembly>());
            domainInstance.Add(domainKey, null);
        }

        /// <summary>
        /// Load an assembly into the specefied domain.
        /// </summary>
        /// <param name="domainKey">The string key of the domain to be loaded into.</param>
        /// <param name="assemblyStream">The COFF byte array of the assembly.</param>
        /// <param name="createInstance">By default we also create a new module instance upon loading the assembly.</param>
        public static Assembly LoadAssemblyIntoDomain(string domainKey, byte[] assemblyStream, bool createInstance = true){
            AppDomain domain = domains[domainKey];
            Assembly newAssembly = domain.Load(assemblyStream);
            domainAssemblies[domainKey].Add(newAssembly);
            assemblies.Add(newAssembly.FullName, assemblyStream);
            if (createInstance) {
                PrestoModule module = createPrestoInstance(newAssembly, domainKey);
                domainInstance.Add(domainKey, module);
            }
            return newAssembly;
        }

        /// <summary>
        /// Initializes the presto module instance 
        /// </summary>
        private static PrestoModule createPrestoInstance(Assembly assembly, string domainKey) {
            //get all types housed in the assembly
            Type[] assemblyTypes = assembly.GetTypes();
            //create an instance of the PrestoModule
            PrestoModule module = null;
            foreach (Type type in assemblyTypes) {
                if (type.IsSubclassOf(typeof(PrestoModule))) {
                    module = (PrestoModule)Activator.CreateInstance(type);
                    module.Cluster = GlobalCluster.CreateCluster(domainKey);
                    module.DomainKey = domainKey;
                    break;
                }
            }
            return module;
        }

        /// <summary>
        /// Get the PrestoModule instance loaded in a domain. If no instance exists, one is created.
        /// </summary>
        /// <param name="key">The key of the domain.</param>
        /// <returns>The presto Module Instance.</returns>
        public static PrestoModule GetModuleInstance(string key) {
            if (domainInstance[key] == null) {
                createPrestoInstance(domainAssemblies[key][0], key);
            }
            return domainInstance[key];
        }

        /// <summary>
        /// Unloads and destroys the domain with the specified key. Also deletes any assemblies associated with the domain.
        /// </summary>
        /// <param name="domainKey">The key of the domain to be destroyed.</param>
        public static void DestroyDomain(string domainKey){
            //TODO: destroy a domain
        }

        /// <summary>
        /// Whether or not a domain with a given key exists.
        /// </summary>
        /// <param name="domainKey">The key of the domain to check for.</param>
        /// <returns></returns>
        public static bool HasDomain(string domainKey) {
            if(domains.ContainsKey(domainKey)){
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tells whether the assembly with the given name is loaded into the domain with the given key.
        /// Will return false if the given domain key does not correspond to a currently loaded domain.
        /// </summary>
        /// <param name="domainKey"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static bool DomainHasAssembly(string domainKey, string assemblyName) {
            if (!HasDomain(domainKey)) {
                return false;
            }
            bool isThere = false;
            List<Assembly> assemblies = domainAssemblies[domainKey];
            foreach (Assembly assembly in assemblies) {
                if (assembly.FullName == assemblyName) {
                    isThere = true;
                    break;
                }
            }
            return isThere;
        }

        /// <summary>
        /// Gets the assembly with the specified name from the domain, returns null if it doesnt exist
        /// in the domain or if the domain doesnt exist.
        /// </summary>
        /// <param name="domainKey">The key of the domain to get the assembly from.</param>
        /// <param name="assemblyName">The full name of the assembly.</param>
        /// <returns>The desired assembly or null.</returns>
        public static Assembly GetAssemblyFromDomain(string domainKey, string assemblyName){
            if(!DomainHasAssembly(domainKey, assemblyName)){
                return null;
            }
            Assembly desired = null;
            List<Assembly> assemblies = domainAssemblies[domainKey];
            foreach (Assembly assembly in assemblies) {
                if (assembly.FullName == assemblyName) {
                    desired = assembly;
                    break;
                }
            }
            return desired;
        }

        /// <summary>
        /// Get the COFF-based image of an assembly in a byte array.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>The COFF-based image of an assembly in a byte array.</returns>
        public static byte[] GetAssemblyStream(string assemblyName) {
            return assemblies[assemblyName];
        }
    }
}
