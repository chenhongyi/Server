using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    internal static class StringEx
    {
        internal static string SafeFormat(string fmt, params object[] args)
        {
            if (fmt == null)
            {
                fmt = string.Empty;
            }

            if (args != null && args.Length > 0)
            {
                try
                {
                    fmt = string.Format(fmt, args);
                }
                catch (Exception)
                {
                    // we don't want a bad logging string to ruin the whole app
                }
            }

            return fmt;
        }
    }
}
