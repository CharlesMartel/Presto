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
        private Socket socket;

        /// <summary>
        /// Create a new server state object to manage a currently running connection
        /// </summary>
        /// <param name="handler"></param>
		public ServerState (Socket handler)
		{
            //set the working socket
            socket = handler;
		}
	}
}

