using System;
using System.IO;
using System.Text;

namespace Presto.Common {
    /// <summary>
    /// A common log for all program errors or debug statements to be written to
    /// </summary>
    public static class Log {
        //The log will be placed in the applications running directory
        private static string LOG_URL = "presto.log";
        //We need a lock object to synchronize the writes to the log
        private static Object syncLock = new Object();


        /// <summary>
        /// Log an Error.
        /// </summary>
        /// <param name='message'>
        /// The Error message to be logged.
        /// </param>
        public static void Error(string message) {
            string toWrite = "ERROR:" + Environment.NewLine + message + Environment.NewLine;
            byte[] bytesToWrite = UTF8Encoding.UTF8.GetBytes(toWrite);
            //lock for IO synchronization
            lock (syncLock) {
                FileStream fs = new FileStream(LOG_URL, FileMode.Append, FileAccess.Write);
                fs.Write(bytesToWrite, 0, bytesToWrite.Length);
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// Log a Warning.
        /// </summary>
        /// <param name='message'>
        /// The Warning message to be logged.
        /// </param>
        public static void Warning(string message) {
            string toWrite = "WARNING:" + Environment.NewLine + message + Environment.NewLine;
            byte[] bytesToWrite = UTF8Encoding.UTF8.GetBytes(toWrite);
            //lock for IO synchronization
            lock (syncLock) {
                FileStream fs = new FileStream(LOG_URL, FileMode.Append, FileAccess.Write);
                fs.Write(bytesToWrite, 0, bytesToWrite.Length);
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// Log a Generic statement.
        /// </summary>
        /// <param name='message'>
        /// The Generic message to be logged.
        /// </param>
        public static void Generic(string message) {
            string toWrite = "LOG:" + Environment.NewLine + message + Environment.NewLine;
            byte[] bytesToWrite = UTF8Encoding.UTF8.GetBytes(toWrite);
            //lock for IO synchronization
            lock (syncLock) {
                FileStream fs = new FileStream(LOG_URL, FileMode.Append, FileAccess.Write);
                fs.Write(bytesToWrite, 0, bytesToWrite.Length);
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// Write text to the console, will only work if debug mode is active.
        /// </summary>
        /// <param name="text">The text to write to the console.</param>
        public static void DebugConsole(string text) {
            Console.WriteLine(text);
        }

    }
}