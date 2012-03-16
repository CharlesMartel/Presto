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
            for (int i = 0; i < 6; i++)
            {
                //push a new execution of the distributed function into the cluster
                Cluster.Execute(distributedFunction, new FunctionInput(), functionCallback);
            }
        }

        public override void Unload()
        {
            //any cleanup would go here
            Console.WriteLine("All finished!");
        }

        public static PrestoResult distributedFunction(PrestoParameter param)
        {
            //we can use the param but since this is a basic example we will just ignore and move on
            FunctionOutput output = new FunctionOutput();
            output.value = 1;
            return output;
        }

        public static void functionCallback(PrestoResult result)
        {
            FunctionOutput output = (FunctionOutput)result;
            Console.WriteLine(output.value);
        }
    }


}
