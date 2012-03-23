using System;

namespace Presto.Transfers {

    /// <summary>
    /// A holder for all active jobs
    /// </summary>
    struct OutboundJob {
        public Action<PrestoResult> Callback;
        public DateTime StartTime;
        public string ContextID;
        public string DomainKey;

        public OutboundJob(string contextid, DateTime startTime, Action<PrestoResult> callback, string domainKey) {
            Callback = callback;
            StartTime = startTime;
            ContextID = contextid;
            DomainKey = domainKey;
        }
    }
}
