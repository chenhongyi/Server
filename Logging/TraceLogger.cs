using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class TraceLogger : ILogger
    {
        private const string _prefix = "SFL!";
        private readonly string _logName;
        private readonly TraceSource traceSource;

        public TraceLogger(string logName)
        {
            this._logName = "[" + (logName ?? string.Empty) + "] ";

            this.traceSource = new TraceSource("TraceLogger", SourceLevels.All);
        }

        public virtual void Debug(string format, params object[] args)
        {
            this.traceSource.TraceInformation(this.Prefix(format), args);
        }

        public virtual void Info(string format, params object[] args)
        {
            this.traceSource.TraceInformation(this.Prefix(format), args);
        }

        public virtual void Warning(string format, params object[] args)
        {
            this.traceSource.TraceEvent(TraceEventType.Warning, 0, this.Prefix(format), args);
        }

        public virtual void Warning(Exception exception, string format, params object[] args)
        {
            format = string.Format(format, args);
            format += " " + exception.ToString();
            this.traceSource.TraceEvent(TraceEventType.Warning, 0, this.Prefix(format));
        }

        public void Error(Exception exception, string fmt, params object[] args)
        {
            fmt = StringEx.SafeFormat(fmt, args);
            fmt += " " + exception.ToString();
            this.traceSource.TraceEvent(TraceEventType.Error, 0, this.Prefix(fmt));
        }

        public virtual void TraceApi(string method, TimeSpan timespan)
        {
        }

        public virtual void TraceApi(string method, TimeSpan timespan, string format, params object[] vars)
        {
        }

        private string Prefix(string fmt)
        {
            return _prefix + this._logName + fmt;
        }
    }
}
