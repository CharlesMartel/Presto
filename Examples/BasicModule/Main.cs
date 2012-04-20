using System;
using Presto;

namespace BasicModule{

    public class Main : PrestoModule {
        
        public static long numjobs = 0;

        public override void Init()
        {
            //Nothing to initialize
        }

        public override void Startup() {
            //get the start time of the operation
            DateTime begin = DateTime.Now;

            //throw 250 jobs at the cluster.
            for (int i = 0; i < 250; i++) {
                //push a new execution of the distributed function into the cluster
                FunctionInput input = new FunctionInput();
                input.value = i;
                Cluster.Execute(distributedFunction, input, callback);
            }

            //We can force a block until all jobs return from the cluster.
            Cluster.Wait();

            //get the end time of the operation.
            DateTime end = DateTime.Now;
            TimeSpan lot = end - begin;

            //Write the length of time to the console.
            Console.WriteLine("Time taken: " + lot.ToString());

            //Cleanup and Complete.
            SignalComplete();
        }

        //The function that will be distributed to other nodes.
        public static PrestoResult distributedFunction(PrestoParameter param) {
            FunctionOutput output = new FunctionOutput();
            FunctionInput input = (FunctionInput)param;
            output.value = input.value;
            return output;
        }

        //The callback called after the function completes.
        public static void callback(PrestoResult result) {
            FunctionOutput output = (FunctionOutput)result;
            Console.WriteLine("Execution number: " + output.value + " from node id: " + output.ExecutionNode.HostName);
            System.Threading.Interlocked.Increment(ref numjobs);
        }

    }


}
