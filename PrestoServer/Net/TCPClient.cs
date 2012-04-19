using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Presto.Common;

namespace Presto.Net {
    /// <summary>
    /// A MessageType based Asynchronous TCP Client to interact with a corresponding MessageType based asynchronous Presto TCP Server.
    /// Adapted from the MSDN example at http://msdn.microsoft.com/en-us/library/bew39x2a.aspx
    /// 
    /// *Note The actual connection is left synchronous to avoid timing issues with the stream writes.
    /// </summary>
    public class TCPClient : IDisposable {

        private IPEndPoint serverEndpoint;
        private TcpClient tcpClient;

        // A hash table holding all dispatch references and pointer to their delegates
        private Dictionary<MessageType, Action<ClientState>> dispatchList = new Dictionary<MessageType, Action<ClientState>>();

        /// <summary>
        /// Create the TCP Client with the specified host and port.
        /// </summary>
        /// <param name="host">String representation of the host on the network.</param>
        /// <param name="port">The port to Connect to on the host.</param>
        public TCPClient(string host, int port) {
            //Get the endpoint from DNS and set it as the serverEnpoint
            IPAddress[] addresses = Dns.GetHostAddresses(host);
            serverEndpoint = new IPEndPoint(addresses[0], port);
        }

        /// <summary>
        /// Create the TCP Client with the specified host IPEndpoint.
        /// </summary>
        /// <param name="host">IPEndpoint of the host.</param>
        public TCPClient(IPEndPoint host) {
            //set the internal serverEnpoint to the provided one.
            serverEndpoint = host;
        }

        /// <summary>
        /// Whether or not the connection is active.
        /// </summary>
        /// <returns>True if it is, false if it is not.</returns>
        public bool IsConnected() {
            return tcpClient.Connected;
        }

