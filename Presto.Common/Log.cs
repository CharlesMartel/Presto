using System;
using System.IO;
using System.Text;

namespace Presto.Common
{
	/// <summary>
	/// A common log for all program errors or debug statements to be written to
	/// </summary>
	public static class Log
	{
		//The log will be placed in the applications running directory
		private static const string LOG_URL = "presto.log";
		//We need a lock object to synchronize the writes to the log
		private static Object syncLock = new Object ();
		
		
		/// <summary>
		/// Log an error.
		/// </summary>
		/// <param name='message'>
		/// The error message to be logged.
		/// </param>
		public static void error (string message)
		{
            string toWrite = "ERROR:" + Environment.NewLine + message + Environment.NewLine;
            byte[] bytesToWrite = UTF8Encoding.UTF8.GetBytes(toWrite);
			//lock for IO synchronization
			lock (syncLock) {
                FileStream fs = new FileStream (LOG_URL, FileMode.Append, FileAccess.Write);
                fs.Write(bytesToWrite, 0, bytesToWrite.Length);
                fs.Close();
                fs.Dispose();
			}			
		}

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name='message'>
        /// The warning message to be logged.
        /// </param>
        public static void warning(string message)
		{
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
		/// Log a generic statement.
		/// </summary>
		/// <param name='message'>
		/// The generic message to be logged.
		/// </param>
		public static void generic (string message)
		{
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
	
	}
}