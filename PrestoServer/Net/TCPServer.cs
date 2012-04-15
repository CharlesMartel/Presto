using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Presto.Common;

namespace Presto.Net {
    /// <summary>
    /// An asynchronous TCP server. 
    /// Adapted from the MSDN example at  http://msdn.microsoft.com/en-us/library/fx6588te.aspx
    /// 
    /// This server allows dispatching according to the included MessageTypes structure. By simply associating message types
    /// with Actions that recieve the ServerState Object as their only paramater, we can leave the dispatching internal to the 
    /// tcp server. The message types are the first 8 ASCII characters / First 8 bytes of the tcp transmission.
    /// 
    /// The server is entirely asynchronous and all dispatched actions get thrown into the thread pool.
    /// </summary>
    public class TCPServer {

        private Socket listener;
        private IPEndPoint ipEndpoint;
        // Thread signal.
        private ManualResetEvent allDone = new ManualResetEvent(false);
        // A hash table holding all dispatch references and pointer to their delegates
        private Dictionary<MessageType, Action<ServerState>> dispatchList = new Dictionary<MessageType, Action<ServerState>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Presto.Net.TCPServer"/> class. 
        /// An asynchronous tcp socket bound to the specified port.
        /// </summary>
        /// <param name='port'>
        /// The port to bind to.
        /// </param>
        public TCPServer(int port) {
            ipEndpoint = new IPEndPoint(IPAddress.Any, port);
            listener = new Socket(ipEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Start up the tcp server and begin accepting clients.
        /// </summary>
        public void Start() {
            //create a listening thread and Start the listener
            Thread listenThread = new Thread(listen);
            listenThread.Start();
            listenThread.Priority = ThreadPriority.Highest;
        }

        /// <summary>
        /// Internal: starts up the listening loop
        /// </summary>
        private void listen() {
            //try to bind the enpoint and Start the listener
            try {

                listener.Bind(ipEndpoint);
                listener.Listen(2048); // the integer passed to listen specifies a maximum backlag size. Im not entirely sure what that actually entails though. 
                //listener.NoDelay = true; // this is for nagles... bleh... i dont feel like explaining.

                while (true) {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    listener.BeginAccept(new AsyncCallback(accept), listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            } catch (Exception e) {
                Log.Error(e.ToString());
            }
        }


        /// <summary>
        /// Internal: Connect to the Client and spin off a recieve callback
        /// </summary>
        /// <param name="ar"></param>
        private void accept(IAsyncResult ar) {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the Client request.
            Socket asyncListener = (Socket)ar.AsyncState;
            Socket handler = asyncListener.EndAccept(ar);

            //Create the state object
            ServerState state = new ServerState(handler);
            //begin recieving the data
            state.socket.BeginReceive(state.Buffer, 0, ServerState.BufferSize, 0, new AsyncCallback(read), state);
        }

        /// <summary>
        /// read data sent from the Client, called recursively until all data is receieved
        /// </summary>
        /// <param name="ar"></param>
        private void read(IAsyncResult ar) {

            //I absolutely hate the implementation of this. I hate it so much. Need to rewrite fully.

            try {
                //Get the Server State Object
                ServerState state = (ServerState)ar.AsyncState;

                //read from the socket
                int readCount = state.socket.EndReceive(ar);

                //check if reading is done, move on if so. then trigger another read
                if (readCount > 0) {
                    //purge the Buffer and Start another read
                    state.PurgeBuffer(readCount);
                    //check if the message is fully recieved, if it is, create a new state object and pass that to the read,
                    // if not, continue the read with the same state object
                    if (state.IsFullyRecieved()) {
                        //Need to redo all this below, not as efficient as id like
                        ServerState newState = new ServerState(state.socket);
                        byte[] excessData = state.CompleteAndTrim();
                        dispatch(state);
                        newState.PreSetData(excessData);
                        bool moreToProcess = newState.IsFullyRecieved();
                        if (moreToProcess) {
                            do {
                                ServerState newStateRepeat = new ServerState(state.socket);
                                newStateRepeat.PreSetData(excessData);
                                excessData = newStateRepeat.CompleteAndTrim();
                                dispatch(newStateRepeat);
                                newState.PreSetData(excessData);
                            } while (newState.IsFullyRecieved());
                            newState.socket.BeginReceive(newState.Buffer, 0, ServerState.BufferSize, 0, new AsyncCallback(read), newState);
                        } else {
                            newState.socket.BeginReceive(newState.Buffer, 0, ServerState.BufferSize, 0, new AsyncCallback(read), newState);
                        }
                    } else {
                        state.socket.BeginReceive(state.Buffer, 0, ServerState.BufferSize, 0, new AsyncCallback(read), state);
                    }
                } else {
                    //socket has been closed... handle it
                    //TODO: handle socket close
                }
            } catch (Exception e) {
                //The server was closed.
                Log.Error(e.ToString());
            }
        }

        /// <summary>
        /// Dispatches the recieved message to the appropriate object 
        /// </summary>
        /// <param name="state">The state object to be dispatched.</param>
        private void dispatch(ServerState state) {
            //first extract the message type from the message
            string messageType = state.GetMessageType();

            //if messageType is null we return the Unknowm message response
            if (messageType == null) {
                state.Write(MessageType.UNKOWN);
            }

            //find the corresponding message type in the listing and dispatch accordingly, or return Unknown message response
            if (dispatchList.ContainsKey(messageType)) {
                dispatchList[messageType].BeginInvoke(state, null, null);
            } else {
                state.Write(MessageType.UNKOWN);
            }
        }


        /// <summary>
        /// Sets a dispatch action to the dispatch list and triggers that action according to the message type.
        /// </summary>
        /// <param name="messageType">The message type from the Presto.Common.Net.MessageType struct.</param>
        /// <param name="dispatchAction">An Action that recieves a server state object as its only parameter.</param>
        public void RegisterDispatchAction(MessageType messageType, Action<ServerState> dispatchAction) {
            dispatchList[messageType] = dispatchAction;
        }
    }
}