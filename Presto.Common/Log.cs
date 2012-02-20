using System;
using System.IO;

namespace Presto.Common
{
	/// <summary>
	/// A common log for all program errors or debug statements to be written to
	/// </summary>
	public static class Log
	{
		//The log will be placed in the applications running directory
		private const string LOG_URL = "presto.log";
		//We need a lock object to synchronize the writes to the log
		private Object syncLock = new Object ();
		
		
		/// <summary>
		/// Log an error.
		/// </summary>
		/// <param name='message'>
		/// The error message to be logged.
		/// </param>
		public static void error (string message)
		{
			string toWrite = "ERROR:" + Environment.NewLine + message + Environment.NewLine;e
			//lock for IO synchronization
			lock (syncLock) {
				FileStream fs = new FileStream (LOG_URL, FileMode.Append, FileAccess.Write);
			}			
		}
		
		/// <summary>
		/// Log a warning.
		/// </summary>
		/// <param name='message'>
		/// The warning message to be logged.
		/// </param>
		public static void warning (string message)
		{
			//lock for IO synchronization
			lock (syncLock) {
				
			}			
		}
		
		/// <summary>
		/// Log a generic debug statement.
		/// </summary>
		/// <param name='message'>
		/// The debug message to be logged.
		/// </param>
		public static void debug (sting message)
		{
			//lock for IO synchronization
			lock (syncLock) {
				
			}			
		}		
	
	}
}