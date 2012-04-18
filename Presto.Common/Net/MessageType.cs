using System.Collections.Generic;

namespace Presto.Net {
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

        //--------Server Startup and Connect------------//
        /// <summary>
        /// The server wishes to connect, sent from client
        /// </summary>
        public static readonly MessageType CONNECT;
        /// <summary>
        /// The recieving server has accepted the connection and is ready to recieve requests
        /// </summary>
        public static readonly MessageType CONNECTION_ACCEPTED;
        /// <summary>
        /// An existing connection exists between the two servers
        /// </summary>
        public static readonly MessageType CONNECTION_EXISTS;

        //--------Assembly and Domain transfer messages------------//
        /// <summary>
        /// An assembly binary is attatched to this message and the binary needs to be loaded by the recieving PrestoServer
        /// </summary>
        public static readonly MessageType ASSEMBLY_TRANSFER_MASTER;
        /// <summary>
        /// An assembly binary is attatched to this message and the recieving slave server needs to load the binary and await instructions
        /// </summary>
        public static readonly MessageType ASSEMBLY_TRANSFER_SLAVE;
        /// <summary>
        /// A response notifying the sending service of a succesfull assembly transfer.
        /// </summary>
        public static readonly MessageType ASSEMBLY_TRANSFER_COMPLETE;
        /// <summary>
        /// Tell the node to unload a aprticular assembly.
        /// </summary>
        public static readonly MessageType DOMAIN_UNLOAD;

        //---------Process Execution messages-----------//
        /// <summary>
        /// An error response indicating that the Execution cannot be processed as the recieving server does not have 
        /// the assembly to execute in.
        /// </summary>
        public static readonly MessageType MISSING_ASSEMBLY;
        /// <summary>
        /// Signals the recieving server to carryout an execution according to the serialized ExecutionContext object 
        /// carried in with the message.
        /// </summary>
        public static readonly MessageType EXECUTION_BEGIN;
        /// <summary>
        /// A return signal giving back the processed data from a completed execution in the form of
        /// a serialized ExecutionResult object.
        /// </summary>
        public static readonly MessageType EXECUTION_COMPLETE;
        /// <summary>
        /// In Presto, a recieving server has the authority to deny an execution. This result tells the signaling server
        /// that the execution has been denied.
        /// </summary>
        public static readonly MessageType EXECUTION_DENIED;

        //---------Cluster Maintenance Activities -------//
        /// <summary>
        /// A request for a server to verify itself.
        /// </summary>
        public static readonly MessageType VERIFY;
        /// <summary>
        /// The response from a verification.
        /// </summary>
        public static readonly MessageType VERIFICATION_RESPONSE;
        /// <summary>
        /// A status request from the command line presto app.
        /// </summary>
        public static readonly MessageType STATUS_TERMINAL;

        //---------User Scheduled Directives -----------//
        /// <summary>
        /// Send a message to a remote node.
        /// </summary>
        public static readonly MessageType USER_MESSAGE;



        /// <summary>
        /// The static constructor for the MessageType enum.
        /// </summary>
        static MessageType() {
            UNKOWN = new MessageType("99999999");
            CONNECT = new MessageType("00000001");
            CONNECTION_ACCEPTED = new MessageType("00000002");
            CONNECTION_EXISTS = new MessageType("00000003");
            ASSEMBLY_TRANSFER_MASTER = new MessageType("10000000");
            ASSEMBLY_TRANSFER_SLAVE = new MessageType("10000001");
            ASSEMBLY_TRANSFER_COMPLETE = new MessageType("10000002");
            DOMAIN_UNLOAD = new MessageType("10000003");
            MISSING_ASSEMBLY = new MessageType("20000000");
            EXECUTION_BEGIN = new MessageType("20000001");
            EXECUTION_COMPLETE = new MessageType("20000002");
            EXECUTION_DENIED = new MessageType("20000003");
            VERIFY = new MessageType("30000001");
            VERIFICATION_RESPONSE = new MessageType("30000002");
            STATUS_TERMINAL = new MessageType("30000003");
            USER_MESSAGE = new MessageType("40000001");
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
        private MessageType(string myName) {
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
        /// <param name="messageType"></param>
        /// <returns>The string representation of the MessageType</returns>
        public static implicit operator string(MessageType messageType) {
            return messageType.ToString();
        }

        /// <summary>
        /// Implicitly convert from string to MessageType
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns>Will return null if "messageType" matches no MessageType</returns>
        public static implicit operator MessageType(string messageType) {
            //be sure that the string is not null

            if (messageType == null) {
                return null;
            }

            MessageType result;
            if (instance.TryGetValue(messageType, out result)) {
                return result;
            } else {
                return null;
            }
        }
    }
}
