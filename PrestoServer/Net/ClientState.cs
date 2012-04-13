using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Presto.Net {
    /// <summary>
    /// ServerState is a state object that gets passed around as the holder for an asynchronous socket.
    /// </summary>
    public class ClientState {
        /// <summary>
        /// The internal TcpClient.
        /// </summary>
        public TcpClient Client;
        /// <summary>
        /// Size of the recieve buffer.
        /// </summary>
        public const int BufferSize = 16384;
        /// <summary>
        /// The internal byte buffer.
        /// </summary>
        public byte[] Buffer = new byte[BufferSize];
        //An appendable byte list that will hold data being flushed from the Buffer
        private List<byte> data = new List<byte>();
        //a boolean to tell if the message is fully recieved
        private bool messageFullyRecieved = false;

        /// <summary>
        /// Create a new server state object to manage a currently running connection
        /// </summary>
        /// <param name="client">The tcp client object associated with this ClientState.</param>
        internal ClientState(TcpClient client) {
            //set the working Client
            this.Client = client;
        }

        /// <summary>
        /// Copies all remaining data in the Buffer into the full data array and clears the Buffer.
        /// </summary>
        internal void PurgeBuffer(int bytesRead) {
            //we copy the bytes read out of the Buffer and add it to the data list
            data.AddRange(new List<byte>(Buffer).GetRange(0, bytesRead));
            Buffer = new byte[BufferSize];
        }

        /// <summary>
        /// Sometimes, multiple messages can get caught in the same buffer and get added into the state object. What we do in that
        /// is call the trim excess and simply take the excess and throw it into the next read on the stream, so we keep the data
        /// in line. This gets called at the end of every full read.
        /// </summary>
        /// <returns>The byte array of any excess data.</returns>
        internal byte[] CompleteAndTrim() {
            long dataArrayLength = BitConverter.ToInt64(data.ToArray(), 0);
            List<byte> fullTransfer = data.GetRange(0, (int)dataArrayLength + 8);
            data.RemoveRange(0, (int)dataArrayLength + 8);
            List<byte> excess = data;
            data = fullTransfer;
            return excess.ToArray();
        }

        /// <summary>
        /// preset what data is in the data array.
        /// </summary>
        /// <param name="presetData">The data to preset the data array with.</param>
        internal void PreSetData(byte[] presetData) {
            data.Clear();
            data.AddRange(presetData);
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
        internal bool IsFullyRecieved() {
            recalcMessageFullyRecieved();
            return messageFullyRecieved;
        }

        /// <summary>
        /// Internal function to recaculate if the message is fully recieved.
        /// </summary>
        private void recalcMessageFullyRecieved() {
            if (data.Count < 8) {
                messageFullyRecieved = false;
                return;
            }
            List<byte> datalengtharray = data.GetRange(0, 8);
            long datalength = BitConverter.ToInt64(datalengtharray.ToArray(), 0);
            if (data.Count >= datalength + 8) {
                messageFullyRecieved = true;
            } else {
                messageFullyRecieved = false;
            }
        }

        /// <summary>
        /// Get the data portion of the message as a byte array
        /// </summary>
        /// <returns>The data as an array.</returns>
        public byte[] GetDataArray() {
            List<byte> dataByteArray = data.GetRange(16, data.Count - 16);
            return dataByteArray.ToArray();
        }

        /// <summary>
        /// Get the data portion of the message as an ASCII encoded string
        /// </summary>
        /// <returns>The message data as an string encoded in ASCII.</returns>
        public string GetDataASCIIString() {
            List<byte> dataByteArray = new List<byte>(GetDataArray());
            return ASCIIEncoding.ASCII.GetString(dataByteArray.ToArray());
        }

        /// <summary>
        /// Get the data passed in with the message as a memory stream.
        /// </summary>
        /// <returns>The data as a memory stream.</returns>
        public MemoryStream GetDataMemoryStream() {
            return new MemoryStream(GetDataArray(), false);
        }
    }
}

