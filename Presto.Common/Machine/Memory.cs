using System;
using System.Diagnostics;

namespace Presto.Common.Machine {
    
    /// <summary>
    /// Offers details about the memory a current presto intance is running in.
    /// </summary>
    public static class Memory {

        /// <summary>
        /// Internal counter for ram
        /// </summary>
        private static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        /// <summary>
        /// Get the total size of the memory in the computer the Presto instance is running on.
        /// </summary>
        /// <returns>The total size of the memory.</returns>
        public static long GetTotalSize()
        {
            //Not really sure yet how to implement this cross platform...
            //TODO: Implement GetTotalSize
            throw new NotImplementedException();
            //return 0;
        }

        /// <summary>
        /// Get the size of the available memory in the computer the Presto instance is running on.
        /// </summary>
        /// <returns>The amount of available memory.</returns>
        public static float GetAvailable() 
        {
            return ramCounter.NextValue();
        }
    }
}
