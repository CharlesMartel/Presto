using System.Diagnostics;
using Presto.Common;

namespace Presto.Machine {

    /// <summary>
    /// Offers details about the memory a current presto intance is running in.
    /// </summary>
    public static class Memory {

        /// <summary>
        /// Internal counter for ram
        /// </summary>
        private static PerformanceCounter ramCounter = null;


        /// <summary>
        /// Get the total size of the memory in the computer the Presto instance is running on.
        /// </summary>
        /// <returns>The total size of the memory.</returns>
        public static long GetTotalSize() {
            if (ramCounter == null) {
                if (Config.Platform == ExecutionPlatform.DOTNET) {
                    ramCounter = new PerformanceCounter("Memory", "Available Bytes");
                } else if (Config.Platform == ExecutionPlatform.MONO) {
                    ramCounter = new PerformanceCounter("Mono Memory", "Total Physical Memory");
                }
                ramCounter.NextValue();
            }

            return (long)ramCounter.NextValue();
        }
    }
}
