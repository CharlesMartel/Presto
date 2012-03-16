using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Presto.Common
{
    /// <summary>
    /// Provides the ability to add things to a queue to be processed by a specefied function keeping the processing synchronous but not necessarily ordered.
    /// </summary>
    /// <typeparam name="T">The type of data the queue will hold.</typeparam>
    public class SynchronizedProcessingQueue<T>
    {
        //The queue of data
        private ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
        //the queue count isnt persistent enough for us, we need another counter
        private int counter = 0;
        //the processing function
        private Action<T> processor;
        //internal processing caller
        private Action internalProcessor;
        //is the data being processed
        private bool isProcessing = false;
        //an internal sync mutex to keep timing correct
        private object mutex = new object();

        /// <summary>
        /// Create a new Synchronized Processing Queue with the specified processing function.
        /// </summary>
        /// <param name="processor">The function to process the incoming queue data.</param>
        public SynchronizedProcessingQueue(Action<T> processingFunction)
        {
            processor = processingFunction;
            internalProcessor = new Action(processQueue);
        }

        /// <summary>
        /// Add data to the queue. This data will then be processed by the specified processing function.
        /// </summary>
        /// <param name="data">The data added to the queue.</param>
        public void Add(T data)
        {
            //queue the data and signal the processor if it is not already running.
            queue.Enqueue(data);
            lock (mutex)
            {
                counter++;
                if (!isProcessing)
                {
                    internalProcessor.BeginInvoke(null, null);
                }
            }
        }

        /// <summary>
        /// Process when data gets added to the queue.
        /// </summary>
        private void processQueue()
        {
            lock (mutex)
            {
                isProcessing = true;
            }
            bool stillData = true;
            while (stillData)
            {
                T data;
                bool isdata = queue.TryDequeue(out data);
                if (isdata)
                {
                    processor(data);
                    lock (mutex)
                    {
                        counter--;
                    }
                }
                lock (mutex)
                {
                    if (IsEmpty())
                    {
                        stillData = false;
                        isProcessing = false;
                    }
                }
            }

        }

        /// <summary>
        /// See if the queue is empty. A queue would be empty under two circumstances. 
        /// Either no data has been added or all data has been processed.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (counter > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Block the current thread until all data in the queue has been processed. Accurate to a millisecond.
        /// This should be used under known circumstances, as this will wait indefinitely if data keeps being added to 
        /// the queue. only when the queue has a chance to reach zero entries will the thread be allowed to continue.
        /// </summary>
        public void Wait()
        {
            while (counter > 0)
            {
                Thread.Sleep(1);
            }
        }
    }
}
