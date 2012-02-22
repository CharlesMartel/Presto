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

        /// <summary>
        /// Because all cpus are different, we cannot simply rely on percentage to decide use, so we generate a rating
        /// based on processor capability, this method returns that rating.
        /// </summary>
        /// <returns></returns>
        public static int GetCPURating() 
        {
            return 0;
        }

        /// <summary>
        /// The DPI (Distribution Performance Indicator) is number representing what the current machine has left to offer
        /// to the cluster. It is an absolute number based on the relative cpu rating that gives a standard value to base
        /// distribution decisions on. This method returns that value.
        /// </summary>
        public static float GetDPI(){
            return 0;
        }
    }
}
