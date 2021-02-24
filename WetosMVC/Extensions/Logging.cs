using System;
using System.Diagnostics;
using System.Text;

//using Microsoft.Practices.EnterpriseLibrary.Logging;
//using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
//using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;

namespace WetosMVCMainApp.Extensions
{
	public class Logging
	{
		private static string GetStack(Exception ex)
		{
			if (ex.GetBaseException() != null)
				return ex.GetBaseException().StackTrace;
			else if (ex.InnerException != null)
				return ex.InnerException.StackTrace;
			else
				return ex.StackTrace;
		}

		private static string RetrieveDetailedException(Exception ex)
		{
			StringBuilder sbException = new StringBuilder();

			if (ex != null)
				sbException.Append(ex.Message).Append(Environment.NewLine);

			if (ex.InnerException != null)
				sbException.Append("InnerException: ").
				Append(ex.InnerException.Message).
				Append(Environment.NewLine);

			Exception baseEx = ex.GetBaseException();
			if (baseEx != null)
				sbException.Append("Base Exception: ").
				Append(ex.GetBaseException().Message).
				Append(Environment.NewLine);

			return sbException.ToString();
		}

		/// <summary>
		/// Logs the errors.
		/// </summary>
		/// <param name="ex">The ex.</param>
		/// <param name="title">The title.</param>
		public static void LogErrors(Exception ex, string title)
		{
			LogEntry logEntry = new LogEntry();
			logEntry.Title = title + Environment.NewLine;
			logEntry.Message = ex.Message + Environment.NewLine + Environment.NewLine;
			logEntry.Message += "Stack: " + GetStack(ex).Replace("'", "''").Trim() + Environment.NewLine + Environment.NewLine;
			logEntry.Message += "DetailedException: " + RetrieveDetailedException(ex) + Environment.NewLine;
			logEntry.Severity = System.Diagnostics.TraceEventType.Error;
			Logger.Write(logEntry); 
		}

		public static bool LogExceptions(Exception ex, string policy)
		{
			return ExceptionPolicy.HandleException(ex, policy);
		}

		public static void WriteTrace(string logMessage, string traceCategory)
		{
			if (Logger.Writer.IsLoggingEnabled())
			{
				using (new Tracer(traceCategory))
				{
					LogEntry logEntry = new LogEntry();
					logEntry.Categories.Clear();
					logEntry.Categories.Add("Troubleshooting");
					logEntry.Priority = 5;
					logEntry.Severity = TraceEventType.Error;
					logEntry.Message = logMessage + "  Current activity=\"" + Trace.CorrelationManager.ActivityId + "\"";

					Logger.Write(logEntry);


				}
			}
		}
		/// <summary>
		/// Starts the perfomance log.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="layerName">Name of the layer.</param>
		/// <param name="functionName">Name of the function.</param>
		public static void StartPerfomanceLog(string userId, string layerName, string functionName)
		{
			PerformanceLog(userId, layerName, functionName, true);
		}

		/// <summary>
		/// Ends the perfomance log.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="layerName">Name of the layer.</param>
		/// <param name="functionName">Name of the function.</param>
		public static void EndPerfomanceLog(string userId, string layerName, string functionName)
		{
			PerformanceLog(userId, layerName, functionName, false);
		}

		/// <summary>
		/// Performances the log.
		/// </summary>
		/// <param name="userId">The user id.</param>
		/// <param name="layerName">Name of the layer.</param>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="isStarted">if set to <c>true</c> [is started].</param>
		private static void PerformanceLog(string userId, string layerName, string functionName, bool isStarted)
		{
			string startEnd = string.Empty;
			string performanceDetails = string.Empty;
			if (isStarted)
			{
				startEnd = "Start";
			}
			else
			{
				startEnd = "End";
			}

            performanceDetails = startEnd + " " + userId + " " + layerName + " " + functionName + " " + DateTime.Now.ToString();
			LogEntry logPerformance = new LogEntry();
			
			using (new Tracer(WETOSTraceCategory.PerformanceEvents))
			{
			
				logPerformance.ExtendedProperties.Add("", performanceDetails);
				Logger.Write(logPerformance);
			}
		}

	}

	public static class WETOSExceptionPolicy
	{
        public const string DefaultPolicy = "Default Policy";
	}

	public static class WETOSTraceCategory
	{
		public const string UIEvents = "UI Events";
		public const string PerformanceEvents = "Performance Events";
	}
}
