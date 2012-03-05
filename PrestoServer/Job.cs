using System;

namespace Presto {
    /// <summary>
    /// A holder for all active jobs
    /// </summary>
    struct Job {
        public Action<PrestoResult> Callback;
        public DateTime StartTime;
        public string ContextID;

        public Job(string contextid, DateTime startTime, Action<PrestoResult> callback) {
            Callback = callback;
            StartTime = startTime;
            ContextID = contextid;
        }
    }
}
