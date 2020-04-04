using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Microsoft.Crm.Services.Utility
{
    internal class TraceLogger
    {
        private StringBuilder _LastError = new StringBuilder();
        private TraceSource source;
        private Exception _LastException;

        public string LastError
        {
            get
            {
                return this._LastError.ToString();
            }
        }

        public Exception LastException
        {
            get
            {
                return this._LastException;
            }
        }

        public SourceLevels CurrentTraceLevel
        {
            get
            {
                if (this.source != null)
                    return this.source.Switch.Level;
                return SourceLevels.Off;
            }
        }

        public TraceLogger(string traceSourceName)
        {
            if (string.IsNullOrWhiteSpace(traceSourceName))
                this.source = new TraceSource("CrmSvcUtil");
            else
                this.source = new TraceSource(traceSourceName);
        }

        public void ResetLastError()
        {
            this._LastError.Remove(0, this.LastError.Length);
            this._LastException = (Exception)null;
        }

        public void TraceInformation(string message, params object[] messagedata)
        {
            this.Log(string.Format((IFormatProvider)CultureInfo.CurrentUICulture, message, messagedata));
        }

        public void TraceWarning(string message, params object[] messagedata)
        {
            this.Log(string.Format((IFormatProvider)CultureInfo.CurrentUICulture, message, messagedata), TraceEventType.Warning);
        }

        public void TraceError(string message, params object[] messagedata)
        {
            this.Log(string.Format((IFormatProvider)CultureInfo.CurrentUICulture, message, messagedata), TraceEventType.Error);
        }

        public void TraceMethodStart(string message, params object[] messagedata)
        {
            this.Log(string.Format((IFormatProvider)CultureInfo.CurrentUICulture, message, messagedata), TraceEventType.Start);
        }

        public void TraceMethodStop(string message, params object[] messagedata)
        {
            this.Log(string.Format((IFormatProvider)CultureInfo.CurrentUICulture, message, messagedata), TraceEventType.Stop);
        }

        public void Log(string message)
        {
            this.source.TraceEvent(TraceEventType.Information, 8, message);
        }

        public void Log(string message, TraceEventType eventType)
        {
            this.source.TraceEvent(eventType, (int)eventType, message);
            if (eventType != TraceEventType.Error)
                return;
            this._LastError.Append(message);
        }

        public void Log(string message, TraceEventType eventType, Exception exception)
        {
            StringBuilder sw = new StringBuilder();
            sw.AppendLine("Message: " + message);
            TraceLogger.LogExceptionToFile(exception, sw, 0);
            if (sw.Length > 0)
                this.source.TraceEvent(TraceEventType.Error, 2, sw.ToString());
            else
                this.source.TraceEvent(eventType, (int)eventType, sw.ToString());
            if (eventType != TraceEventType.Error)
                return;
            this._LastError.Append(sw.ToString());
            this._LastException = exception;
        }

        public void Log(Exception exception)
        {
            StringBuilder sw = new StringBuilder();
            TraceLogger.LogExceptionToFile(exception, sw, 0);
            if (sw.Length > 0)
                this.source.TraceEvent(TraceEventType.Error, 2, sw.ToString());
            this._LastError.Append(sw.ToString());
            this._LastException = exception;
        }

        private static void LogExceptionToFile(Exception objException, StringBuilder sw, int level)
        {
            if (level != 0)
                sw.AppendLine(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Inner Exception Level {0}\t: ", (object)level));
            sw.AppendLine("Source\t: " + (objException.Source != null ? objException.Source.ToString().Trim() : "Not Provided"));
            sw.AppendLine("Method\t: " + (objException.TargetSite != (MethodBase)null ? objException.TargetSite.Name.ToString() : "Not Provided"));
            sw.AppendLine("Date\t: " + DateTime.Now.ToLongTimeString());
            sw.AppendLine("Time\t: " + DateTime.Now.ToShortDateString());
            sw.AppendLine("Error\t: " + (string.IsNullOrEmpty(objException.Message) ? "Not Provided" : objException.Message.ToString().Trim()));
            sw.AppendLine("Stack Trace\t: " + (string.IsNullOrEmpty(objException.StackTrace) ? "Not Provided" : objException.StackTrace.ToString().Trim()));
            sw.AppendLine("======================================================================================================================");
            ++level;
            if (objException.InnerException == null)
                return;
            TraceLogger.LogExceptionToFile(objException.InnerException, sw, level);
        }
    }
}