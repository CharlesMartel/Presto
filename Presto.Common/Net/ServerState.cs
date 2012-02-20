using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Presto.Common;

namespace Presto.Common.Net
{
	/// <summary>
	/// ServerState is a state object that gets passed around as the holder for an asynchronous socket.
	/// </summary>
	public class ServerState
	{
		//the internal socket
		public Socket socket;
		// Size of receive buffer.
		public const int bufferSize = 1024;
		// Receive buffer.
		public byte[] buffer = new byte[bufferSize];
		
		/// <summary>
		/// Create a new server state object to manage a currently running connection
		/// </summary>
		/// <param name="socket">The sync socket associated with the state object.</param>
		public ServerState (Socket socket)
		{
			//set the working socket
			this.socket = socket;
		}
	}
}

