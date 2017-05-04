using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class CompositeLogger : ILogger
    {
        private readonly List<ILogger> _loggers;

        public CompositeLogger()
        {
            this._loggers = new List<ILogger>();
        }

        public CompositeLogger(params ILogger[] loggers)
        {
            this._loggers = new List<ILogger>(loggers);
        }

        public void Debug(string fmt, params object[] vars)
        {
            this._loggers.ForEach(l => l.Debug(fmt, vars));
        }

        public void Info(string fmt, params object[] vars)
        {
            this._loggers.ForEach(l => l.Info(fmt, vars));
        }

        public void Warning(string fmt, params object[] vars)
        {
            this._loggers.ForEach(l => l.Warning(fmt, vars));
        }

        public void Warning(Exception exception, string fmt, params object[] args)
        {
            this._loggers.ForEach(l => l.Warning(exception, fmt, args));
        }

        public void Error(Exception exception, string fmt, params object[] args)
        {
            this._loggers.ForEach(l => l.Error(exception, fmt, args));
        }
    }
}
