using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Presto.Common {
    /// <summary>
    /// Holds all configuration parameters for the PrestoServer
    /// </summary>
    public static class Config {
        private static string hostsFile = "hosts.conf";
        private static string configFile = "presto.conf";
        private static Dictionary<string, string> configEntries = new Dictionary<string, string>();
        private static List<string> hostEntries = new List<string>();

        /// <summary>
        /// The platform of the runtime beneath the application. > .Net or Mono
        /// </summary>
        public static ExecutionPlatform Platform;

        /// <summary>
        /// A universal id length across the application.
        /// </summary>
        public static int UIDLength = 16;

        /// <summary>
        /// The end of stream pattern is what accompanies the end of each presto network stream.
        /// </summary>
        public static byte[] EndOfStreamPattern;
        private static string endOfStreamFrom = "PRESTOEOS";

        /// <summary>
        /// A list of all assemblies to preload into new domains.
        /// </summary>
        public static List<string> DomainPreloads = new List<string>();

        /// <summary>
        /// Loads and reads the configuration file into the Configuration properties
        /// </summary>
        public static void Initialize() {
            loadConfig();
            loadHosts();

            //discover platform
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix) {
                Platform = ExecutionPlatform.MONO;
            } else {
                Platform = ExecutionPlatform.DOTNET;
            }

            //set the end of stream pattern
            EndOfStreamPattern = ASCIIEncoding.ASCII.GetBytes(endOfStreamFrom);

            //set the domain assembly preloads
            DomainPreloads.Add("LibPresto");
        }

        /// <summary>
        /// load all config parameters into an internal dictionary
        /// </summary>
        private static void loadConfig() {
            //A pretty cool little config file reader procedure I found on stack overflow, edited for my needs
            //http://stackoverflow.com/questions/485659/can-net-load-and-parse-a-properties-file-equivalent-to-java-properties-class

            foreach (string line in File.ReadAllLines(configFile)) {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("'")) &&
                    (!line.StartsWith("\\\\")) &&
                    (line.Contains("="))) {
                    int index = line.IndexOf('=');
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();

                    if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                        (value.StartsWith("'") && value.EndsWith("'"))) {
                        value = value.Substring(1, value.Length - 2);
                    }
                    configEntries.Add(key, value);
                }
            }
        }

        /// <summary>
        /// load all other hosts into a host array
        /// </summary>
        private static void loadHosts() {
            foreach (string line in File.ReadAllLines(hostsFile)) {
                if ((!string.IsNullOrEmpty(line)) &&
                    (!line.StartsWith(";")) &&
                    (!line.StartsWith("#")) &&
                    (!line.StartsWith("\\\\")) &&
                    (!line.StartsWith("'"))) {
                    //add host to host array
                    hostEntries.Add(line.Trim());
                }
            }
        }

        /// <summary>
        /// Get the list of cluster hosts.
        /// </summary>
        /// <returns>A string array of cluster hosts</returns>
        public static string[] GetHosts() {
            return hostEntries.ToArray();
        }

        /// <summary>
        /// Retrieve a parameter from the application configuration.
        /// </summary>
        /// <param name="paramName">The parameter name.</param>
        /// <returns>The parameter value.</returns>
        public static string GetParameter(string paramName) {
            return configEntries[paramName];
        }

    }

    /// <summary>
    /// The runtime that presto is executing on.
    /// </summary>
    public enum ExecutionPlatform {
        /// <summary>
        /// Microsofts .NET Runtime.
        /// </summary>
        DOTNET,
        /// <summary>
        /// The MONO Runtime.
        /// </summary>
        MONO
    }
}
