using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.Common.Net {
    /// <summary>
    /// A structure holding all available message types.
    /// Message types are an 8 character ASCII string passed as an identifier to the recieving end of the tcp connection as to what
    /// to dispatch the request to. THe message type will be the first 8 characters of a message in ASCII. Consequently the message type
    /// is the first 8 bytes.
    /// </summary>
    struct MessageType {

        //The given message was unkown or unparsable
        public static string UNKOWN = "99999999";
    }
}
