using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Presto.Common.Net {
    /// <summary>
    /// ServerState is a state object that gets passed around as the holder for an asynchronous socket.
    /// </summary>
    public class ServerState {
        //the internal socket
        public Socket socket;
        // Size of receive Buffer.
        public const int BufferSize = 1024;
        // Receive Buffer.
        public byte[] Buffer = new byte[BufferSize];
        //An appendable byte list that will hold data being flushed from the Buffer
        private List<byte> data = new List<byte>();
        //a boolean to tell if the message is fully recieved
        private bool messageFullyRecieved = false;

        /// <summary>
        /// Create a new server state object to manage a currently running connection
        /// </summary>
        /// <param name="socket">The sync socket associated with the state object.</param>
        internal ServerState(Socket socket) {
            //set the working socket
            this.socket = socket;
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
            byte[] dataArray = data.ToArray();
            int index = ByteSearch(dataArray, Config.EndOfStreamPattern);
            if (Config.EndOfStreamPattern.Length + index >= dataArray.Length) {
                data.RemoveRange(index, Config.EndOfStreamPattern.Length);
                return new byte[0];
            }
            int excessIndex = Config.EndOfStreamPattern.Length + index;
            int excessLength = data.Count - excessIndex;
            byte[] excess = data.GetRange(excessIndex, excessLength).ToArray();
            data.RemoveRange(index, excessLength + Config.EndOfStreamPattern.Length);
            return excess;           
        }

        /// <summary>
        /// preset what data is in the data array.
        /// </summary>
        /// <param name="presetData">The data to preset the data array with.</param>
        internal void PreSetData(byte[] presetData){
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
            if (!(data.Count > 7)) {
                return null;
            }

            //get the message segment from the transmission, convert to string and convert the string to a message type
            List<byte> messageTypeByteList = data.GetRange(0, 8);
            byte[] messageTypeByteArray = messageTypeByteList.ToArray();
            return ASCIIEncoding.ASCII.GetString(messageTypeByteArray);
        }

        /// <summary>
        /// Sends the passed in data as the passed in message type and closes the socket.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="data"></param>
        public void WriteAndClose(MessageType messageType, byte[] data = null) {
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
            write(output.ToArray());

            //close the socket
            CloseSocket();
        }

        /// <summary>
        /// Sends the passed in data as the passed in message type
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="data"></param>
        public void Write(MessageType messageType, byte[] data = null) {
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
            write(output.ToArray());
        }

        /// <summary>
        /// Internal Write function. Writes the passed in data to the socket stream.
        /// </summary>
        /// <param name="data">the byte data to be written</param>
        private void write(byte[] data) {
            List<byte> tempByteArray = new List<byte>();
            tempByteArray.AddRange(data);
            tempByteArray.AddRange(Config.EndOfStreamPattern);
            data = tempByteArray.ToArray();

            //send the data
            socket.Send(data);
        }

        /// <summary>
        /// Closes the ServerState's associated socket
        /// </summary>
        public void CloseSocket() {
            //close the socket
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
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
            int indexOfEOS = ByteSearch(data.ToArray(), Config.EndOfStreamPattern);
            if (indexOfEOS > -1) {
                messageFullyRecieved = true;
            }
            else {
                messageFullyRecieved = false;
            }
        }

        /// <summary>
        /// Get the data portion of the message as a byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetDataArray() {
            List<byte> dataByteArray = data.GetRange(8, data.Count - 8);
            return dataByteArray.ToArray();
        }

        /// <summary>
        /// Get the data portion of the message as an ASCII encoded string
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// There needs to be an internal way of finding the end of a message, this is that.
        /// </summary>
        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0) {
            int found = -1;
            bool matched = false;
            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length) {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++) {
                    //if the start bytes match we will start comparing all other bytes
                    if (searchIn[i] == searchBytes[0]) {
                        if (searchIn.Length > 1) {
                            //multiple bytes to be searched we have to compare byte by byte
                            matched = true;
                            for (int y = 1; y <= searchBytes.Length - 1; y++) {
                                if (searchIn[i + y] != searchBytes[y]) {
                                    matched = false;
                                    break;
                                }
                            }
                            //everything matched up
                            if (matched) {
                                found = i;
                                break;
                            }

                        }
                        else {
                            //search byte is only one bit nothing else to do
                            found = i;
                            break; //stop the loop
                        }
                    }
                }
            }
            return found;
        }
    }
}

