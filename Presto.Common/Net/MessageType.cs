using System;
using System.Collections.Generic;

namespace Presto.Common.Net {
    /// <summary>
    /// A type safe enum holding all available message types.
    /// Message types are an 8 character ASCII string passed as an identifier to the recieving end of the tcp connection as to what
    /// to dispatch the request to. THe message type will be the first 8 characters of a message in ASCII. Consequently the message type
    /// is the first 8 bytes.
    /// </summary>
    public class MessageType {

        /// <summary>
        /// The given messange type was unknown or unparseable.
        /// </summary>
        public static readonly MessageType UNKOWN;

        //--------Assembly transfer messages------------//
        /// <summary>
        /// AN assembly binary is attatched to this message.
        /// </summary>
        public static readonly MessageType ASSEMBLY_INCOMING;


        /// <summary>
        /// The static constructor for the MessageType enum.
        /// </summary>
        static MessageType() {
            UNKOWN = new MessageType("99999999");
            ASSEMBLY_INCOMING = new MessageType("10000000");
        }



        /* Beneath is entirely internal to the type safe enum pattern used for the MessageType class
         * Not to be changed. 
         */
        private string name;
        private static Dictionary<string, MessageType> instance = new Dictionary<string, MessageType>();

        /// <summary>
        /// Internal constructor for the type safe enum MessageType
        /// </summary>
        /// <param name="myName">The name of the message type.</param>
        private MessageType(string myName)
        {
            name = myName;
            instance[name] = this;
        }

        /// <summary>
        /// Get the name of the enumeration as a string.
        /// </summary>
        /// <returns>The string representation of the enumeration.</returns>
        public override string ToString() {
            return name;
        }

        /// <summary>
        /// Implicit conversion from Message Type to String
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns>The string representation of the MessageType</returns>
        public static implicit operator string(MessageType mtype)
        {
            return mtype.ToString();
        }

        /// <summary>
        /// Implicitly convert from string to MessageType
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns>Will return null if "mtype" matches no MessageType</returns>
        public static implicit operator MessageType(string mtype)
        {
            //be sure that the string is not null

            if (mtype == null)
            {
                return null;
            }

            MessageType result;
            if (instance.TryGetValue(mtype, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
