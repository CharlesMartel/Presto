using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Presto.Common;
using Presto.Net;


namespace Presto {

    /// <summary>
    /// Entry for Presto Main
    /// </summary>
    class MainClass {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The arguments for presto
        /// </param>
        public static void Main(string[] args) {
            //Initialize the configuration
            Config.Initialize();
            //make sure we have at least one argument, if not, run usage
            if (!(args.Length > 0)) {
                usage();
                return;
            }
            //switch on the arguments 
            switch (args[0]) {
                case "exec":
                    //make sure there is a second argument to pass as the assembly url
                    if (!(args.Length > 1)) {
                        usage();
                        break;
                    }
                    //exec the assembly
                    exec(args[1]);
                    break;

                case "status":
                    status();
                    break;

                default:
                    usage();
                    break;
            }
        }

        /// <summary>
        /// Deploy the specified assembly into the cluster. 
        /// </summary>
        /// <param name='assemblyURL'>
        /// The url of the assembly to load and execute
        /// </param>
        public static void exec(string assemblyURL) {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", Int32.Parse(Config.GetParameter("SERVER_PORT")));
            if (!File.Exists(assemblyURL)) {
                assemblyURL += ".dll";
            }
            FileStream fs = File.OpenRead(assemblyURL);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            List<byte> message = new List<byte>();
            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(MessageType.ASSEMBLY_TRANSFER_MASTER);
            message.AddRange(BitConverter.GetBytes((long)(bytes.Length + messageTypeEncoded.Length)));
            message.AddRange(messageTypeEncoded);
            message.AddRange(bytes);
            NetworkStream stream = client.GetStream();
            stream.Write(message.ToArray(), 0, message.ToArray().Length);
            client.LingerState.Enabled = true;
            client.LingerState.LingerTime = 30;
            client.Close();
        }

        /// <summary>
        /// Write the status of the cluster to the console.
        /// </summary>
        public static void status() {
            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", Int32.Parse(Config.GetParameter("SERVER_PORT")));
            NetworkStream stream = client.GetStream();
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(MessageType.STATUS_TERMINAL);
            List<byte> message = new List<byte>(BitConverter.GetBytes((long)(8)));
            message.AddRange(messageTypeEncoded);
            stream.Write(message.ToArray(), 0, message.ToArray().Length);
            byte[] messageSize = new byte[8];
            stream.Read(messageSize, 0, 8);
            long size = BitConverter.ToInt64(messageSize, 0);
            int sizeInt = (int)size;
            byte[] dataArray = new byte[sizeInt];
            stream.Read(dataArray, 0, sizeInt);
            List<byte> datas = new List<byte>(dataArray);
            datas.RemoveRange(0, 8);
            SerializationEngine serializer = new SerializationEngine();
            ServerStatus status = (ServerStatus)serializer.Deserialize(datas.ToArray());

            
            //Console Writes
            Console.WriteLine("Node Count: " + status.NodeCount);
            Console.WriteLine("Cluster DPI: " + status.CDPI);
            Console.WriteLine("Total CPU Count: " + status.CPUCount);
            Console.WriteLine("Total Memory: " + status.TotalMemory);
        }

        /// <summary>
        /// Displays the usage of the Presto commandline program.
        /// </summary>
        public static void usage() {
            //TODO: Fill out usage for Presto
            Console.WriteLine("Usage stuffs");
        }


    }
}
