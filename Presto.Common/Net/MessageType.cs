using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Common.Net {
    /// <summary>
    /// A type safe enum holding all available message types.
    /// Message types are an 8 character ASCII string passed as an identifier to the recieving end of the tcp connection as to what
    /// to dispatch the request to. THe message type will be the first 8 characters of a message in ASCII. Consequently the message type
    /// is the first 8 bytes.
    /// </summary>
    public sealed class MessageType {

        //The given message was unkown or unparsable
        public static readonly MessageType UNKOWN = new MessageType("99999999");

        //--------Assembly transfer messages------------//
        public static readonly MessageType ASSEMBLY_INCOMING = new MessageType("10000000");



        /* Beneath is entirely internal to the type safe enum pattern used for the MessageType class
         * Not to be changed. 
         */
        private readonly string name;
        private static readonly Dictionary<string, MessageType> instance = new Dictionary<string, MessageType>();

        /// <summary>
        /// Internal constructor for the type safe enum MessageType
        /// </summary>
        /// <param name="name"></param>
        private MessageType(string name)
        {
            this.name = name;
            instance[name] = this;
        }

        /// <summary>
        /// Gives the chosen Message Type as a string value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Implicit conversion from Message Type to String
        /// </summary>
        /// <param name="mtype"></param>
        /// <returns>The string representation of the MessageType</returns>
        public static implicit operator string(MessageType mtype)
        {
            return mtype.name;
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
