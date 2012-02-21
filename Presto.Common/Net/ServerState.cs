using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
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
		//An appendable byte list that will hold data being flushed from the buffer
        private List<byte> data = new List<byte>();
        

		/// <summary>
		/// Create a new server state object to manage a currently running connection
		/// </summary>
		/// <param name="socket">The sync socket associated with the state object.</param>
		public ServerState (Socket socket)
		{
			//set the working socket
			this.socket = socket;
		}

        /// <summary>
        /// Copies all remaining data in the buffer into the full data array and clears the buffer.
        /// </summary>
        public void purgeBuffer()
        {
            data.AddRange(buffer);
            buffer = new byte[bufferSize];
        }

        /// <summary>
        /// Extracts the message type from the message and returns it as a string. If the message is not long enough to
        /// contain a message type, null is returned. The message type is the first 8 characters of the message in ASCII so
        /// a check is simple.
        /// </summary>
        /// <returns>Will return the MessageType of the request, or null if the message type could not be parsed.</returns>
        public MessageType getMessageType() 
        {
            //make sure data is long enough to contain a message type
            if (!(data.Count > 7)) {
                return null;
            }

            //get the message segment from the transmission, convert to string and convert the string to a message type
            List<byte> messageTypeByteList = data.GetRange(0, 8);
            Byte[] messageTypeByteArray = messageTypeByteList.ToArray();
            return ASCIIEncoding.ASCII.GetString(messageTypeByteArray); 
        }

        /// <summary>
        /// Sends the passed in data as the passed in message type and closes the socket.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="data"></param>
        public void sendAndClose(MessageType messageType, byte[] data = null) 
        {
            //make sure we have a real message type
            if (messageType == null) {
                throw new ArgumentNullException("messageType");
            }

            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(messageType);

            //combine the messagetype and data byte arrays
            List<byte> output = new List<byte>();
            output.AddRange(messageTypeEncoded);
            if (data != null) {
                output.AddRange(data);
            }

            //send the data
            socket.Send(output.ToArray());

            //close the socket
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        /// <summary>
        /// Sends the passed in data as the passed in message type
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="data"></param>
        public void sendData(MessageType messageType, byte[] data = null) 
        {
            //make sure we have a real message type
            if (messageType == null) {
                throw new ArgumentNullException("messageType");
            }

            //get the message type in bytes
            byte[] messageTypeEncoded = ASCIIEncoding.ASCII.GetBytes(messageType);

            //combine the messagetype and data byte arrays
            List<byte> output = new List<byte>();
            output.AddRange(messageTypeEncoded);
            if (data != null) {
                output.AddRange(data);
            }

            //send the data
            socket.Send(output.ToArray());
        }

        /// <summary>
        /// Closes the ServerState's associated socket
        /// </summary>
        public void closeSocket() 
        {
            //close the socket
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
	}
}

