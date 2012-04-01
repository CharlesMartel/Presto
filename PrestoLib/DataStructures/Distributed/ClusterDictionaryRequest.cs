using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed {

    /// <summary>
    /// A request sent to a datastore node to retrieve the value of a particular piece of data.
    /// </summary>
    /// <typeparam name="T">The type of the data being sent.</typeparam>
    [Serializable]
    struct ClusterDictionaryRequest<T> where T : struct {
        
        string DictionaryName;
        string DomainKey;
        string KeyName;
        string Sender;
        string Receiver;
        string RequestID;

        public ClusterDictionaryRequest (string sender, string receiver, string dictionaryName, string keyName, string requestID, string domainKey) {
            Sender = sender;
            Receiver = receiver;
            DictionaryName = dictionaryName;
            KeyName = keyName;
            DomainKey = domainKey;
            RequestID = requestID;
        }
    }
}
