using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Presto.Common {
    /// <summary>
    /// Provides the ability to add things to a queue to be processed by a specefied function keeping the processing synchronous but not necessarily ordered.
    /// </summary>
    /// <typeparam name="T">The type of data the queue will hold.</typeparam>
    public class SynchronizedProcessingQueue<T> {
        //The queue of data
        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        //the queue count isnt persistent enough for us, we need another counter
        private long counter = 0;
        //the processing function
        private Func<T, bool> processor;
        //internal processing caller
        private Action internalProcessor;
        //is the data being processed
        private long isProcessing = 0;

        /// <summary>
        /// Create a new Synchronized Processing Queue with the specified processing function.
        /// </summary>
        /// <param name="processingFunction">The function to process the incoming queue data.</param>
        public SynchronizedProcessingQueue(Func<T, bool> processingFunction) {
            processor = processingFunction;
            internalProcessor = new Action(processQueue);
        }

        /// <summary>
        /// Add data to the queue. This data will then be processed by the specified processing function.
        /// </summary>
        /// <param name="data">The data added to the queue.</param>
        public void Add(T data) {
            //queue the data and signal the processor if it is not already running.
            queue.Enqueue(data);
            Interlocked.Increment(ref counter);
            if (Interlocked.Read(ref isProcessing) < 1) {
                internalProcessor.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// Process when data gets added to the queue.
        /// </summary>
        private void processQueue() {
            Interlocked.Increment(ref isProcessing);
            bool stillData = true;
            while (stillData) {
                T data;
                bool isdata = queue.TryDequeue(out data);
                if (isdata) {
                    //process the data.
                    bool processed = processor(data);
                    if (processed) {
                        Interlocked.Decrement(ref counter);
                    } else {
                        //add data back into queue to be processed
                        queue.Enqueue(data);
                    }
                }
                if (IsEmpty()) {
                    stillData = false;
                    Interlocked.Decrement(ref isProcessing);
                }
            }

        }

        /// <summary>
        /// See if the queue is empty. A queue would be empty under two circumstances. 
        /// Either no data has been added or all data has been processed.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() {
            if (Interlocked.Read(ref counter) > 0) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Block the current thread until all data in the queue has been processed. Accurate to a millisecond.
        /// This should be used under known circumstances, as this will wait indefinitely if data keeps being added to 
        /// the queue. only when the queue has a chance to reach zero entries will the thread be allowed to continue.
        /// Please understand.. this is a work around because I suck.
        /// </summary>
        public void Wait() {
            while (Interlocked.Read(ref counter) > 0) {
                Thread.Sleep(1);
            }
        }
    }
}
