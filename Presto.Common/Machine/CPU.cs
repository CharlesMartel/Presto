using System;
using System.Diagnostics;

namespace Presto.Common.Machine {

    /// <summary>
    /// Offers details about the cpu a current presto intance is running on.
    /// </summary>
    public static class CPU {
         
        /// <summary>
        /// The performance counter that will deliver performance information on the cpu
        /// </summary>
        private static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        /// <summary>
        /// Get the total cpu usage percentage.
        /// </summary>
        /// <returns></returns>
        public static float GetUsage() 
        {
            return cpuCounter.NextValue();
        }
    }
}
