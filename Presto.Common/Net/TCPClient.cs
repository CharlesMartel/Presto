using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Presto.Common;

namespace Presto.Common.Net
{
    /// <summary>
    /// A MessageType based Asynchronous TCP Client to interact with a corresponding MessageType based asynchronous Presto TCP Server.
    /// Adapted from the MSDN example at http://msdn.microsoft.com/en-us/library/bew39x2a.aspx
    /// 
    /// *Note The actual connection is left synchronous to avoid timing issues with the stream writes.
    /// </summary>
	public class TCPClient
	{

        private IPEndPoint serverEndpoint;
        private TcpClient tcpClient;

        // A hash table holding all dispatch references and pointer to their delegates
        private Dictionary<MessageType, Action<ClientState>> dispatchList = new Dictionary<MessageType, Action<ClientState>>(); 
        
        /// <summary>
        /// Create the TCP Client with the specified host and port.
        /// </summary>
        /// <param name="host">String representation of the host on the network.</param>
        /// <param name="port">The port to Connect to on the host.</param>
		public TCPClient (string host, int port)
		{
            //Get the endpoint from DNS and set it as the serverEnpoint
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            serverEndpoint = new IPEndPoint(addresses[0], port);
		}

        /// <summary>
        /// Create the TCP Client with the specified host IPEndpoint.
        /// </summary>
        /// <param name="host">IPEndpoint of the host.</param>
        public TCPClient(IPEndPoint host)
        {
            //set the internal serverEnpoint to the provided one.
            serverEndpoint = host;
        }

        /// <summary>
        /// Connect the Client to the given host server.
        /// </summary>
        public bool Connect()
        {
            //Set the internal tcpClient object
            tcpClient = new TcpClient();
            try {
                //we leave the Client connection synchronous, to avoid timing issue with writes
                tcpClient.Connect(serverEndpoint);
                //get the network stream
                NetworkStream nStream = tcpClient.GetStream();
                //asynchronously read data
                ClientState state = new ClientState(tcpClient);
                nStream.BeginRead(state.Buffer, 0, state.Buffer.Length, readCallback, state);
                return true;
            }
            catch {
                //there was a problem connecting
                return false;
            }
        }

        /// <summary>
        /// Write data to the Socket stream according to the passed in Message Type and String message
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="message"></param>
        public void Write(MessageType mType, string message = null)
        {
            if (message != null)
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
                Write(mType, bytes);
            }
            else
            {
                Write(mType, new byte[0]);
            }
        }

        /// <summary>
        /// Write data to the Socket stream according to the passed in message type and byte data array
        /// </summary>
        /// <param name="mType"></param>
        /// <param name="data"></param>
        public void Write(MessageType mType, byte[] data)
        {
            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(mType);

            //combine the messagetype and data byte arrays
            List<byte> output = new List<byte>();
            output.AddRange(messageTypeEncoded);
            if (data != null && data.Length != 0)
            {
                output.AddRange(data);
            }

            //Write the output
            write(output.ToArray());
        }

        /// <summary>
        /// Internal Write function. Writes the passed in data to the Socket stream.
        /// </summary>
        /// <param name="data">the byte data to be written</param>
        private void write(byte[] data)
        {
            //get the data length and append it to the beggining of the stream
            long dataLength = data.Length;
            byte[] dataLengthArray = BitConverter.GetBytes(dataLength);
            List<byte> tempByteArray = new List<byte>(dataLengthArray);
            tempByteArray.AddRange(data);
            data = tempByteArray.ToArray();

            //get the tcpClient network stream
            NetworkStream nStream = tcpClient.GetStream();
            //Start the synchronous Write
            nStream.BeginWrite(data, 0, data.Length, writeCallback, nStream);
        }

        /// <summary>
        /// Internal asynchronous Write callback method
        /// </summary>
        /// <param name="result"></param>
        private void writeCallback(IAsyncResult result)
        {
            //get the tcpClient network stream
            NetworkStream nStream = tcpClient.GetStream();
            //finish the Write
            nStream.EndWrite(result);
        }


        private void readCallback(IAsyncResult result)
        {
            int read = 0;
            NetworkStream nStream = null;

            try{
                //get the network stream
                nStream = tcpClient.GetStream();
                //end the read
                read = nStream.EndRead(result);
            } catch (ObjectDisposedException e){
                //if the Client has been disposed, we set read to 0 to allow cleanup of the TCPClient
                read = 0;
            }

            //get the server state object
            ClientState state = (ClientState)result.AsyncState;

            if (read > 0)
            {
                state.PurgeBuffer(read);
                //check if the message is fully recieved, if it is, create a new state object and pass that to the read,
                // if not, continue the read with the same state object
                if (state.IsFullyRecieved()) {
                    ClientState newState = new ClientState(state.Client);
                    nStream.BeginRead(newState.Buffer, 0, ClientState.BufferSize, readCallback, newState);
                }
                else {
                    nStream.BeginRead(state.Buffer, 0, ClientState.BufferSize, readCallback, state);
                }
            }
            else
            {
                //Socket has been closed... handle it
                //TODO: handle Socket close
            }
        }


        /// <summary>
        /// Dispatches the recieved message to the appropriate object 
        /// </summary>
        /// <param name="state">The state object to be dispatched.</param>
        private void dispatch(ClientState state)
        {
            //first extract the message type from the message
            string messageType = state.GetMessageType();

            //if messageType is null we return the Unknowm message response
            if (messageType == null)
            {
                Write(MessageType.UNKOWN);
            }

            //find the corresponding message type in the listing and dispatch accordingly, or return Unknown message response
            if (dispatchList.ContainsKey(messageType))
            {
                dispatchList[messageType].BeginInvoke(state, null, null);
            }
            else
            {
                Write(MessageType.UNKOWN);
            }
        }


        /// <summary>
        /// Sets a dispatch action to the dispatch list and triggers that action according to the message type.
        /// </summary>
        /// <param name="messageType">The message type from the Presto.Common.Net.MessageType struct.</param>
        /// <param name="dispatchAction">An Action that recieves a server state object as its only parameter.</param>
        public void setDispatchAction(string messageType, Action<ClientState> dispatchAction)
        {
            dispatchList[messageType] = dispatchAction;
        }

        /// <summary>
        /// Close the Client.
        /// </summary>
        public void close() {
            tcpClient.Close();
        }
        
	}
}

