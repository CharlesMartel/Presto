using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed {

    /// <summary>
    /// After a request is sent out to retrieve a particular piece fo data for a clustered dictionary this response is sent back.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the dictionary.</typeparam>
    [Serializable]
    class ClusterDictionaryRequestResponse<T> where T : struct {
        T Value;
        string Receiver;
        string Sender;
        string DictionaryName;
        string KeyName;
        string DomainKey;
        string RequestID;
        long TimeStamp;

        public ClusterDictionaryRequestResponse(T value, string receiver, string sender, string domainKey, string dictionaryName, string keyName, string requestID, long timeStamp){
            Value = value;
            Receiver = receiver;
            Sender = sender;
            DictionaryName = dictionaryName;
            KeyName = keyName;
            TimeStamp = timeStamp;
            DomainKey = domainKey;
            RequestID = requestID;
        }
    }


}
