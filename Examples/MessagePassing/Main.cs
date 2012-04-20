using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Presto;

namespace MessagePassing
{
    public class Main : PrestoModule
    {
        //A short example showing how to send and recieve messages between nodes

        public override void Init()
        {
            //set up the message receiver
            Cluster.MessageRecieved += new Cluster.MessageReceivedHandler(Cluster_MessageRecieved);
        }

        public override void Startup()
        {
            //get all nodes in this cluster and send them a message
            ClusterNode[] nodes = Cluster.GetAvailableNodes();
            foreach (ClusterNode node in nodes)
            {
                Cluster.SendMessage(node, "Hello!");
            }

            //since messages do not necessarily need to be returned or even responded to
            //we simply need to force a thread sleep and wait for all to return
            //other threads will handle the message passing
            System.Threading.Thread.Sleep(1000);
            SignalComplete();
        }

        void Cluster_MessageRecieved(string payload, ClusterNode sender)
        {
            //if this is the response message, write the response to the console.
            if (payload == "response")
            {
                Console.WriteLine("Response from: " + sender.HostName);
            }
            else
            {
                Cluster.SendMessage(sender, "response");
            }
        }
    }
}
