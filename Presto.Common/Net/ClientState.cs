using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

namespace Presto.Common.Net {
    /// <summary>
    /// ServerState is a state object that gets passed around as the holder for an asynchronous Socket.
    /// </summary>
    public class ClientState {
        //the internal Socket
        public TcpClient Client;
        // Size of receive Buffer.
        public const int BufferSize = 1024;
        // Receive Buffer.
        public byte[] Buffer = new byte[BufferSize];
        //An appendable byte list that will hold data being flushed from the Buffer
        private List<byte> data = new List<byte>();
        //we keep the message size around
        public long MessageLength = 0;
        //a boolean to tell if the message is fully recieved
        private bool messageFullyRecieved = false;
        //the private serializer
        private SoapFormatter serializer = new SoapFormatter();

        /// <summary>
        /// Create a new server state object to manage a currently running connection
        /// </summary>
        /// <param name="Socket">The sync Socket associated with the state object.</param>
        public ClientState(TcpClient client) {
            //set the working Client
            this.Client = client;
        }

        /// <summary>
        /// Copies all remaining data in the Buffer into the full data array and clears the Buffer.
        /// </summary>
        public void PurgeBuffer(int bytesRead) {
            //we copy the bytes read out of the Buffer and add it to the data list
            data.AddRange(new List<byte>(Buffer).GetRange(0, bytesRead));
            Buffer = new byte[BufferSize];

            //see if the message is fully recieved and set the messageFullyRecieved boolean if so
            List<byte> messageLengthLongList = data.GetRange(0, 8);
            byte[] messageLengthLongArray = messageLengthLongList.ToArray();
            MessageLength = BitConverter.ToInt64(messageLengthLongArray, 0);
            if (data.Count >= MessageLength + 16) {
                messageFullyRecieved = true;
            }
        }

        /// <summary>
        /// Extracts the message type from the message and returns it as a string. If the message is not long enough to
        /// contain a message type, null is returned. The message type is the first 8 characters of the message in ASCII so
        /// a check is simple.
        /// </summary>
        /// <returns>Will return the MessageType of the request, or null if the message type could not be parsed.</returns>
        public MessageType GetMessageType() {
            //make sure data is long enough to contain a message type
            if (!(data.Count > 15)) {
                return null;
            }

            //get the message segment from the transmission, convert to string and convert the string to a message type
            List<byte> messageTypeByteList = data.GetRange(8, 8);
            byte[] messageTypeByteArray = messageTypeByteList.ToArray();
            return ASCIIEncoding.ASCII.GetString(messageTypeByteArray);
        }

        /// <summary>
        /// When a message is fully recieved an internal boolean gets set to true. This is a way for outsiders to check on that
        /// internal property.
        /// </summary>
        /// <returns>boolean telling if the message has been fully recieved</returns>
        public bool IsFullyRecieved() {
            return messageFullyRecieved;
        }

        /// <summary>
        /// Get the data portion of the message as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetDataArray() {
            List<byte> dataByteArray = data.GetRange(16, data.Count - 16);
            return dataByteArray.ToArray();
        }

        /// <summary>
        /// Get the data portion of the message as an ASCII encoded string
        /// </summary>
        /// <returns></returns>
        public string GetDataASCIIString() {
            List<byte> dataByteArray = data.GetRange(16, data.Count - 16);
            return ASCIIEncoding.ASCII.GetString(dataByteArray.ToArray());
        }

        /// <summary>
        /// Get the data inside the message as a generic object that has been deserialized.
        /// </summary>
        /// <returns>The generic object deserialized.</returns>
        public Object GetDataDeserialized() {
            List<byte> dataByteArray = data.GetRange(16, data.Count - 16);
            return serializer.Deserialize(new MemoryStream(dataByteArray.ToArray()));
        }
    }
}

