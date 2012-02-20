using System;
using System.Net;
using System.Net.Sockets;

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
			
		}
	}
}