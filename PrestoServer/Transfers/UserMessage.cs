﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Transfers {
    
    /// <summary>
    /// A struct for sending user messages between nodes.
    /// </summary>
    [Serializable]
    public struct UserMessage {
        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Message;
        /// <summary>
        /// The node id of the sender of the message.
        /// </summary>
        public string Sender;
        /// <summary>
        /// The Domain Key to deploy the message into.
        /// </summary>
        public string DomainKey;

        /// <summary>
        /// Create a new UserMessage Transfer struct.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        /// <param name="sender">The node id of the sender of the message.</param>
        /// <param name="domainID">The domain Key to deploy the message into.</param>
        public UserMessage(string message, string sender, string domainKey) {
            Message = message;
            Sender = sender;
            DomainKey = domainKey;
        }
    }
}
