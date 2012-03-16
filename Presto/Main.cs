using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using Presto.Common.Net;


namespace Presto {
    class MainClass {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// .
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
            FileStream fs = File.OpenRead(assemblyURL);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            List<byte> message = new List<byte>();
            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(MessageType.ASSEMBLY_TRANSFER_MASTER);
            message.AddRange(messageTypeEncoded);
            message.AddRange(bytes);
            message.AddRange(Config.EndOfStreamPattern);
            NetworkStream stream = client.GetStream();
            stream.Write(message.ToArray(), 0, message.ToArray().Length);
            client.Close();
        }

        /// <summary>
        /// Write the status of the cluster to the console.
        /// </summary>
        public static void status() {
            //TODO: get cluster status and Write to console	
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