        /// <summary>
        /// Connect the Client to the given host server.
        /// </summary>
        public bool Connect() {
            //Set the internal tcpClient object
            tcpClient = new TcpClient();
            //tcpClient.NoDelay = true; // again.. for nagles... and its entirely too much to explain.
            try {
                //we leave the Client connection synchronous, to avoid timing issue with writes
                tcpClient.Connect(serverEndpoint);
                //get the network stream
                NetworkStream nStream = tcpClient.GetStream();
                //asynchronously read data
                ClientState state = new ClientState(tcpClient);
                nStream.BeginRead(state.Buffer, 0, state.Buffer.Length, readCallback, state);
                return true;
            } catch (Exception e) {
                //there was a problem connecting
                Log.Error(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Reconnects the client to the given host.
        /// </summary>
        /// <returns></returns>
        public bool ReConnect() {
            try {
                //Set the internal tcpClient object
                tcpClient = new TcpClient();
                //tcpClient.NoDelay = true; // again.. for nagles... and its entirely too much to explain.
                //we leave the Client connection synchronous, to avoid timing issue with writes
                tcpClient.Connect(serverEndpoint);
                //get the network stream
                NetworkStream nStream = tcpClient.GetStream();
                //asynchronously read data
                ClientState state = new ClientState(tcpClient);
                nStream.BeginRead(state.Buffer, 0, state.Buffer.Length, readCallback, state);
                return true;
            } catch (Exception e) {
                Log.Error(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Write data to the socket stream according to the passed in Message Type and String message
        /// </summary>
        /// <param name="mType">The message type  of the message.</param>
        /// <param name="message">The message as a string.</param>
        public void Write(MessageType mType, string message = null) {
            if (message != null) {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
                Write(mType, bytes);
            } else {
                Write(mType, new byte[0]);
            }
        }

        /// <summary>
        /// Write data to the socket stream according to the passed in message type and byte data array
        /// </summary>
        /// <param name="mType">The message type of the message.</param>
        /// <param name="data">The byte array of the message.</param>
        public void Write(MessageType mType, byte[] data) {
            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(mType);

            //combine the messagetype and data byte arrays
            List<byte> output = new List<byte>();
            output.AddRange(messageTypeEncoded);
            if (data != null && data.Length != 0) {
                output.AddRange(data);
            }

            //Write the output
            write(output.ToArray());
        }

        /// <summary>
        /// Writes the data to the socket and blocks the thread until all data has been written from the biffer into the stream.
        /// Use tentatively, as this also waits for other data in the buffer to be written.
        /// </summary>
        /// <param name="mType">The message type.</param>
        /// <param name="data">A byte array of the data to be written.</param>
        public void WaitWrite(MessageType mType, byte[] data) {
            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(mType);

            //combine the messagetype and data byte arrays
            List<byte> output = new List<byte>();
            output.AddRange(messageTypeEncoded);
            if (data != null && data.Length != 0) {
                output.AddRange(data);
            }

            //Write the output
            write(output.ToArray());
        }

        /// <summary>
        /// Internal Write function. Writes the passed in data to the socket stream.
        /// </summary>
        /// <param name="data">The byte data to be written</param>
        private bool write(byte[] data) {
            try {
                long datalength = data.Length;
                byte[] datalengtharray = BitConverter.GetBytes(datalength);
                List<byte> holder = new List<byte>();
                holder.AddRange(datalengtharray);
                holder.AddRange(data);
                data = holder.ToArray();

                //get the tcpClient network stream
                NetworkStream nStream = tcpClient.GetStream();
                //Start the synchronous Write
                nStream.BeginWrite(data, 0, data.Length, null, null);
                return true;
            } catch (Exception e) {
                // the connection was closed return false and the synchronizer will take care of it
                Log.Error(e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Callback fired after a read operation completed on the socket
        /// </summary>
        /// <param name="result">The result of the read.</param>
        private void readCallback(IAsyncResult result) {

            //Who wrote this method? I'm not claiming it... nope.

            int read = 0;
            NetworkStream nStream = null;

            try {
                //get the network stream
                nStream = tcpClient.GetStream();
                //end the read
                read = nStream.EndRead(result);
            } catch (ObjectDisposedException e) {
                Log.Error(e.ToString());
                //if the Client has been disposed, we set read to 0 to allow cleanup of the TCPClient
                read = 0;
            } catch (System.IO.IOException e) {
                //the client got disconnected, that is fine, we simply need to re open with the reconnect, that will happen with the verify interval
                Log.Warning("Client: '" + tcpClient.Client.AddressFamily.ToString() + "' disconnected! Exception: " + e.ToString());
                read = 0;
            }


                //get the server state object
                ClientState state = (ClientState)result.AsyncState;

                if (read > 0) {
                    state.PurgeBuffer(read);
                    //check if the message is fully recieved, if it is, create a new state object and pass that to the read,
                    // if not, continue the read with the same state object
                    if (state.IsFullyRecieved()) {
                        //need to redo all this below.. nto as efficient as I'd like
                        ClientState newState = new ClientState(state.Client);
                        byte[] excessData = state.CompleteAndTrim();
                        dispatch(state);
                        newState.PreSetData(excessData);
                        bool moreToProcess = newState.IsFullyRecieved();
                        if (moreToProcess) {
                            do {
                                //this is a terrible terrible process and needs to be rewritten.
                                ClientState newStateRepeat = new ClientState(state.Client);
                                newStateRepeat.PreSetData(excessData);
                                excessData = newStateRepeat.CompleteAndTrim();
                                dispatch(newStateRepeat);
                                newState.PreSetData(excessData);
                            } while (newState.IsFullyRecieved());
                            nStream.BeginRead(newState.Buffer, 0, ClientState.BufferSize, readCallback, newState);
                        } else {
                            nStream.BeginRead(newState.Buffer, 0, ClientState.BufferSize, readCallback, newState);
                        }
                    } else {
                        nStream.BeginRead(state.Buffer, 0, ClientState.BufferSize, readCallback, state);
                    }
                } else {
                    //socket has been closed... handle it
                    //TODO: handle socket close
                }
        }


        /// <summary>
        /// Dispatches the recieved message to the appropriate object 
        /// </summary>
        /// <param name="state">The state object to be dispatched.</param>
        private void dispatch(ClientState state) {
            //first extract the message type from the message
            string messageType = state.GetMessageType();

            //if messageType is null we return the Unknowm message response
            if (messageType == null) {
                Write(MessageType.UNKOWN);
            }

            //find the corresponding message type in the listing and dispatch accordingly, or return Unknown message response
            if (dispatchList.ContainsKey(messageType)) {
                dispatchList[messageType].BeginInvoke(state, null, null);
            } else {
                Write(MessageType.UNKOWN);
            }
        }


        /// <summary>
        /// Sets a dispatch action to the dispatch list and triggers that action according to the message type.
        /// </summary>
        /// <param name="messageType">The message type from the Presto.Common.Net.MessageType struct.</param>
        /// <param name="dispatchAction">An Action that recieves a server state object as its only parameter.</param>
        public void SetDispatchAction(string messageType, Action<ClientState> dispatchAction) {
            dispatchList[messageType] = dispatchAction;
        }

        /// <summary>
        /// Close the Client.
        /// </summary>
        public void close() {
            tcpClient.Close();
        }

        /// <summary>
        /// Closes and disposes the TCP client.
        /// </summary>
        public void Dispose() {
            close();
            GC.SuppressFinalize(this);
        }
    }
}

