using System;
using Presto;

namespace Basic {
    [Serializable]
    public class Main : PrestoModule {
        public override void Load() {
            DateTime begin = DateTime.Now;
            for (int i = 0; i < 100; i++) {
                //push a new execution of the distributed function into the cluster
                FunctionInput input = new FunctionInput();
                input.value = i;
                Cluster.Execute(distributedFunction, input, functionCallback);
            }
            //We can force a block untill all jobs return from the cluster.
            Cluster.Wait();
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
        public static void functionCallback(PrestoResult result) {
            FunctionOutput output = (FunctionOutput)result;
            Console.WriteLine("Execution number: " + output.value + " from node id: " + output.ExecutionNodeID);
        }
    }


}
