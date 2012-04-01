using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Presto.DataStructures.Distributed {
    
    /// <summary>
    /// Outgoing cluster dictionary update message to be passed to other nodes. Called when set is used.
    /// </summary>
    /// <typeparam name="T">The type of the value stored in the dictionary.</typeparam>
    [Serializable]
    struct ClusterDictionaryUpdate<T> where T : struct{

        T Value;
        string Receiver;
        string Sender;
        string DictionaryName;
        string KeyName;
        string DomainKey;
        long TimeStamp;

        public ClusterDictionaryUpdate(T value, string receiver, string sender, string domainKey, string dictionaryName, string keyName, long timeStamp){
            Value = value;
            Receiver = receiver;
            Sender = sender;
            DictionaryName = dictionaryName;
            KeyName = keyName;
            TimeStamp = timeStamp;
            DomainKey = domainKey;
        }
    }
}
