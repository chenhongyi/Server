using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class NullLogger : ILogger
    {
        public NullLogger()
        {
        }

        public void Debug(string format, params object[] args)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Warning(string format, params object[] args)
        {
        }

        public void Warning(Exception exception, string format, params object[] args)
        {
        }

        public void Error(Exception exception, string fmt, params object[] vars)
        {
        }

        public void Error(string fmt, params object[] vars)
        {
        }

        public void Error(string errorType, string format, params object[] args)
        {
        }

        public void TraceApi(string method, TimeSpan timespan)
        {
        }

        public void TraceApi(string method, TimeSpan timespan, string format, params object[] vars)
        {
        }

        public void TraceError(Exception exception, string format, params object[] args)
        {
        }

        public void TraceError(string errorType, string format, params object[] args)
        {
        }
    }
}
