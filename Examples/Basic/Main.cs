using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto;

namespace Basic
{
    [Serializable]
    public class Main : PrestoModule
    {
        public override void Load()
        {
            DateTime begin = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                //push a new execution of the distributed function into the cluster
                FunctionInput input = new FunctionInput();
                input.value = i;
                Cluster.Execute(distributedFunction, input, functionCallback);
            }
            Cluster.Wait();
            DateTime end = DateTime.Now;
            TimeSpan lot = end - begin;
            Console.WriteLine("Time taken: " + lot.ToString());
        }

        public override void Unload()
        {
            //any cleanup would go here
            Console.WriteLine("All finished!");
        }

        public static PrestoResult distributedFunction(PrestoParameter param)
        {
            FunctionOutput output = new FunctionOutput();
            FunctionInput input = (FunctionInput)param;
            output.value = input.value;
            return output;
        }

        public static void functionCallback(PrestoResult result)
        {
            FunctionOutput output = (FunctionOutput)result;
            Console.WriteLine("Execution number: " + output.value + " from node id: " + output.ExecutionNodeID);
        }
    }


}
