using System;
using System.Net;
using System.Net.Sockets;
using Presto.Common;

namespace Presto.Common.Net
{
	/// <summary>
	/// An asynchronous TCP server. 
	/// Adapted from the MSDN example at  http://msdn.microsoft.com/en-us/library/5w7b7x5f.aspx
	/// </summary>
	public class TCPServer
	{
		
		private Socket listener;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Presto.Common.Net.TCPServer"/> class. 
		/// An asynchronous tcp socket bound to the specified port.
		/// </summary>
		/// <param name='port'>
		/// The port to bind to.
		/// </param>
		public TCPServer (int port)
		{
			IPEndPoint ipEndpoint = new IPEndPoint (IPAddress.Any, port);
			listener = new Socket (ipEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
	
			//try to bind the enpoint and start the listener
			try {
				listener.Bind (ipEndpoint);
				listener.Listen (10); // the integer passed to listen specifies a maximum backlag size. Im not entirely sure what that actually means though. 
			} catch (Exception e) {
				//TODO: write exception to error log
			}			
		}
	}
}