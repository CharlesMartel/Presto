using System;
using System.Collections.Generic;

namespace Presto.Machine {

    /// <summary>
    /// Gathers the distribution performance indicator Information for the machine and for the cluster.
    /// </summary>
    public static class DPI {

        /// <summary>
        /// This machines individual dpi
        /// </summary>
        private static double dpi = 0;

        /// <summary>
        /// Calculate DPI of the current machine.
        /// </summary>
        /// <returns>The "long" value of this machines DPI</returns>
        public static void CalculateDPI() {
            //I have no other method of measuring performance than to do a simple 
            //prime number finder to 1000 and measure the speed of the result and then multiply the result by the
            //total number of cores, there are no optimizations here, we don't want any, we want this to be a tad slow and fill up a list
            //so we get a test on not just the cpu, but also allocation time. We also throw in some random floating point and string
            //ops for... fun mostly... Realistically, all this is made up as I wrote it... kinda entertaining actually... I've decided
            //that every number needs a unit, so I present to you the rather arbitrary, and somewhat stupid.. Presto Compute Unit
            //A DPI is measured in PCU... if that becomes a thing I swear I'll laugh my ass off for days...
            long start = DateTime.Now.Ticks;
            //we hold the not primes instead to consume more memory
            List<int> notPrimeHolder = new List<int>();
            List<double> someList = new List<double>();
            List<string> stringList = new List<string>();
            for (int i = 1; i <= 10000; i++) {
                for (int j = 2; j < i; i++) {
                    if (i % j == 0) {
                        notPrimeHolder.Add(i);
                        break;
                    }
                }
                // flp ops
                double k = i / 5.73578 + 7.99999 * 32.39858 - 5.6982;
                k = k * 23465.78032 / 90543.4589021 + 78342.49480932 - 473829.48392;
                someList.Add(k);
                //string ops
                string t = k.ToString();
                int u = t.IndexOf(".");
                string h = t.Replace(".", "DOT");
                stringList.Add(h);
            }
            long end = DateTime.Now.Ticks;
            long roundTrip = end - start;
            double reversed = Math.Pow(10, 6) - roundTrip;
            dpi = reversed;
            dpi = dpi * CPU.GetCount();
            //Probably shoulda just implemented whetstone or something instead...
        }

        /// <summary>
        /// Get the DPI for the running machine.
        /// </summary>
        /// <returns>The DPI of the current machine.</returns>
        public static double GetDPI() {
            return dpi;
        }
    }
}
