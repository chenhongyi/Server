using Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Logging
{
    public static class LoggingSource
    {
        public static void Initialize()
        {
            LoggerFactory.SetFactory(
                n => new TraceLogger(n)
                );
        }

        public static void Initialize(Action<string> message)
        {
            string logger = LoggingConfiguration.Instance.Logger ?? string.Empty;
            logger = logger.ToLower();

            if (logger == "delegatelogger")
            {
                LoggerFactory.SetFactory(
                    n => new DelegateLogger(n, message)
                    );
            }
            else if (logger == "compositelogger")
            {
                LoggerFactory.SetFactory(
                    n => new CompositeLogger(
                        new DelegateLogger(n, message),
                        new TraceLogger(n)
                        )
                    );
            }
            else if (logger == "nulllogger")
            {
                LoggerFactory.SetFactory(
                    n => new NullLogger()
                    );
            }
            else
            {
                LoggerFactory.SetFactory(
                    n => new TraceLogger(n)
                    );
            }
        }
    }
}
