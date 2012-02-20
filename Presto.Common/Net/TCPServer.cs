using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Presto.Common;

namespace Presto.Common.Net
{
	/// <summary>
	/// An asynchronous TCP server. 
	/// Adapted from the MSDN example at  http://msdn.microsoft.com/en-us/library/fx6588te.aspx
	/// </summary>
	public class TCPServer
	{
		
		private Socket listener;
		private IPEndPoint ipEndpoint;
		// Thread signal.
		private ManualResetEvent allDone = new ManualResetEvent (false);

		/// <summary>
		/// Initializes a new instance of the <see cref="Presto.Common.Net.TCPServer"/> class. 
		/// An asynchronous tcp socket bound to the specified port.
		/// </summary>
		/// <param name='port'>
		/// The port to bind to.
		/// </param>
		public TCPServer (int port)
		{
			ipEndpoint = new IPEndPoint (IPAddress.Any, port);
			listener = new Socket (ipEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);	
		}

		public void start ()
		{
			//create a listening thread and start the listener
			Thread listenThread = new Thread (listen);
			listenThread.Start ();
		}

		private void listen ()
		{
			//try to bind the enpoint and start the listener
			try {

				listener.Bind (ipEndpoint);
				listener.Listen (10); // the integer passed to listen specifies a maximum backlag size. Im not entirely sure what that actually entails though. 

				while (true) {
					// Set the event to nonsignaled state.
					allDone.Reset ();

					listener.BeginAccept (new AsyncCallback (accept), listener);

					// Wait until a connection is made before continuing.
					allDone.WaitOne ();
				}

			} catch (Exception e) {
				Log.error (e.ToString ());
			}
		}

		private void accept (IAsyncResult ar)
		{
			// Signal the main thread to continue.
			allDone.Set ();

			// Get the socket that handles the client request.
			Socket asyncListener = (Socket)ar.AsyncState;
			Socket handler = asyncListener.EndAccept (ar);

			//Create the state object
			ServerState state = new ServerState (handler)
		}
	}
}