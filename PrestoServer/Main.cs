using System;
using Presto.Common;
using Presto.Common.Net;
using System.IO;

namespace Presto
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            TCPServer serv = new TCPServer(2500);
            serv.Start();
            serv.RegisterDispatchAction(MessageType.ASSEMBLY_TRANSFER_MASTER, configure);
            TCPClient cli = new TCPClient("localhost", 2500);
            cli.Connect();
            FileStream fs = File.OpenRead("Presto.Common.dll");
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            cli.Write(MessageType.ASSEMBLY_TRANSFER_MASTER, bytes);
            cli.close();
		}
		
		public static void configure (ServerState state)
		{
            AssemblyWrapper wrap = new AssemblyWrapper(state.GetDataArray());
            Console.WriteLine(wrap.Validate());
		}
	}
}
