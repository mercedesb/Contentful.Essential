using log4net;
using log4net.Core;
using System;

namespace Contentful.Essential
{
	/// <summary>
	/// Static logger for system logging, this is mostly verbose debug logging not intended for sites in production.
	/// </summary>
	public static class SystemLog
	{
		/// <summary>
		/// Logging shortcut that adds information about type and method to the message
		/// </summary>
		public static void Log(object caller, string message, Level logLevel, Exception exception = null)
		{
			var type = caller?.GetType();
			Log(type, message, logLevel, exception);
		}

		/// <summary>
		/// Logging shortcut that adds information about type and method to the message
		/// </summary>
		public static void Log(Type callingType, string message, Level logLevel, Exception exception = null)
		{
			var formattedMessage = message;

			// create the message caller type, but only add it if it contain something
			if (callingType != null)
			{
				formattedMessage = $"{callingType.FullName}: {message}";
			}

			WriteLog(formattedMessage, logLevel, exception, "Contentful.Essential");
		}

		/// <summary>
		/// Log a message to the specified logger (or default)
		/// </summary>
		private static void WriteLog(string message, Level level, Exception exception = null, string logger = null)
		{
			var logInstance = LogManager.GetLogger(logger, string.Empty);
			Write(logInstance, message, level, exception);
		}

		/// <summary>
		/// Log a message for the specified type
		/// </summary>
		private static void WriteLog(Type callingType, string message, Level level, Exception exception = null)
		{
			var logInstance = LogManager.GetLogger(callingType);
			Write(logInstance, message, level, exception);
		}

		private static void Write(ILog logInstance, string message, Level level, Exception exception = null)
		{
			if (level >= Level.Fatal)
				logInstance.Fatal(message, exception);
			else if (level >= Level.Error)
				logInstance.Error(message, exception);
			else if (level >= Level.Warn)
				logInstance.Warn(message, exception);
			else if (level >= Level.Info)
				logInstance.Info(message, exception);
			else
				logInstance.Debug(message, exception);
		}
	}
}
